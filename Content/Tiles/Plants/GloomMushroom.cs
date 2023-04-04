using Coralite.Content.Items.Botanical.Plants;
using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Tiles.Plants
{
    public class GloomMushroom : BaseBottomSolidSeedPlantTile<NormalPlantTileEntity>
    {
        public GloomMushroom() : base(AssetDirectory.PlantTiles, 16, 2, ItemType<GloomSpores>(), ItemType<GloomMushrooms>()) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.MushroomGrass }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 20 }, -2, SoundID.Grass, DustID.Granite, Color.Black, 16, "吸光蘑菇", 1, 0);
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = Main.rand.Next(2);
            seedItemStack = Main.rand.Next(3);
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(4);

            seedItemStack = Main.rand.Next(3);
        }
    }
}
