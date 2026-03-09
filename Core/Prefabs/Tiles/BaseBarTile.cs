using Terraria;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Core.Prefabs.Tiles
{
    public abstract class BaseBarTile(string texturePath, bool pathHasName = false) : ModTile
    {
        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            DustType = GetDustType();

            AddMapEntry(GetMapColor(), Language.GetText("MapObject.MetalBar")); // localized text for "Metal Bar"
        }

        public abstract Color GetMapColor();
        public abstract int GetDustType();

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
