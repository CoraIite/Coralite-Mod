using Coralite.Content.Tiles.ShadowCastle;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class BookRoom : ShadowCastleRoom
    {
        public override Point[] UpCorridor => new Point[]
        {
            new Point(32,5),
        };
        public override Point[] DownCorridor => new Point[]
        {
            new Point(32,64-5),
        };
        public override Point[] LeftCorridor => new Point[]
        {
            new Point(5,38),
        };
        public override Point[] RightCorridor => new Point[]
        {
            new Point(64-5,39),
        };

        public BookRoom(Point center) : base(center, RoomType.Sanctum)
        {
        }

        public override void PostGenerateSelf()
        {
            GenerateObject();

            for (int i = 5; i < Width - 5; i++)
                for (int j = 14; j < Height - 14; j++)
                {
                    Tile t = Main.tile[roomRect.Left + i, roomRect.Top + j];
                    Tile top = Main.tile[roomRect.Left + i, roomRect.Top + j - 1];

                    if (t.HasTile && t.TileType == ModContent.TileType<MercuryPlatformTile>() &&
                        !top.HasTile)
                    {
                        WorldGen.PlaceTile(roomRect.Left + i, roomRect.Top + j - 1, TileID.Books);
                    }
                }

            bool placed = false;

            while (!placed)
            {

                int x = roomRect.Left + WorldGen.genRand.Next(5, Width - 5);
                int y = roomRect.Top + WorldGen.genRand.Next(14, Width - 14);

                Tile t = Main.tile[x, y];

                if (t.HasTile && t.TileType == TileID.Books)
                {
                    Main.tile[x, y].ClearTile();
                    WorldGen.PlaceTile(x, y, ModContent.TileType<ShadowDartTile>());
                    placed = true;
                }
            }

            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(roomRect.Left, roomRect.Top, 5, 14, Width - 5, Height - 14
                , (ushort)ModContent.TileType<MercuryBookcaseTile>(), () => 1, 10);
            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(roomRect.Left, roomRect.Top, 5, 14, Width - 5, Height - 14
                , (ushort)ModContent.TileType<MercuryChairTile>(), () => WorldGen.genRand.NextFromList(-1, 1), 16, style: 0);
            WorldGenHelper.PlaceOnGroundDecorations_NoCheck(roomRect.Left, roomRect.Top, 5, 14, Width - 5, Height - 14
                , (ushort)ModContent.TileType<MercuryTableTile>(), () => 1, 10);

            WorldGenHelper.PlaceOnTopDecorations(roomRect.Left, roomRect.Top, 5, 14, Width - 5, Height - 14
                , (ushort)ModContent.TileType<MercuryDroplight2Tile>(), 20);
            WorldGenHelper.PlaceOnTopDecorations(roomRect.Left, roomRect.Top, 5, 14, Width - 5, Height - 14
                , (ushort)ModContent.TileType<MercuryBigDroplightTile>(), 20);
        }
    }
}
