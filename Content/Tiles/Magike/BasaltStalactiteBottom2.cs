using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.Magike
{
    public class BasaltStalactiteBottom2 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleWrapLimit = 3;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = DustID.CorruptionThorns;
            AddMapEntry(new Microsoft.Xna.Framework.Color(31, 31, 50));
        }

    }
}
