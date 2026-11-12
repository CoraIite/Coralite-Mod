using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// 使用ai0记录样式,0蓝移，1红移<br></br>
    /// 弹幕时间固定30+80+30
    /// </summary>
    public class ShiftStar : CoraliteBossHostileProj
    {
        public override string Texture => AssetDirectory.Blank;

        private ref float Timer => ref Projectile.localAI[1];

        /// <summary>
        /// 样式
        /// </summary>
        private ref float Style => ref Projectile.ai[0];

        public int TrailCount = 22;

        public float alpha = 0;

        public Vector2 recordeCenter;
        public float recordDistance;


        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 500;
        }

        public override void Initialize()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.InitOldRotCache(TrailCount);
            Projectile.InitOldPosCache(TrailCount);
        }

        public override void AI()
        {
            if (Timer < 30)
                alpha = Helper.BezierEase(Timer / 30f);
            else if (Timer < 30 + 60)
                alpha = 1;
            else if (Timer < 30 + 60 + 30)
            {
                alpha = 1 - Helper.BezierEase((Timer - 30 - 60) / 30f);
                Projectile.velocity = Projectile.velocity.RotatedBy(0.03f);
            }
            else
                Projectile.Kill();

            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.UpdateOldPosCache();
            Projectile.UpdateOldRotCache();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.oldPos.Length != TrailCount)
            {
                return false;
            }

            Color trailColor = Color.Silver * alpha;
            Color c2 ;

            float length = 1 + 0.4f;
            float scale = 1 + 0.5f;

            Color ShineColor;
            Color DarkColor;

            if (Style == 0)//蓝移
            {
                c2 = Color.CornflowerBlue * alpha;
                ShineColor = new Color(65, 155, 242) * alpha;
                DarkColor = new Color(26, 23, 130) * alpha;
            }
            else//红移
            {
                c2 = new Color(247, 74, 74) * alpha;
                ShineColor = new Color(255, 120, 145) * alpha;
                DarkColor = new Color(135, 8, 31) * alpha;
            }

            for (int i = 0; i < TrailCount-1; i++)//绘制拖尾
            {
                float f = 1 - i / (float)TrailCount;
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition;
                Helper.DrawPrettyLine(1 - f, 0, pos, Color.Lerp(ShineColor, DarkColor, f), trailColor, f, 0, 0.01f, 0.4f, 1, Projectile.oldRot[i], scale, new Vector2(length, 1));
            }

            Helper.DrawPrettyStarSparkle(1, 0, Projectile.Center - Main.screenPosition, c2 with { A = 0 }, c2, 0.5f, 0, 0.5f, 0.5f, 1, 0, Vector2.One * 1.3f, Vector2.One * 0.8f);

            Helper.DrawPrettyStarSparkle(1, 0, Projectile.Center - Main.screenPosition, c2 with { A = 0 }, c2, 0.5f, 0, 0.5f, 0.5f, 1, MathHelper.PiOver4, Vector2.One*0.7f, Vector2.One * 0.8f);

            return false;
        }
    }
}
