using Coralite.Content.Items.BotanicalItems.Seeds;
using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Tiles.Plants
{
    public class EgenolfiaSandy : BaseBottomSoildSeedPlantTile<NormalPlantTileEntity>
    {
        public EgenolfiaSandy() : base(AssetDirectory.PlantTiles, 26, 3, ItemType<EgenolfiaBuds>(), ItemType<SandliteDust>()) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.Sand, TileID.Sandstone, TileID.HardenedSand }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 34 }, -18, SoundID.Grass, DustID.Grass, Color.Brown, 26, "刺蕨");
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = Main.rand.Next(4);
            seedItemStack = Main.rand.Next(3);
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(6);

            seedItemStack = Main.rand.Next(3);
        }
    }
}
