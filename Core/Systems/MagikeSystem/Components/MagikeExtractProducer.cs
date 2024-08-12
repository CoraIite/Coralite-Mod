using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeExtractProducer : MagikeCostItemProducer
    {
        public override bool CanConsumeItem(Item item)
            => item.GetGlobalItem<MagikeItem>().magikeAmount > 0;

        public override int GetMagikeAmount(Item item)
            => item.GetGlobalItem<MagikeItem>().magikeAmount;
    }
}
