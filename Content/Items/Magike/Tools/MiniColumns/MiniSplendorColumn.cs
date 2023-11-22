using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.Tools.MiniColumns
{
    public class MiniSplendorColumn : BaseMagikeChargeableItem
    {
        public MiniSplendorColumn() : base(1500, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<SplendorMagicoreRarity>(), 1000, AssetDirectory.MagikeTools)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MiniSoulColumn>()
                .AddIngredient<SplendorMagicore>( 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
