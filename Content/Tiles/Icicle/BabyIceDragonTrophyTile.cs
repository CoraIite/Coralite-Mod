using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.Icicle
{
    public class BabyIceDragonTrophyTile:ModTile
    {
        public override string Texture => AssetDirectory.IcicleTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
            DustType = DustID.WoodFurniture;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<BabyIceDragonTrophy>());
        }

    }
}