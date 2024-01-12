using Microsoft.Xna.Framework;

namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class Spire : ShadowCastleRoom
    {
        public override Point[] UpCorridor => new Point[]
        {
            new Point(32,17),
        };
        public override Point[] DownCorridor => throw new System.Exception("你不该使用这个！！！！！！为什么为什么为什么为什么为什么为什么为什么为什么为什么为什么为什么为什么要用它！！！！！");
        public override Point[] LeftCorridor => new Point[]
        {
            new Point(18,26),
        };
        public override Point[] RightCorridor => new Point[]
        {
            new Point(46,26),
        };

        public Spire(Point center) : base(center, RoomType.Spire)
        {
        }
    }
}
