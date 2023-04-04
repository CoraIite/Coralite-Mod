using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Plants
{
    public class CoraliteFireblossom : BaseBottomSolidSeedPlantTile<NormalPlantTileEntity>
    {
        public CoraliteFireblossom() : base(AssetDirectory.PlantTiles, 16, 3, ItemID.FireblossomSeeds, ItemID.Fireblossom) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.JungleGrass }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 20 }, 2, SoundID.Grass, DustID.Grass, Color.OrangeRed, 16, "火焰花", 1, 0);
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
