using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Tools.MiniColumns
{
    public class MiniSplendorColumn : MagikeChargeableItem
    {
        public MiniSplendorColumn() : base(1500, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<SplendorMagicoreRarity>(), 1000, AssetDirectory.MagikeTools)
        {
        }

        public override void SetDefs()
        {
            Item.GetMagikeItem().magikeSendable = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MiniSoulColumn>()
                .AddIngredient<SplendorMagicore>(2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
