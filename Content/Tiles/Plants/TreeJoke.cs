//using Coralite.Content.Items.Botanical.Plants;
//using Coralite.Content.Items.Botanical.Seeds;
//using Coralite.Core;
//using Coralite.Core.Prefabs.Tiles;
//using Coralite.Helpers;
//using Terraria;
//using Terraria.ID;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Tiles.Plants
//{
//    public class TreeJoke : BaseBottomSolidSeedPlantTile<NormalPlantTileEntity>
//    {
//        public TreeJoke() : base(AssetDirectory.PlantTiles, 16, 2, ItemType<TreeJokeSeed>(), ItemType<WoodStick>(), ItemID.SlimeStaff) { }

//        public override void SetStaticDefaults()
//        {
//            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.Grass }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 28 }, -10, SoundID.Grass, DustID.WoodFurniture, Color.Brown, 16, "树生树树果", 1, 0);
//        }

//        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
//        {
//            if (Main.rand.NextBool(2000))
//                rarePlantStack = 1;

//            plantItemStack = Main.rand.Next(5, 15);
//            seedItemStack = Main.rand.Next(3);
//        }

//        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
//        {
//            if (Main.rand.NextBool(1500))
//                rarePlantStack = 1;

//            if (stage == PlantStage.Grown)
//                plantItemStack = Main.rand.Next(5, 20);

//            seedItemStack = Main.rand.Next(4);
//        }
//    }
//}
