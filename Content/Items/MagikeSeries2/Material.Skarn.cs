using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class Skarn : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public void AddMagikeCraftRecipe()
        {
            MagikeSystem.AddRemodelRecipe<Basalt, Skarn>(MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 2, 15), 4, conditions: Condition.Hardmode);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SkarnTile>());
        }


    }
}
