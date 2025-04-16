using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Terraria;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    internal class SkarnCutterTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public const int FrameHeight = 18 * 5;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 18];
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(2, 4);

            AnimationFrameHeight = TileObjectData.newTile.CoordinateFullHeight;

            TileObjectData.addTile(Type);

            DustType = ModContent.DustType<SkarnDust>();
            LocalizedText pylonName = CreateMapEntryName();
            AddMapEntry(Color.White, pylonName);

            RegisterItemDrop(ModContent.ItemType<SkarnCutter>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter > 4)
            {
                frameCounter = 0;
                if (++frame > 9)
                    frame = 0;
            }
        }
    }
}
