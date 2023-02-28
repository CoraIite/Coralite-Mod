using Coralite.Content.Items.BotanicalItems.Seeds;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.Plants
{
    public class EyeballHerb : BaseBottomSoildSeedPlantTile<NormalPlantTileEntity>
    {
        public EyeballHerb() : base(AssetDirectory.PlantTiles, 24, 4, ModContent.ItemType<EyeballSeed>(), ItemID.Lens,ItemID.BlackLens) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.CrimsonGrass, TileID.Crimstone }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 32 }, -14, SoundID.Grass, DustID.CrimsonPlants, Color.DarkRed, 24, "眼球草");
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (Main.rand.NextBool(50))
                rarePlantStack = 1;
            plantItemStack = Main.rand.Next(1, 3);
            seedItemStack = Main.rand.Next(0, 3);
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(1, 4);

            seedItemStack = Main.rand.Next(1, 3);
        }
    }
}
