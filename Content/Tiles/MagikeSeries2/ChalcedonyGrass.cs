using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class ChalcedonyGrass1x1 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 5;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = DustID.Pearlsand;
            AddMapEntry(new Color(217, 255, 185));

            MinPick = 150;
            MineResist = 2;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (!WorldGen.InWorld(i - 1, j + 1) || !WorldGen.InWorld(i + 1, j + 1))
                return true;

            Tile t1 = Framing.GetTileSafely(i - 1, j + 1);
            Tile t2 = Framing.GetTileSafely(i + 1, j + 1);

            Tile t11 = Framing.GetTileSafely(i - 1, j);
            Tile t22 = Framing.GetTileSafely(i + 1, j);

            if (!t1.HasTile && !t1.HasTile)
            {
                Main.tile[i, j].TileFrameX = (short)(Main.rand.Next(2) * 18);
                return false;
            }
            if (!t2.HasTile && !t22.HasTile)
            {
                Main.tile[i, j].TileFrameX = (short)(Main.rand.Next(3, 5) * 18);
                return false;
            }

            return true;
        }
    }
}
