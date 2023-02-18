using Coralite.Content.Items.BotanicalItems.Plants;
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
    public class PileaNotata : BaseBottomSoildSeedlingPlantTile<NormalPlantTileEntity>
    {
        public PileaNotata() : base(AssetDirectory.PlantTiles, 28, 3, ItemType<PileaNotataSeedling>(), ItemType<PileaNotatata>()) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.SnowBlock, TileID.IceBlock }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 36 }, -20, SoundID.Grass, DustID.Grass, Color.Green, 28, "冷水花", 1, 0);
        }

        public override void DropItemNormally(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedlingItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(2);
            seedlingItemStack = Main.rand.Next(2);
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedlingItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(3);

            seedlingItemStack = Main.rand.Next(3);
        }
    }
}
