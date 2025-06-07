using Coralite.Content.Bosses.ModReinforce.PurpleVolt;
using Coralite.Core;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.Thunder
{
    public class ZacurrentRelicTile : ModTile
    {
        public override string Texture => AssetDirectory.ThunderTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(1, 4);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.addTile(Type);

            DustType = DustID.PurpleTorch;
            AddMapEntry(ZacurrentDragon.ZacurrentPurple);
        }
    }
}
