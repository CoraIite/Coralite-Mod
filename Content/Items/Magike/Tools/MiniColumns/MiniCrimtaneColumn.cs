using Coralite.Content.Items.Glistent;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Tools.MiniColumns
{
    public class MiniCrimtaneColumn : MagikeChargeableItem
    {
        public MiniCrimtaneColumn() : base(1500, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<MagicCrystalRarity>(), -1, AssetDirectory.MagikeTools)
        {
        }

        public override void SetDefs()
        {
            Item.GetMagikeItem().magikeSendable = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MiniCrystalColumn>()
                .AddIngredient<GlistentBar>(2)
                .AddIngredient(ItemID.TissueSample, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
