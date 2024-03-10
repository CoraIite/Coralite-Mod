using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderveinDragonTrophyTile : ModTile
    {
        public override string Texture => AssetDirectory.ThunderTiles + Name;

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
    }
}