using Coralite.Content.Tiles.ShadowCastle;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class DungeonArtifactRoom : ShadowCastleRoom
    {
        public override Point[] UpCorridor => new Point[]
{
            new Point(32,9),
};
        public override Point[] DownCorridor => new Point[]
        {
            new Point(32,64-9),
        };
        public override Point[] LeftCorridor => new Point[]
        {
            new Point(10,32),
        };
        public override Point[] RightCorridor => new Point[]
        {
            new Point(64-9,32),
        };

        public DungeonArtifactRoom(Point center) : base(center, RoomType.DenguonChest)
        {
        }

        public override void PostGenerateSelf()
        {
            GenerateObject();

            ushort chestTileType = 21;

            Point p = roomRect.TopLeft().ToPoint() + new Point(18, 32);
            int style2 = 23;
            int contain = ItemID.PiranhaGun;
            //丛林
            WorldGen.AddBuriedChest(p.X, p.Y, contain, notNearOtherChests: false, style2, trySlope: false, chestTileType);

            p = roomRect.TopLeft().ToPoint() + new Point(25, 24);
            style2 = 24;
            contain = ItemID.ScourgeoftheCorruptor;
            //腐化
            WorldGen.AddBuriedChest(p.X, p.Y, contain, notNearOtherChests: false, style2, trySlope: false, chestTileType);

            p = roomRect.TopLeft().ToPoint() + new Point(39, 24);
            style2 = 25;
            contain = ItemID.VampireKnives;
            //血腥
            WorldGen.AddBuriedChest(p.X, p.Y, contain, notNearOtherChests: false, style2, trySlope: false, chestTileType);

            p = roomRect.TopLeft().ToPoint() + new Point(46, 32);
            style2 = 26;
            contain = ItemID.RainbowGun;
            //神圣
            WorldGen.AddBuriedChest(p.X, p.Y, contain, notNearOtherChests: false, style2, trySlope: false, chestTileType);

            p = roomRect.TopLeft().ToPoint() + new Point(25, 41);
            style2 = 27;
            contain = ItemID.StaffoftheFrostHydra;
            //冰雪
            WorldGen.AddBuriedChest(p.X, p.Y, contain, notNearOtherChests: false, style2, trySlope: false, chestTileType);

            chestTileType = 467;
            p = roomRect.TopLeft().ToPoint() + new Point(39, 41);
            style2 = 13;
            contain = ItemID.StormTigerStaff;
            //沙漠
            WorldGen.AddBuriedChest(p.X, p.Y, contain, notNearOtherChests: false, style2, trySlope: false, chestTileType);

            WorldGenHelper.PlaceOnTopDecorations(roomRect.Left, roomRect.Top, 5, 14, Width - 5, Height - 14
                , (ushort)ModContent.TileType<MercuryDroplight2Tile>(), 20);
            WorldGenHelper.PlaceOnTopDecorations(roomRect.Left, roomRect.Top, 5, 14, Width - 5, Height - 14
                , (ushort)ModContent.TileType<MercuryBigDroplightTile>(), 20);
        }
    }
}
