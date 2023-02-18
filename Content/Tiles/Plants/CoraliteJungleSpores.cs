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
    public class CoraliteJungleSpores : BaseBottomSoildSeedPlantTile<NormalPlantTileEntity>
    {
        public CoraliteJungleSpores() : base(AssetDirectory.PlantTiles, 16, 3, ItemType<JungleBuds>(), ItemID.JungleSpores) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.JungleGrass }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 20 }, -2, SoundID.Grass, DustID.JungleGrass, Color.Green, 16, "丛林孢子", 1, 0, true);
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = Main.rand.Next(2, 4);
            seedItemStack = Main.rand.Next(0, 2);
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(3, 5);

            seedItemStack = Main.rand.Next(1, 3);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {

        }
    }
}
