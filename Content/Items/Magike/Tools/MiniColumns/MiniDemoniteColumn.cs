using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.Tools.MiniColumns
{
    public class MiniDemoniteColumn : BaseMagikeChargeableItem
    {
        public MiniDemoniteColumn() : base(200, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeTools)
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
                .AddIngredient(ItemID.ShadowScale,5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}

