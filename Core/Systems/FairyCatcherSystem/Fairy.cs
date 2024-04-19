using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Drawing;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵的基类，用于在捕捉的时候以及仙灵瓶里的行为
    /// </summary>
    public abstract class Fairy : ModTexturedType
    {
        public int Type { get; internal set; }

        public override string Texture => AssetDirectory.FairyItems + Name;

        /// <summary>
        /// 是否存活，在捕捉器内使用
        /// </summary>
        public bool active;
        /// <summary>
        /// 是否在捕捉，当指针接触且玩家鼠标左键后设为true<br></br>
        /// 之后将开始
        /// </summary>
        public bool catching;
        /// <summary>
        /// 0-100的捕获进度，到达100则表示捉到，初始值为10
        /// </summary>
        public float catchProgress = 10f;

        public Vector2 position;
        public Vector2 velocity;
        public int width;
        public int height;
        public float scale;
        public float alpha = 1;

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
        /// 当被捕捉时的进度增加量，默认从0加到100需要20秒
        /// </summary>
        public virtual float ProgressAdder { get => 100f / (60 * 20f); }

        /// <summary>
        /// 自身的稀有度，请与出现条件中的相对应
        /// <br>默认<see cref=" FairyAttempt.Rarity.C"/></br>
        /// </summary>
        public virtual FairyAttempt.Rarity Rarity => FairyAttempt.Rarity.C;
        /// <summary>
        /// 物品类型
        /// </summary>
        public abstract int ItemType { get; }

        public Vector2 Center => position + new Vector2(width, height);
        public Rectangle HitBox => new Rectangle((int)position.X, (int)position.Y, width, height);

        protected sealed override void Register()
        {
            ModTypeLookup<Fairy>.Register(this);

            FairyLoader.fairys ??= new List<Fairy>();
            FairyLoader.fairys.Add(this);

            Type = FairyLoader.ReserveParticleID();
        }

        public abstract int GetFairyItemType();

        public virtual Fairy NewInstance()
        {
            var inst = (Fairy)Activator.CreateInstance(GetType(), true);
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
            AI_InCatcher(catcher.GetCursorBox());

            if (ShouldUpdatePosition())
                position += velocity;

            if (catcher.CursorBox.Intersects(HitBox))//鼠标接触到了
            {
                catcher.cursorIntersects = true;
                if (Main.mouseLeft)
                {
                    catching = true;
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
                ReduceProgress();
            else//没开始捕捉的时候，到点就消失
            {
                freeMoveTimer++;
                if (freeMoveTimer > despawnTime)
                {
                    Despawn();
                    return;
                }
            }

            //如果减小到0就消失
            if (catchProgress <= 0)
                Despawn();
            else if (catchProgress > 100)//捕捉
                Catch(catcher.Owner);
        }

        public virtual void OnSpawn() { }

        /// <summary>
        /// 在捕捉器内的AI
        /// </summary>
        public virtual void AI_InCatcher(Rectangle cursor) { }

        public virtual bool ShouldUpdatePosition() => true;

        /// <summary>
        /// 正在捕捉的时候并且玩家未左键或者指针没接触，这是和减少捕捉进度
        /// </summary>
        public virtual void ReduceProgress()
        {
            catchProgress -= 100f / (60 * 20f);
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
                fairyitem.IV = fcp.RollFairyIndividualValues(fairyitem);

            //调用onCatch
            OnCatch(player, i);

            //在玩家处生成物品
            player.QuickSpawnItem(player.GetSource_FairyCatch(this), i);

            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.ItemTransfer,
                new ParticleOrchestraSettings()
                {
                    PositionInWorld = position,
                    MovementVector = player.Center,
                    UniqueInfoPiece = i.type
                });
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

        }

        /// <summary>
        /// 在仙灵瓶里的绘制
        /// </summary>
        public virtual void Draw_InBottle()
        {

        }

        public Texture2D GetTexture() => ModContent.Request<Texture2D>(Texture).Value;
    }
}
