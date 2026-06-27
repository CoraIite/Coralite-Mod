using Coralite.Core.Systems.MagikeSystem.Attributes;
using Coralite.Helpers;
using System;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components.Producers
{
    /*
     * 再次稍微探讨一下魔能生产器的大致分类
     * 
     *  - 自动类型：自动生产魔能，需要计时器
     *  - 消耗物品类型：间隔固定时间检测自身物品容器内的物品，需要计时器，本质是上面的那个的扩展
     *  
     *  - 静默启动类型：自身不主动生产，需要外界激发以生产。
     */
    public abstract class MagikeProducer : MagikeComponent
    {
        public sealed override int ID => MagikeComponentID.MagikeProducer;

        /// <summary> 基础生产量 </summary>
        [UpgradeableProp]
        public int ThroughputBase { get; protected set; }
        /// <summary> 额外生产量 </summary>
        public float ThroughputBonus { get; set; } = 1f;

        /// <summary> 生产量 </summary>
        public virtual int Throughput { get => Math.Clamp((int)(ThroughputBase * ThroughputBonus), 0, int.MaxValue); }

        /// <summary>
        /// 是否能生产，同时对应物块悬浮状态
        /// </summary>
        public abstract bool CanProduce();

        /// <summary>
        /// 生产
        /// </summary>
        public virtual void Produce()
        {
            //魔能产出是状态变更，服务端权威；客户端只跑视觉（见 OnProduceVisual）
            if (!VaultUtils.isClient)
                Entity.GetMagikeContainer().AddMagike(Throughput);

            OnProduceVisual();
        }

        /// <summary>
        /// 生产时的视觉表现（粒子等），各端都会运行；不得在其中改变任何权威状态。<br></br>
        /// 第一波把组件 Update 默认服务端权威后，纯客户端不再每帧跑生产逻辑，
        /// 故把视觉从产出逻辑里拆出，并让生产器 <see cref="MagikeActiveProducer.UpdateOnClient"/> 在各端运行以恢复粒子。
        /// </summary>
        public virtual void OnProduceVisual()
        {
            MagikeHelper.SpawnDustOnProduce(Entity.Position, Coralite.MagicCrystalPink);
        }

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(ThroughputBase), ThroughputBase);
            tag.Add(preName + nameof(ThroughputBonus), ThroughputBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            ThroughputBase = tag.GetInt(preName + nameof(ThroughputBase));
            ThroughputBonus = tag.GetFloat(preName + nameof(ThroughputBonus));
        }
    }
}
