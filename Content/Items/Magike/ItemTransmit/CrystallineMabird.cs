using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;

namespace Coralite.Content.Items.Magike.ItemTransmit
{
    public class CrystallineMabird : Mabird, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.ItemTransmits + Name;

        public override int SendLength => 16 * 30;
        public override int CatchStack => 5;
        public override float Speed => 4;
        public override short RestTime => 60 * 2;

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<CrystallineMagike, CrystallineMabird>(MagikeHelper.CalculateMagikeCost<BrilliantLevel>( 4, 45)
                , 4)
                .Register();
        }

        public override void SetDefaults()
        {
            Item.maxStack = 1;//不可堆叠

            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }
    }
}
