using Coralite.Content.Items.BotanicalItems.Seeds;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Tiles.Plants
{
    public class WaterDrinker : BaseBottomSoildSeedPlantTile<NormalPlantTileEntity>
    {
        public WaterDrinker() : base(AssetDirectory.PlantTiles, 16, 4, ItemType<WaterDrinkerSeed>(), ItemID.WetBomb) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.Sand, TileID.Sandstone }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 26 }, -6, SoundID.Grass, DustID.WoodFurniture, Color.Brown, 16, "饮水棘", 1, 0);
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = Main.rand.Next(2);
            seedItemStack = 1;
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(0, 3);

            seedItemStack = Main.rand.Next(2);
        }
    }
}
