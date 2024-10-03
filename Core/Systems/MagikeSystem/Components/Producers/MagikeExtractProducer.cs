using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components.Producers
{
    public class MagikeExtractProducer : MagikeCostItemProducer
    {
        public override string GetCanProduceText => MagikeSystem.GetUIText(MagikeSystem.UITextID.ItemWithMagike);

        public override MagikeSystem.UITextID NameText { get => MagikeSystem.UITextID.ExtractProducerName; }

        public override bool CanConsumeItem(Item item)
            => item.GetGlobalItem<MagikeItem>().magikeAmount > 0;

        public override int GetMagikeAmount(Item item)
            => item.GetGlobalItem<MagikeItem>().magikeAmount;

        public override void SaveData(string preName, TagCompound tag)
        {
            //base.SaveData(preName, tag);//不需要存取基类里的生产量

            tag.Add(preName + nameof(Timer), Timer);

            tag.Add(preName + nameof(ProductionDelayBase), ProductionDelayBase);
            tag.Add(preName + nameof(ProductionDelayBonus), ProductionDelayBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            //base.LoadData(preName, tag);//不需要存取基类里的生产量

            Timer = tag.GetInt(preName + nameof(Timer));

            ProductionDelayBase = tag.GetInt(preName + nameof(ProductionDelayBase));
            ProductionDelayBonus = tag.GetFloat(preName + nameof(ProductionDelayBonus));
        }
    }
}
