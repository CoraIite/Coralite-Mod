using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.Tools.MiniColumns
{
    public class MiniSoulColumn : BaseMagikeChargeableItem
    {
        public MiniSoulColumn() : base(750, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<CrystallineMagikeRarity>(), 600, AssetDirectory.MagikeTools)
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
                .AddIngredient(ItemID.Ectoplasm,5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}