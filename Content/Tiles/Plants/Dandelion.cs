﻿//using Coralite.Content.Items.Botanical.Seeds;
//using Coralite.Core;
//using Coralite.Core.Prefabs.Tiles;
//using Coralite.Helpers;
//using Terraria;
//using Terraria.ID;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Tiles.Plants
//{
//    public class Dandelion : BaseBottomSolidSeedPlantTile<NormalPlantTileEntity>
//    {
//        public Dandelion() : base(AssetDirectory.PlantTiles, 32, 3, ItemType<DandelionSeed>(), ItemID.PaperAirplaneA) { }

//        public override void SetStaticDefaults()
//        {
//            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.Grass }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 44 }, -26, SoundID.Grass, DustID.Grass, Color.Green, 32, "不愤怒的蒲公英", 3, 0);
//        }

//        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
//        {
//            plantItemStack = Main.rand.Next(2);
//            seedItemStack = 1;
//        }

//        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
//        {
//            if (stage == PlantStage.Grown)
//                plantItemStack = Main.rand.Next(0, 3);

//            seedItemStack = Main.rand.Next(2);
//        }
//    }
//}
