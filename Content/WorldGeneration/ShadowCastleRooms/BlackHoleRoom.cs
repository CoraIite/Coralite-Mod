using Coralite.Content.Tiles.ShadowCastle;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class BlackHoleRoom : ShadowCastleRoom
    {
        public override Point[] UpCorridor => new Point[]
        {
            new Point(32,6),
        };
        public override Point[] DownCorridor => new Point[]
        {
            new Point(32,64-6),
        };
        public override Point[] LeftCorridor => new Point[]
        {
            new Point(6,32),
        };
        public override Point[] RightCorridor => new Point[]
        {
            new Point(64-6,32),
        };

        public BlackHoleRoom(Point center) : base(center, RoomType.BlackHoleRoom)
        {
        }

        public override void PostGenerateSelf()
        {
            GenerateObject();
          int index=  WorldGen.PlaceChest(roomRect.X + 31, roomRect.Y + 32, (ushort)ModContent.TileType<BlackHoleChestTile>());
            if (index>=0)
            {
                Chest chest = Main.chest[index];
                int itemIndex = 0;
                chest.item[itemIndex].SetDefaults(ItemID.Zenith);
                itemIndex++;
            }
        }
    }
}
