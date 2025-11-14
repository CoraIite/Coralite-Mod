using Coralite.Content.MutiBlocks;
using Coralite.Core;
using Coralite.Core.Systems.MTBStructure;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.DigDigDig
{
    public class StoneMakerCoreTile : ModTile
    {
        public override string Texture => AssetDirectory.DigDigDigTiles + Name;

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
            CoraliteContent.GetMTBS<DigStoneMakerMTB>().CheckStructure(new Point16(i, j));

            return true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            int type = CoraliteContent.MTBSType<DigStoneMakerMTB>();
            foreach (var p in Main.projectile.Where(p => p.active && p.friendly && p.type == ModContent.ProjectileType<PreviewMultiblockPeoj>() && p.ai[0] == type))
                p.Kill();
        }
    }
}
