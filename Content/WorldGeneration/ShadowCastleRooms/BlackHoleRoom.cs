using Coralite.Content.Items.Shadow;
using Coralite.Content.Items.ShadowCastle;
using Coralite.Content.Tiles.ShadowCastle;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class BlackHoleRoom : ShadowCastleRoom
    {
        public override Point[] UpCorridor => new Point[]
        {
            new(32,6),
        };
        public override Point[] DownCorridor => new Point[]
        {
            new(32,64-6),
        };
        public override Point[] LeftCorridor => new Point[]
        {
            new(6,32),
        };
        public override Point[] RightCorridor => new Point[]
        {
            new(64-6,32),
        };

        public BlackHoleRoom(Point center) : base(center, RoomType.BlackHoleRoom)
        {
        }

        public override void PostGenerateSelf()
        {
            GenerateObject();
            int index = WorldGen.PlaceChest(roomRect.X + 31, roomRect.Y + 32, (ushort)ModContent.TileType<BlackHoleChestTile>());
            if (index >= 0)
            {
                Chest chest = Main.chest[index];
                ChestRoom.RandChestItem(chest, ModContent.ItemType<ShadowCrystal>());

                //放影水晶
                if (WorldGen.genRand.NextBool(1, 3))
                    ChestRoom.RandChestItem(chest, ModContent.ItemType<ShadowCrystal>()
                        , WorldGen.genRand.Next(1, 5));

                //放马内，放一大堆的Money
                for (int i = 0; i < 4; i++)
                    if (WorldGen.genRand.NextBool(1, 2))
                        ChestRoom.RandChestItem(chest, ItemID.GoldCoin
                            , WorldGen.genRand.Next(1, 2));
                for (int i = 0; i < 5; i++)
                    if (WorldGen.genRand.NextBool(1, 3))
                        ChestRoom.RandChestItem(chest, ItemID.SilverCoin
                            , WorldGen.genRand.Next(1, 20));
                for (int i = 0; i < 8; i++)
                    if (WorldGen.genRand.NextBool(1, 3))
                        ChestRoom.RandChestItem(chest, ItemID.SilverCoin
                            , WorldGen.genRand.Next(1, 90));
            }
        }
    }
}
