using Microsoft.Xna.Framework;

namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class BossRoom : ShadowCastleRoom
    {
        public override Point[] DownCorridor => new Point[]
        {
            new Point(47,128)
        };

        public BossRoom(Point center) : base(center+new Point(0, -64), RoomType.BossRoom)
        {
        }

        public override void PostGenerateSelf()
        {
            GenerateObject();
        }
    }
}
