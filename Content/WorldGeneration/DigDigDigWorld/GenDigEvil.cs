using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheEvil { get; set; }

        public static void GenDigEvil(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheEvil.Value;

            ushort evilSand = TileID.Ebonsand;
            ushort evilSandstone = TileID.CorruptSandstone;
            ushort evilHardsand = TileID.CorruptHardenedSand;
            ushort evilGrass = TileID.CorruptGrass;
            ushort evilJungleGrass = TileID.CorruptJungleGrass;
            ushort evilStone = TileID.Ebonstone;
            ushort evilWall = WallID.EbonstoneUnsafe;

            if (WorldGen.crimson)
            {
                evilSand = TileID.Crimsand;
                evilSandstone = TileID.CrimsonSandstone;
                evilHardsand = TileID.CrimsonHardenedSand;
                evilGrass = TileID.CrimsonGrass;
                evilJungleGrass = TileID.CrimsonJungleGrass;
                evilStone = TileID.Crimstone;
                evilWall = WallID.CrimstoneUnsafe;
            }

            var wr = BlurRandom(20);

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int j = Main.maxTilesY - 225;

                for (int k = 0; k < 9; k++)
                {
                    int yoffset = wr.Get();
                    Tile t = Main.tile[i, j - yoffset];

                    t.Clear(Terraria.DataStructures.TileDataType.Wall);
                    WorldGen.PlaceWall(i, j - yoffset, evilWall, true);

                    SetEvil(evilSand, evilSandstone, evilHardsand, evilGrass, evilJungleGrass, evilStone, t);
                }

                for (; j < Main.maxTilesY; j++)
                {
                    Tile t = Main.tile[i, j];

                    t.Clear(Terraria.DataStructures.TileDataType.Wall);
                    WorldGen.PlaceWall(i, j, evilWall, true);

                    SetEvil(evilSand, evilSandstone, evilHardsand, evilGrass, evilJungleGrass, evilStone, t);
                }

                progress.Value += 1f / Main.maxTilesX;
            }
        }

        private static void SetEvil(ushort evilSand, ushort evilSandstone, ushort evilHardsand, ushort evilGrass, ushort evilJungleGrass, ushort evilStone, Tile t)
        {
            if (t.TileType == TileID.Sand)
            {
                t.ResetToType(evilSand);
                return;
            }

            if (t.TileType == TileID.HardenedSand)
            {
                t.ResetToType(evilHardsand);
                return;
            }

            if (t.TileType == TileID.Sandstone)
            {
                t.ResetToType(evilSandstone);
                return;
            }

            if (t.TileType == TileID.Grass)
            {
                t.ResetToType(evilGrass);
                return;
            }

            if (t.TileType == TileID.JungleGrass)
            {
                t.ResetToType(evilJungleGrass);
                return;
            }

            t.ResetToType(evilStone);
        }
    }
}