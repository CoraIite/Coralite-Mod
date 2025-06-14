using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵的基类，用于在捕捉的时候以及仙灵瓶里的行为
    /// </summary>
    public abstract class Fairy : ModTexturedType
    {
        public int Type { get; internal set; }

        public override string Texture => AssetDirectory.FairyItems + Name;

        public virtual int HorizontalFrames { get => 1; }
        public virtual int VerticalFrames { get => 1; }

        /// <summary>
        /// 是否存活，在捕捉器内使用
        /// </summary>
        public bool active;
        /// <summary>
        /// BOSS级仙灵，会阻止其他仙灵生成
        /// </summary>
        public bool BOSS;

        /// <summary>
        /// 捕捉最大值
        /// </summary>
        public float CatchProgressMax { get; } = 60;
        /// <summary>
        /// 捕捉进度
        /// </summary>
        public float CatchProgress {  get; set; }

        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public int width = 12;
        public int height = 12;
        public float scale = 1;
        public float alpha = 1;

        public int frameCounter;
        public Rectangle frame;
        public int spriteDirection = 1;

        public int FairyTimer;
        private AIState State;

        /// <summary>
        /// 在捕捉器内的ID
        /// </summary>
        public int IDInCatcher {  get;private set; }

        private enum AIState
        {
            Spawning,
            FreeMoving,
            Catching,
            Fading
        }


        /// <summary>
        /// 为false时就不会能被捉，进度不会涨
        /// </summary>
        public bool canBeCaught;

        /// <summary>
        /// 未进入捕捉状态而自由移动的时间，大于一定值后直接消失
        /// </summary>
        public int freeMoveTimer = 10 * 60;
        /// <summary>
        /// 捕捉时间，在开始捕捉时设置为<see cref="MaxCatchTime"/> ，为0时逃走
        /// </summary>
        public int CatchTime;

        /// <summary>
        /// 最大捕捉时间
        /// </summary>
        public virtual int MaxCatchTime { get => 60 * 15; }

        /// <summary>
        /// 自身的稀有度，请与出现条件中的相对应
        /// <br>默认<see cref=" FairyRarity.C"/></br>
        /// </summary>
        public virtual FairyRarity Rarity => FairyRarity.C;
        /// <summary>
        /// 物品类型
        /// </summary>
        public abstract int ItemType { get; }

        public Vector2 Center
        {
            get => position + new Vector2(width / 2, height / 2);
            set => position = value - new Vector2(height / 2, width / 2);
        }

        public Vector2 Bottom
        {
            get => position + new Vector2(width / 2, height);
            set => position = value - new Vector2(height / 2, width);
        }

        public Rectangle HitBox => new((int)position.X, (int)position.Y, width, height);

        /// <summary>
        /// 仙灵是否在上一次更新中超出了捕捉圈
        /// </summary>
        public bool OutOfCircle { get; set; }

        /// <summary>
        /// 在开始捕捉的时候调用
        /// </summary>
        public event Action OnStartCatch;

        protected sealed override void Register()
        {
            ModTypeLookup<Fairy>.Register(this);

            FairyLoader.fairys ??= [];
            FairyLoader.fairys.Add(this);

            Type = FairyLoader.ReserveFairyID();
        }

        public virtual Fairy NewInstance()
        {
            var inst = (Fairy)Activator.CreateInstance(GetType(), true);
            inst.Type = Type;
            return inst;
        }

        /// <summary>
        /// 用于注册生成方式，详细参考<see cref="FairySystem"/>
        /// </summary>
        public virtual void RegisterSpawn() { }

        #region 捕获器中的AI

        /// <summary>
        /// 设置仙灵的各种默认值
        /// </summary>
        /// <param name="attempt"></param>
        public void Spawn(FairyAttempt attempt)
        {
            active = true;
            canBeCaught = true;
            scale = 1;
            position = new Vector2(attempt.X, attempt.Y) * 16;
            alpha = 0;
            FairyTimer = 60;

            IDInCatcher = attempt.catcherProj.GetFairyID();

            OnSpawnAndSetDefault(attempt);
        }

        /// <summary>
        /// 设置初始值<br></br>
        /// 默认<see cref="width"/><see cref="height "/>为8<br></br>
        /// 默认<see cref="FairyTimer"/>为60，会逐渐减小
        /// </summary>
        public virtual void OnSpawnAndSetDefault(FairyAttempt attempt) { }

        /// <summary>
        /// 在捕捉器内的行为
        /// </summary>
        public void UpdateInCatcher(FairyCatcherProj catcher)
        {
            AI_InCatcher(catcher);

            if (ShouldUpdatePosition())
                position += velocity;

            //限制不能出圈
            CircleLimit(catcher);

            switch (State)
            {
                case AIState.Spawning:
                case AIState.Fading:
                default:
                    break;

                case AIState.FreeMoving://没开始捕捉的时候，到点就消失
                    {
                        freeMoveTimer--;
                        if (freeMoveTimer < 0)
                        {
                            TurnToFading();
                            return;
                        }
                    }
                    break;
                case AIState.Catching:
                    {
                        CatchTime--;
                        //如果减小到0就消失
                        if (CatchTime <= 0)
                            TurnToFading();
                        else if (CatchProgress > CatchProgressMax)//捕捉
                            BeCaught(catcher.Owner);
                    }
                    break;
            }
        }

        /// <summary>
        /// 限制仙灵的位置，让它不会出捕捉圈
        /// </summary>
        /// <param name="catcher"></param>
        public virtual void CircleLimit(FairyCatcherProj catcher)
        {
            Vector2 webCenter = catcher.webCenter;
            if (Vector2.Distance(Center, webCenter) > catcher.webRadius)
            {
                OutOfCircle = true;
                Center = webCenter + ((Center - webCenter).SafeNormalize(Vector2.Zero) * catcher.webRadius);
            }
            else
                OutOfCircle = false;    
        }

        /// <summary>
        /// 在捕捉器内的AI
        /// </summary>
        public virtual void AI_InCatcher(FairyCatcherProj catcher)
        {
            PreAI_InCatcher();

            switch (State)
            {
                case AIState.Spawning:
                    Spawning();
                    break;
                case AIState.FreeMoving:
                    FreeMoving();
                    break;
                case AIState.Catching:
                    Catching(catcher);
                    break;
                default:
                case AIState.Fading:
                    Fading();
                    break;
            }

            PostAI_InCatcher();
        }

        /// <summary>
        /// 可以在这里更新贴图等
        /// </summary>
        public virtual void PreAI_InCatcher() { }
        public virtual void PostAI_InCatcher() { }

        /// <summary>
        /// 生成中，1秒钟时间的渐入效果
        /// </summary>
        public virtual void Spawning()
        {
            FairyTimer--;

            if (FairyTimer < 1)
                State = AIState.FreeMoving;

            alpha = 1 - (FairyTimer / 60f);
        }

        /// <summary>
        /// 在被捕捉时调用
        /// </summary>
        public virtual void Catching(FairyCatcherProj catcher) { }

        /// <summary>
        /// 在捕捉环内自由移动时调用
        /// </summary>
        public virtual void FreeMoving() { }
        /// <summary>
        /// 在消失时调用，1秒钟淡出
        /// </summary>
        public virtual void Fading()
        {
            FairyTimer++;
            alpha -= 1 / 60f;
            if (FairyTimer > 60)
                Despawn();
        }

        public virtual void TurnToFading()
        {
            State = AIState.Fading;
            FairyTimer = 0;
        }

        public void Despawn()
        {
            active = false;
            OnDespawn();
        }

        /// <summary>
        /// 在消失时调用
        /// </summary>
        public virtual void OnDespawn() { }

        /// <summary>
        /// 是否会根据<see cref="velocity"/>更新<see cref="position"/>
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldUpdatePosition() => true;

        /// <summary>
        /// 在被捕捉时调用
        /// </summary>
        public virtual void Catch(int catchPower)
        {
            if (State == AIState.FreeMoving)
            {
                State = AIState.Catching;
                alpha = 1;
                CatchTime = MaxCatchTime;
                OnStartCatch?.Invoke();
                OnStartCatch = null;
            }

            CatchProgress += catchPower;
        }

        /// <summary>
        /// 被捕获时执行，生成物品
        /// </summary>
        public void BeCaught(Player player)
        {
            active = false;

            //new一个物品出来
            Item i = new(ItemType);

            //为物品的字段赋值，如果这个物品不是一个仙灵那么就跳过
            if (i.ModItem is BaseFairyItem fairyitem && player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fairyitem.Initialize(FairyIV.GetFairyIV(this, fcp));

            //调用onCatch
            OnBeCaught(player, i);

            //在玩家处生成物品
            player.QuickSpawnItem(player.GetSource_FairyCatch(this), i);
            Chest.VisualizeChestTransfer(Center, player.Center, i, 1);
            Helper.PlayPitched("Fairy/CatchFairy", 0.4f, 0);
            FairySystem.SetFairyCaught(this);
        }

        /// <summary>
        /// 在捕获时调用
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnBeCaught(Player player, Item fairyItem) { }

        /// <summary>
        /// 自定义仙灵的个体值，例如某些仙灵固定个体值等
        /// </summary>
        /// <param name="fairyIV"></param>
        public virtual void ModifyIV(ref FairyIV fairyIV,FairyCatcherPlayer fcp) { }

        #endregion

        #region 帮助方法

        public void SetDirectionNormally()
        {
            spriteDirection = Math.Sign(velocity.X);
        }

        public void UpdateFrameY(int spacing)
        {
            if (++frameCounter > spacing)
            {
                frameCounter = 0;
                if (++frame.Y >= VerticalFrames)
                {
                    frame.Y = 0;
                }
            }
        }

        #endregion

        #region 绘制

        /// <summary>
        /// 在捕捉器内的绘制
        /// </summary>
        public virtual void Draw_InCatcher()
        {
            this.QuickDraw(Color.White * alpha, 0);

            //Texture2D mainTex = ModContent.Request<Texture2D>(Texture).Value;
            //var frame = mainTex.Frame(HorizontalFrames, VerticalFrames, this.frame.X, this.frame.Y);

            //Main.spriteBatch.Draw(mainTex, Center - Main.screenPosition, frame, Color.White * alpha, rotation, frame.Size() / 2, scale, spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            DrawProgressBar();
        }

        public virtual void DrawProgressBar()
        {
            if (State == AIState.Catching)
                DrawFairyProgressBar(Bottom + new Vector2(0, 14), CatchProgress, CatchProgressMax);
        }

        ///// <summary>
        ///// 在仙灵瓶里的绘制
        ///// </summary>
        //public virtual void Draw_InBottle()
        //{
        //    Texture2D mainTex = ModContent.Request<Texture2D>(Texture).Value;
        //    var frame = mainTex.Frame(HorizontalFrames, VerticalFrames, this.frame.X, this.frame.Y);

        //    Main.spriteBatch.Draw(mainTex, Center - Main.screenPosition, frame, Color.White, rotation, frame.Size() / 2, scale, spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        //}

        //public Texture2D GetTexture() => ModContent.Request<Texture2D>(Texture).Value;

        public void DrawFairyProgressBar(Vector2 center, float Health, float MaxHealth, float alpha = 0.9f)
        {
            if (Health <= 0)
                return;

            float factor = Health / (float)MaxHealth;
            if (factor > 1f)
                factor = 1f;

            Color backColor = Color.DarkGreen;
            Color barColor = Color.LawnGreen;
            float totalBarLength = width * 3f;


            if (BOSS)//boss仙灵特殊颜色
            {
                backColor = Color.DarkGoldenrod;
                barColor = Color.LightGoldenrodYellow;
                totalBarLength = width * 4;
            }

            ModifyProgressBarDraw(ref center,ref backColor, ref barColor, ref totalBarLength);

            Texture2D tex = CoraliteAssets.Sparkle.BarSPA.Value;

            float scale = totalBarLength / tex.Width * this.scale;
            backColor *= alpha;
            barColor *= alpha * factor;

            barColor.A = 0;

            center -= Main.screenPosition;


            //绘制底部条，固定绘制一个横杠
            tex.QuickCenteredDraw(Main.spriteBatch, center, backColor, scale: scale);

            //绘制顶部，裁剪矩形绘制
            Rectangle rect = new Rectangle(0, 0, (int)(factor * tex.Width), tex.Height);
            Main.spriteBatch.Draw(tex, center + new Vector2(-scale * tex.Width / 2, 0), rect, barColor, 0, new Vector2(0, tex.Height / 2), scale, 0, 0);

            //绘制指针
            tex = CoraliteAssets.Sparkle.ShotLineSPA.Value;
            scale = totalBarLength / tex.Width * this.scale;
            Vector2 pos = center + new Vector2(-scale * tex.Width / 2 + factor * scale * tex.Width, 0);

            Vector2 scale1 = new(scale * tex.Height / tex.Width * 0.66f, scale);
            Main.spriteBatch.Draw(tex, pos, null, backColor
                , MathHelper.PiOver2, tex.Size() / 2, scale1, 0, 0);
            Main.spriteBatch.Draw(tex, pos, null, barColor
                , MathHelper.PiOver2, tex.Size() / 2, scale1, 0, 0);

            //return;

            ////绘制条的背景
            //Main.spriteBatch.Draw(FairySystem.ProgressBarOuter.Value, center, null, backColor,
            //    0f, FairySystem.ProgressBarOuter.Size() / 2, scale, SpriteEffects.None, 0f);

            //Texture2D innerTex = FairySystem.ProgressBarInner.Value;
            //var topLeft = new Vector2(center.X - (innerTex.Width * scale / 2), center.Y - (innerTex.Height * scale / 2));

            //var source = new Rectangle(0, 0, (int)(innerTex.Width * factor), innerTex.Height);

            //Main.spriteBatch.Draw(innerTex, topLeft, source, barColor,
            //    0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public virtual void ModifyProgressBarDraw(ref Vector2 center,ref Color backColor,ref Color barColor,ref float totalBarLength)
        {

        }

        #endregion
    }
}
