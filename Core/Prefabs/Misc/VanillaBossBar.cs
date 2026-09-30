namespace Coralite.Core.Prefabs.Misc
{
    public class VanillaBossBar : BaseBossHealthBar
    {
        public override string Texture => AssetDirectory.Bosses + "UI_BossBar";

        public override int FrameCount => 6;

        public override Point BackgroundTopLeftOffset => new Point(30, 22);
        public override Point BarSize => new Point(456, 22);

        public override Color DontTakeDamageColor => Color.DarkGray * 0.4f;
        public override Color BarColor =>new Color(200, 80, 80, 40);
        public override Color BackgroundColor => Color.White * 0.5f;

        //public override int HealthBarFrameWidth => 1;

        public override Vector2 IconOffset => new(4, 20);

        public override int BarFrameY => 2;
        public override int BackFrameY => 1;
    }
}
