using Coralite.Content.Raritys;
using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagicCrystalBrickWall : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Walls.Magike.MagicCrystalBrickWall>());
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<MagicCrystalBrick>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
