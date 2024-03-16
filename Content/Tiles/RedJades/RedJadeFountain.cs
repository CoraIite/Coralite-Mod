using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.RedJades
{
    public class RedJadeFountain : ModTile
    {
        public override string Texture => AssetDirectory.RedJadeTiles + "RedJadeFountainTile";

        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;

            DustType = DustID.GemRuby;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.RedJadeRed);
        }

        public override void HitWire(int i, int j)
        {
            //if (Main.rand.NextBool(4))
            //{
            Tile tile = Framing.GetTileSafely(i, j);
            int xOff = tile.TileFrameX / 18;
            int yOff = tile.TileFrameY / 18;

            Vector2 topLeft = new Vector2(i - xOff, j - yOff) * 16;
            topLeft += new Vector2(16 + 8, 8);
            Dust.NewDustPerfect(topLeft, DustID.GemRuby,
                (-1.57f + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2() * Main.rand.NextFloat(2f, 4f));
            //}
        }
    }
}
