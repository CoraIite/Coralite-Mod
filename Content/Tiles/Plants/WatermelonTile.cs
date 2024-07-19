//using Coralite.Content.Items.Botanical.Seeds;
//using Coralite.Content.Items.Materials;
//using Coralite.Core;
//using Coralite.Core.Prefabs.Tiles;
//using Coralite.Helpers;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ObjectData;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Tiles.Plants
//{
//    public class WatermelonTile : BaseBottomSolidSeedPlantTile<NormalPlantTileEntity>
//    {
//        public WatermelonTile() : base(AssetDirectory.PlantTiles, 30, 4, ItemType<WatermelonSeed>(), ItemType<Watermelon>()) { }

//        public override void SetStaticDefaults()
//        {
//            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.JungleGrass }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 16 }, 2, SoundID.Grass, DustID.Grass, Color.Green, 30, "西瓜", 1, 0);
//        }

//        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
//        {
//            plantItemStack = 1;
//            seedItemStack = Main.rand.Next(3);
//        }

//        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
//        {
//            if (stage == PlantStage.Grown)
//                plantItemStack = 1;

//            seedItemStack = Main.rand.Next(3);
//        }

//        public override bool RightClick(int i, int j)
//        {
//            PlantStage stage = BotanicalHelper.GetPlantStage(i, j, FrameWidth, FrameCount);
//            if (stage == PlantStage.Grown)
//            {
//                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), PlantType);
//                Tile tile = Framing.GetTileSafely(i, j);
//                TileObjectData data = TileObjectData.GetTileData(tile);
//                tile.TileFrameX -= (short)((data.CoordinateWidth + data.CoordinatePadding) * 2);
//            }

//            return true;
//        }
//    }
//}
