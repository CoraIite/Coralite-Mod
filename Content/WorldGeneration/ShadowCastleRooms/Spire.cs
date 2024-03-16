namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class Spire : ShadowCastleRoom
    {
        public override int RandomTypeCount => 2;
        public override Point[] UpCorridor => new Point[]
        {
            new Point(32,17),
            new Point(38,10),
        };
        public override Point[] DownCorridor => throw new System.Exception("你不该使用这个！！！！！！为什么为什么为什么为什么为什么为什么为什么为什么为什么为什么为什么为什么要用它！！！！！");
        public override Point[] LeftCorridor => new Point[]
        {
            new Point(18,26),
            new Point(11,28),
        };
        public override Point[] RightCorridor => new Point[]
        {
            new Point(46,26),
            new Point(50,28),
        };

        public Spire(Point center) : base(center, RoomType.Spire)
        {
        }

        public override void PostGenerateSelf()
        {
            GenerateObject();
        }
    }
}
