using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        /// 和鼠标接触
        /// </summary>
        public bool cursorIntersects;

        /// <summary>
        /// 0-100的捕获进度，到达100则表示捉到，初始值为20
        /// </summary>
        public float catchProgress = 20f;

        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public int width = 8;
        public int height = 8;
        public float scale = 1;
        public float alpha = 1;

        public int frameCounter;
        public Rectangle frame;
        public int spriteDirection = 1;

        public int Timer;
        private AIState State;

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
        public int freeMoveTimer;
        public int despawnTime = 60 * 20;

        /// <summary>
        /// 当被捕捉时的进度增加量，默认从0加到100需要15秒
        /// </summary>
        public virtual float ProgressAdder { get => 100f / (60 * 15f); }

        /// <summary>
        /// 自身的稀有度，请与出现条件中的相对应
        /// <br>默认<see cref=" FairyAttempt.Rarity.C"/></br>
        /// </summary>
        public virtual FairyAttempt.Rarity Rarity => FairyAttempt.Rarity.C;
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

        public Rectangle HitBox => new Rectangle((int)position.X, (int)position.Y, width, height);

        protected sealed override void Register()
        {
            ModTypeLookup<Fairy>.Register(this);

            FairyLoader.fairys ??= new List<Fairy>();
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

        public void Spawn(FairyAttempt attempt)
        {
            active = true;
            canBeCaught = true;
            scale = 1;
            width = 8;
            height = 8;
            position = new Vector2(attempt.X, attempt.Y) * 16;
            alpha = 0;
            Timer = 60;

            OnSpawn();
        }

        /// <summary>
        /// 设置初始值<br></br>
        /// 默认<see cref="width"/><see cref="height "/>为8<br></br>
        /// 默认<see cref="Timer"/>为60，会逐渐减小
        /// </summary>
        public virtual void OnSpawn() { }

        /// <summary>
        /// 在捕捉器内的行为
        /// </summary>
        public void UpdateInCatcher(BaseFairyCatcherProj catcher)
        {
            AI_InCatcher(catcher.GetCursorBox(), catcher);

            if (ShouldUpdatePosition())
                position += velocity;

            //限制不能出圈
            Vector2 webCenter = catcher.webCenter.ToWorldCoordinates();
            if (Vector2.Distance(Center, webCenter) > catcher.webRadius)
                Center = webCenter + (Center - webCenter).SafeNormalize(Vector2.Zero) * catcher.webRadius;

            switch (State)
            {
                case AIState.Spawning:
                case AIState.Fading:
                default:
                    break;

                case AIState.FreeMoving://没开始捕捉的时候，到点就消失
                    {
                        freeMoveTimer++;
                        if (freeMoveTimer > despawnTime)
                        {
                            TurnToFading();
                            return;
                        }

                        if (catcher.CursorBox.Intersects(HitBox) && Main.mouseLeft)//开始捕捉
                        {
                            State = AIState.Catching;
                            alpha = 1;
                        }
                    }
                    break;
                case AIState.Catching:
                    {
                        if (catcher.CursorBox.Intersects(HitBox))//鼠标接触到了
                        {
                            catcher.cursorIntersects = true;
                            cursorIntersects = false;

                            if (Main.mouseLeft)
                            {
                                cursorIntersects = true;
                                if (canBeCaught)
                                {
                                    float progressAdder = ProgressAdder;
                                    if (catcher.Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                                        fcp.TotalCatchPowerBonus(ref progressAdder, catcher.Owner.HeldItem);
                                    catchProgress += progressAdder;
                                }
                            }
                        }
                        else//鼠标没碰到，并且正在捕捉中，那么减少条
                        {
                            cursorIntersects = false;
                            ReduceProgress();
                        }

                        //如果减小到0就消失
                        if (catchProgress <= 0)
                            TurnToFading();
                        else if (catchProgress > 100)//捕捉
                            Catch(catcher.Owner);

                    }
                    break;
            }
        }

        /// <summary>
        /// 在捕捉器内的AI
        /// </summary>
        public virtual void AI_InCatcher(Rectangle cursor, BaseFairyCatcherProj catcher)
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
                    {
                        if (cursorIntersects)
                            OnCursorIntersects(cursor, catcher);

                        Catching(cursor, catcher);
                    }
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

        public virtual void Spawning()
        {
            Timer--;

            if (Timer < 1)
                State = AIState.FreeMoving;

            alpha = 1 - Timer / 60f;
        }

        /// <summary>
        /// 在被捕捉时调用
        /// </summary>
        public virtual void Catching(Rectangle cursor, BaseFairyCatcherProj catcher) { }
        /// <summary>
        /// 在捕捉时并且鼠标接触的时候调用
        /// </summary>
        public virtual void OnCursorIntersects(Rectangle cursor, BaseFairyCatcherProj catcher) { }
        /// <summary>
        /// 在没被捕捉的时候调用
        /// </summary>
        public virtual void FreeMoving() { }
        /// <summary>
        /// 在消失时调用
        /// </summary>
        public virtual void Fading()
        {
            Timer++;
            alpha -= 1 / 60f;
            if (Timer > 60)
                Despawn();
        }

        public virtual void TurnToFading()
        {
            State = AIState.Fading;
            Timer = 0;
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

        public virtual bool ShouldUpdatePosition() => true;

        /// <summary>
        /// 正在捕捉的时候并且玩家未左键或者指针没接触，这是和减少捕捉进度<br></br>
        /// 默认25秒从100减到0
        /// </summary>
        public virtual void ReduceProgress()
        {
            catchProgress -= 100f / (60 * 25f);
        }

        /// <summary>
        /// 被捕获时执行
        /// </summary>
        public void Catch(Player player)
        {
            active = false;

            //new一个物品出来
            Item i = new Item(ItemType);

            //为物品的字段赋值，如果这个物品不是一个仙灵那么就跳过
            if (i.ModItem is BaseFairyItem fairyitem && player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                fairyitem.IV = fcp.RollFairyIndividualValues(fairyitem);
                fairyitem.Life = (int)fairyitem.FairyLifeMax;
            }

            //调用onCatch
            OnCatch(player, i);

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
        public virtual void OnCatch(Player player, Item fairyItem) { }

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

        /// <summary>
        /// 在仙灵瓶物块中的AI
        /// </summary>
        /// <param name="limit"></param>
        public virtual void AI_InBottle(Rectangle limit)
        {

        }

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
                DrawFairyProgressBar(Bottom.X, Bottom.Y + 14, (int)catchProgress, 100, 0.9f, 0.75f);
        }

        /// <summary>
        /// 在仙灵瓶里的绘制
        /// </summary>
        public virtual void Draw_InBottle()
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(Texture).Value;
            var frame = mainTex.Frame(HorizontalFrames, VerticalFrames, this.frame.X, this.frame.Y);

            Main.spriteBatch.Draw(mainTex, Center - Main.screenPosition, frame, Color.White, rotation, frame.Size() / 2, scale, spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        //public Texture2D GetTexture() => ModContent.Request<Texture2D>(Texture).Value;

        public static void DrawFairyProgressBar(float X, float Y, int Health, int MaxHealth, float alpha, float scale = 1f)
        {
            if (Health <= 0)
                return;

            float factor = Health / (float)MaxHealth;
            if (factor > 1f)
                factor = 1f;

            Color backColor = Color.DarkGreen;
            Color barColor = Color.LawnGreen;

            backColor *= alpha;
            barColor *= alpha;

            Vector2 center = new Vector2(X, Y) - Main.screenPosition;

            //绘制条的背景
            Main.spriteBatch.Draw(FairySystem.ProgressBarOuter.Value, center, null, backColor,
                0f, FairySystem.ProgressBarOuter.Size() / 2, scale, SpriteEffects.None, 0f);

            Texture2D innerTex = FairySystem.ProgressBarInner.Value;
            var topLeft = new Vector2(center.X - innerTex.Width * scale / 2, center.Y - innerTex.Height * scale / 2);

            var source = new Rectangle(0, 0, (int)(innerTex.Width * factor), innerTex.Height);

            Main.spriteBatch.Draw(innerTex, topLeft, source, barColor,
                0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        #endregion
    }
}
