using Coralite.Core.Loaders;
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

        public override string Texture => AssetDirectory.Particles + Name;

        /// <summary>
        /// 是否存活，在捕捉器内使用
        /// </summary>
        public bool active;

        /// <summary>
        /// 0-1的捕获进度，到达1则表示捉到
        /// </summary>
        public float catchProgress;

        protected sealed override void Register()
        {
            ModTypeLookup<Fairy>.Register(this);

            FairyLoader.fairys ??= new List<Fairy>();
            FairyLoader.fairys.Add(this);

            Type = FairyLoader.ReserveParticleID();
        }

        public virtual Fairy NewInstance()
        {
            var inst = (Fairy)Activator.CreateInstance(GetType(), true)!;
            return inst;
        }

        /// <summary>
        /// 在捕捉器内的行为
        /// </summary>
        public void UpdateInCatcher()
        {
            AI_InCatcher();

        }

        public abstract int GetFairyItemType();

        /// <summary>
        /// 用于注册生成方式，详细参考<see cref="fas"/>
        /// </summary>
        public virtual void RegisterSpawn()
        {

        }

        /// <summary>
        /// 被捕获时执行
        /// </summary>
        public void Catch(Player player)
        {
            //new一个物品出来

            //为物品的字段赋值，如果这个物品不是一个仙灵那么就跳过

            //调用onCatch


            //在玩家处生成物品

        }

        /// <summary>
        /// 在捕获时调用
        /// </summary>
        /// <param name="player"></param>
        public virtual void OnCatch(Player player,Item fairyItem) { }

        /// <summary>
        /// 在捕捉器内的AI
        /// </summary>
        public virtual void AI_InCatcher()
        {

        }

        /// <summary>
        /// 在仙灵瓶物块中的AI
        /// </summary>
        /// <param name="limit"></param>
        public virtual void AI_InBottle(Rectangle limit)
        {

        }

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
    }
}
