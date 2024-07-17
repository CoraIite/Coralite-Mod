﻿using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Helpers;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    /*
     * 再次稍微探讨一下魔能生产器的大致分类
     * 
     *  - 自动类型：自动生产魔能，需要计时器
     *  - 消耗物品类型：间隔固定时间检测自身物品容器内的物品，需要计时器，本质是上面的那个的扩展
     *  
     *  - 静默启动类型：自身不主动生产，需要外界激发以生产。
     */
    public abstract class MagikeProducer : Component
    {
        public sealed override int ID => MagikeComponentID.MagikeProducer;

        /// <summary> 基础生产量 </summary>
        public int ThroughputBase {  get; private set; }
        /// <summary> 额外生产量 </summary>
        public int ThroughputExtra {  get; set; }

        /// <summary> 生产量 </summary>
        public int Throughput { get => ThroughputBase + ThroughputExtra; }

        /// <summary>
        /// 生产
        /// </summary>
        public void Produce()
        {
            Entity.GetMagikeContainer().AddMagike(ThroughputBase);
        }

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(ThroughputBase), ThroughputBase);
            tag.Add(preName + nameof(ThroughputExtra), ThroughputExtra);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            ThroughputBase = tag.GetInt(preName + nameof(ThroughputBase));
            ThroughputExtra = tag.GetInt(preName + nameof(ThroughputExtra));
        }
    }
}
