using Coralite.Content.Items.BotanicalItems.Seeds;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Tiles.Plants
{
    public class TestPlant : BasePlantTileWithSeed<NormalPlantTileEntity>
    {
        public TestPlant() : base(AssetDirectory.PlantTiles, 16, 4, ItemType<TestSeed>(), ItemID.Wood) { }

        public override void SetStaticDefaults()
        {
            (this).PlantPrefab<NormalPlantTileEntity>(new int[] { TileID.Grass }, new int[] { TileID.ClayPot, TileID.PlanterBox },new int[] {16},0, SoundID.Grass, DustID.Grass, Color.Green, 16,"测试植物");
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = 3;
            seedItemStack = 4;
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = 4;
            seedItemStack = 5;
        }
    }
}
