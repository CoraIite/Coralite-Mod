using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria;

namespace Coralite.Content.Items.FairyBottle
{
    public class PerfumeBottle : BaseFairyBottle
    {
        public override int FightCapacity => 5;
        public override int ContainCapacity => 20;

        public override void SetDefaults()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 0, 0, 20));
        }
    }
}
