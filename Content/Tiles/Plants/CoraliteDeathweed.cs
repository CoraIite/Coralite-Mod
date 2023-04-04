using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Plants
{
    public class CoraliteDeathweed : BaseBottomSolidSeedPlantTile<NormalPlantTileEntity>
    {
        public CoraliteDeathweed() : base(AssetDirectory.PlantTiles, 16, 3, ItemID.DeathweedSeeds, ItemID.Deathweed) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.CorruptGrass, TileID.CrimsonGrass }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 18 }, 2, SoundID.Grass, DustID.Grass, Color.Purple, 16, "死亡草", 1, 0);
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
