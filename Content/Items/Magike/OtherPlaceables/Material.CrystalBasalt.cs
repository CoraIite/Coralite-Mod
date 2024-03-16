using Coralite.Content.Raritys;
using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class CrystalBasalt : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystalBasaltTile>());
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>()
                .AddIngredient<Basalt>()
                .AddTile(TileID.HeavyWorkBench)
                .AddCondition(Condition.InGraveyard)
                .Register();
        }

    }
}
