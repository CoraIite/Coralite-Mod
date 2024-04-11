using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵的基类，用于在捕捉的时候以及仙灵瓶里的行为
    /// </summary>
    public abstract class Fairy<TItem> where TItem : ModItem
    {
        /// <summary>
        /// 是否存活，在
        /// </summary>
        public bool active;

        /// <summary>
        /// 0-1的捕获进度，到达1则表示捉到
        /// </summary>
        public float catchProgress;

        /// <summary>
        /// 在捕捉器内的行为
        /// </summary>
        public void UpdateInCatcher()
        {
            AI_InCatcher();

        }

        /// <summary>
        /// 用于注册生成方式
        /// </summary>
        public virtual void RegisterSpawn()
        {

        }

        public virtual void AI_InCatcher()
        {

        }

        public virtual void AI_InBottle()
        {

        }
    }
}
