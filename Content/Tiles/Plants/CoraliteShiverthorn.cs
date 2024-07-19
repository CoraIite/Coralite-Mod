//using Coralite.Core;
//using Coralite.Core.Prefabs.Tiles;
//using Coralite.Helpers;
//using Terraria;
//using Terraria.ID;

//namespace Coralite.Content.Tiles.Plants
//{
//    public class CoraliteShiverthorn : BaseBottomSolidSeedPlantTile<NormalPlantTileEntity>
//    {
//        public CoraliteShiverthorn() : base(AssetDirectory.PlantTiles, 16, 3, ItemID.MoonglowSeeds, ItemID.Moonglow) { }

//        public override void SetStaticDefaults()
//        {
//            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.IceBlock, TileID.SnowBlock, TileID.CorruptIce, TileID.HallowedIce, TileID.FleshIce }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 18 }, 2, SoundID.Grass, DustID.Grass, Color.AliceBlue, 16, "寒颤棘", 1, 0);
//        }

//        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
//        {
//            plantItemStack = Main.rand.Next(1, 3);
//            seedItemStack = Main.rand.Next(1, 3);
//        }

//        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
//        {
//            if (stage == PlantStage.Grown)
//                plantItemStack = Main.rand.Next(1, 4);

//            seedItemStack = Main.rand.Next(1, 3);
//        }
//    }
//}
