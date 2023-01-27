using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Plants
{
    public class CoraliteBlinkroot : BaseBottomSoildSeedPlantTile<NormalPlantTileEntity>
    {
        public CoraliteBlinkroot() : base(AssetDirectory.PlantTiles, 16, 3, ItemID.BlinkrootSeeds, ItemID.Blinkroot) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.Dirt,TileID.Mud }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 20 }, 2, SoundID.Grass, DustID.Grass, Color.Brown, 16, "闪耀根", 1, 0);
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = Main.rand.Next(1, 3);
            seedItemStack = Main.rand.Next(1, 3);
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(1, 4);

            seedItemStack = Main.rand.Next(1, 3);
        }
    }
}
