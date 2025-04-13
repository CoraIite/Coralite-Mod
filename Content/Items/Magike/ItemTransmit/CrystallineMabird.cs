using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;

namespace Coralite.Content.Items.Magike.ItemTransmit
{
    public class CrystallineMabird : Mabird
    {
        public override string Texture => AssetDirectory.ItemTransmits + Name;

        public override int SendLength => 16 * 30;
        public override int CatchStack => 5;
        public override float Speed => 4;
        public override short RestTime => 60 * 2;

        public override void SetDefaults()
        {
            Item.maxStack = 1;//不可堆叠

            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }
    }
}
