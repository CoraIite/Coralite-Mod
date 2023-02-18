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
    public class NacliteMoss : BaseTopSoildSeedlingPlantTile<NormalPlantTileEntity>
    {
        public NacliteMoss() : base(AssetDirectory.PlantTiles, 16, 2, ItemType<NacliteSeedling>(), ItemType<Naclite>()) { }

        public override void SetStaticDefaults()
        {
            (this).SoildTopPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.HardenedSand }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 22 }, -2, SoundID.Grass, DustID.Grass, Color.Green, 16, "盐晶苔藓", 1, 0);
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
