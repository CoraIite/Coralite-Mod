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
        /// 是否在捕捉，当指针接触且玩家鼠标左键后设为true<br></br>
        /// 之后将开始捕捉
        /// </summary>
        public bool catching;

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
        public int width;
        public int height;
        public float scale;
        public float alpha = 1;

        public int frameCounter;
        public Rectangle frame;
        public int spriteDirection;

        public int Timer;

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

            if (catcher.CursorBox.Intersects(HitBox))//鼠标接触到了
            {
                catcher.cursorIntersects = true;
                cursorIntersects = false;

                if (Main.mouseLeft)
                {
                    catching = true;
                    cursorIntersects = true;
                    if (canBeCaught)
                    {
                        float progressAdder = ProgressAdder;
                        if (catcher.Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                            progressAdder = fcp.fairyCatchPowerBonus.ApplyTo(progressAdder);
                        catchProgress += progressAdder;
                    }
                }
            }
            else if (catching)//鼠标没碰到，并且正在捕捉中，那么减少条
            {
                cursorIntersects = false;
                ReduceProgress();
            }
            else//没开始捕捉的时候，到点就消失
            {
                freeMoveTimer++;
                if (freeMoveTimer > despawnTime)
                {
                    Despawn();
                    return;
                }
            }

            //Main.NewText(catchProgress);
            //如果减小到0就消失
            if (catchProgress <= 0)
                Despawn();
            else if (catchProgress > 100)//捕捉
                Catch(catcher.Owner);
        }

        public void Spawn(FairyAttempt attempt)
        {
            active = true;
            canBeCaught = true;
            scale = 1;
            width = 8;
            height = 8;
            position = new Vector2(attempt.X, attempt.Y) * 16;

            OnSpawn();
        }

        public virtual void OnSpawn() { }

        /// <summary>
        /// 在捕捉器内的AI
        /// </summary>
        public virtual void AI_InCatcher(Rectangle cursor,BaseFairyCatcherProj catcher) { }

        public virtual bool ShouldUpdatePosition() => true;

        /// <summary>
        /// 正在捕捉的时候并且玩家未左键或者指针没接触，这是和减少捕捉进度<br></br>
        /// 默认25秒从100减到0
        /// </summary>
        public virtual void ReduceProgress()
        {
            catchProgress -= 100f / (60 * 25f);
        }

        #endregion

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

            //ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.ItemTransfer,
            //    new ParticleOrchestraSettings()
            //    {
            //        PositionInWorld = position,
            //        MovementVector = player.Center,
            //        UniqueInfoPiece = i.type
            //    });
        }

        /// <summary>
        /// 在捕获时调用
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnCatch(Player player, Item fairyItem) { }

        /// <summary>
        /// 在仙灵瓶物块中的AI
        /// </summary>
        /// <param name="limit"></param>
        public virtual void AI_InBottle(Rectangle limit)
        {

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
        /// 在捕捉器内的绘制
        /// </summary>
        public virtual void Draw_InCatcher()
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(Texture).Value;
            var frame = mainTex.Frame(HorizontalFrames, VerticalFrames, this.frame.X, this.frame.Y);

            Main.spriteBatch.Draw(mainTex, Center - Main.screenPosition, frame, Color.White, rotation, frame.Size() / 2, scale, spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            DrawProgressBar();
        }

        public virtual void DrawProgressBar()
        {
            if (catching)
                DrawFairyProgressBar(Bottom.X, Bottom.Y + 14, (int)catchProgress, 100, 0.9f, 0.75f);
        }

        /// <summary>
        /// 在仙灵瓶里的绘制
        /// </summary>
        public virtual void Draw_InBottle()
        {

        }

        public Texture2D GetTexture() => ModContent.Request<Texture2D>(Texture).Value;

        public static void DrawFairyProgressBar(float X, float Y, int Health, int MaxHealth, float alpha, float scale = 1f)
        {
            if (Health <= 0)
                return;

            float factor = Health / (float)MaxHealth;
            if (factor > 1f)
                factor = 1f;

            Color backColor = Color.DarkGreen;
            Color barColor = Color.LawnGreen;

            //if (Main.LocalPlayer.TryGetModPlayer(out FairyCatcherPlayer fcp))
            //{
            //    backColor = fcp.CatcherBackColor * 0.8f;
            //    backColor.A = 255;
            //    barColor = fcp.CatcherBackColor * 1.5f;
            //}

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
    }
}
