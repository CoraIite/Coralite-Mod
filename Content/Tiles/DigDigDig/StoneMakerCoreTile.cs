using Coralite.Content.MutiBlocks;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.DigDigDig
{
    public class StoneMakerCoreTile:ModTile
    {
        public override string Texture => AssetDirectory.DigDigDigTiles+Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);

            AddMapEntry(Color.Gray);

            DustType = DustID.Stone;
        }

        public override bool RightClick(int i, int j)
        {
            CoraliteContent.GetMTBS<DigStoneMakerMBS>().CheckStructure(new Point(i, j));

            return true;
        }
    }
}
