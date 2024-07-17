using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeActiveProducer : MagikeProducer
    {
        /// <summary> 基础发送时间 </summary>
        public int ProductionDelayBase { get; private set; }
        /// <summary> 发送时间减少量（效率增幅量） </summary>
        public float ProductionDelayBonus { get; set; } = 1f;

        /// <summary> 发送时间 </summary>
        public int ProductionDelay
        {
            get => Math.Clamp((int)(ProductionDelayBase * ProductionDelayBonus), 1, int.MaxValue);
        }

        /// <summary> 生产魔能的计时器 </summary>
        private int _produceTimer;

        public bool CanProduce()
        {
            _produceTimer--;
            if (_produceTimer == 0)
            {
                _produceTimer = ProductionDelay;
                return true;
            }

            return false;
        }

        public override void Update(IEntity entity)
        {
            //生产时间限制
            if (!CanProduce())
                return;

        }

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);

            tag.Add(preName + nameof(ProductionDelayBase), ProductionDelayBase);
            tag.Add(preName + nameof(ProductionDelayBonus), ProductionDelayBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);

            ProductionDelayBase = tag.GetInt(preName + nameof(ProductionDelayBase));
            ProductionDelayBonus = tag.GetFloat(preName + nameof(ProductionDelayBonus));
        }
    }
}
