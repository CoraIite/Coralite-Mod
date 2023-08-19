using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public abstract class BaseNightmareSparkle:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
        public Vector2 mainSparkleScale;
        public float circleSparkleScale;

        public Color ShineColor;

        public override bool PreDraw(ref Color lightColor)
        {
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + MathHelper.PiOver2;
            //中心的闪光
            Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White * 0.1f, ShineColor * 0.8f,
                0.5f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale, Vector2.One * 2);

            //周围一圈小星星
            for (int i = 0; i < 7; i++)
            {
                Vector2 dir = (Main.GlobalTimeWrappedHourly + i * MathHelper.TwoPi / 7).ToRotationVector2();
                Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos + dir * (24 + factor * 4), Color.White*0.7f, ShineColor,
                    0.5f + factor * 0.1f, 0f, 0.5f, 0.5f, 1f, rot, new Vector2(circleSparkleScale, circleSparkleScale), new Vector2(0.5f, 0.5f));
            }

            //绘制额外旋转的星星，和上面叠起来变成一个
            Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White * 0.3f, Color.Black,
                0.5f - factor * 0.1f, 0f, 0.5f, 0.5f, 1f, Projectile.rotation + MathHelper.PiOver4, new Vector2(mainSparkleScale.Y * 0.75f), Vector2.One);

            //绘制一层小的更加亮的来让星星中心变亮点
            Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White * 0.5f, Color.Black,
                0.5f+factor*0.1f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale * 0.2f, new Vector2(0.5f, 0.5f));

            return false;
        }
    }
}
