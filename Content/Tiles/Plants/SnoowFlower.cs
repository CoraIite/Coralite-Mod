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
    public class SnoowFlower : BaseBottomSoildSeedPlantTile<NormalPlantTileEntity>
    {
        public SnoowFlower() : base(AssetDirectory.PlantTiles, 28, 6, ItemType<SnoowSeed>(), ItemID.SnowBlock) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.SnowBlock }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 30 }, -12, SoundID.Grass, DustID.SnowBlock, Color.White, 28, "雪融花");
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = Main.rand.Next(5, 15);
            seedItemStack = Main.rand.Next(3);
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(5, 20);

            seedItemStack = Main.rand.Next(4);
        }
    }
}
