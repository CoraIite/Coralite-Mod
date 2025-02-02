using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Tools.MiniColumns
{
    public class MiniSoulColumn : MagikeChargeableItem
    {
        public MiniSoulColumn() : base(6000, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<CrystallineMagikeRarity>(), -1, AssetDirectory.MagikeTools)
        {
        }

        public override void SetDefs()
        {
            Item.GetMagikeItem().magikeSendable = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MiniBrilliantColumn>()
                .AddIngredient(ItemID.Ectoplasm, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}