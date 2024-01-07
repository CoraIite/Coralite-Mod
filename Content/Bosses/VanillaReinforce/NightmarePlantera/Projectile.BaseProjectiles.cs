using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public abstract class BaseNightmareSparkle : BaseNightmareProj
    {
        public override string Texture => AssetDirectory.Blank;
        public Vector2 mainSparkleScale;
        public float circleSparkleScale;

        public static Asset<Texture2D> MainLight;
        public static Asset<Texture2D> Flow;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                MainLight = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light");
                Flow = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Flow");
            }
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                MainLight = null;
                Flow = null;
            }   
        }

        public Color ShineColor;

        //public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        //{
        //    Vector2 dir = Projectile.rotation.ToRotationVector2() * Projectile.width;
        //    return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - dir, Projectile.Center + dir);
        //}

        public override bool PreDraw(ref Color lightColor)
        {
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + MathHelper.PiOver2;
            //中心的闪光

            Texture2D lightTex = MainLight.Value;
            var origin = lightTex.Size() / 2;

            Color c = NightmarePlantera.lightPurple;
            c.A = 0;
            var scale = mainSparkleScale * 0.15f;
            Main.spriteBatch.Draw(lightTex, pos, null, c, Projectile.rotation + 1.57f, origin, scale, 0, 0);
            Main.spriteBatch.Draw(lightTex, pos, null, c, Projectile.rotation + 1.57f, origin, scale, 0, 0);

            //Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, NightmarePlantera.lightPurple, NightmarePlantera.lightPurple,
            //    0.5f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale, Vector2.One * 2);
            Texture2D flowTex = Flow.Value;
            origin = flowTex.Size() / 2;

            Color shineC = ShineColor * 0.75f;
            ShineColor.A = 0;

            var scale2 = scale.X * 0.5f;
            Main.spriteBatch.Draw(flowTex, pos, null, shineC, Projectile.rotation + 1.57f + Main.GlobalTimeWrappedHourly, origin, scale2, 0, 0);
            Main.spriteBatch.Draw(flowTex, pos, null, c * 0.5f, Projectile.rotation - Main.GlobalTimeWrappedHourly, origin, scale2, 0, 0);

            //周围一圈小星星
            scale2 = circleSparkleScale * 0.2f;
            for (int i = 0; i < 7; i++)
            {
                float rot2 = (Main.GlobalTimeWrappedHourly * 2 + i * MathHelper.TwoPi / 7);
                Vector2 dir =rot2.ToRotationVector2();
                dir = pos + dir * (18 + factor * 2);
                rot2 += 1.57f;
                Main.spriteBatch.Draw(lightTex, dir, null, shineC, rot2, origin, scale2, 0, 0);
                //Main.spriteBatch.Draw(lightTex, dir, null, shineC*0.5f, rot2, origin, scale2, 0, 0);
                //Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos + dir * (18 + factor * 2), Color.White, ShineColor,
                //    0.5f + factor * 0.1f, 0f, 0.5f, 0.5f, 1f, rot, new Vector2(circleSparkleScale, circleSparkleScale), new Vector2(0.5f, 0.5f));
            }

            ////绘制额外旋转的星星，和上面叠起来变成一个
            //Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White * 0.3f, ShineColor * 0.5f,
            //    0.5f - factor * 0.1f, 0f, 0.5f, 0.5f, 1f, Projectile.rotation + MathHelper.PiOver4, new Vector2(mainSparkleScale.Y * 0.75f), Vector2.One);

            ////绘制一层小的更加亮的来让星星中心变亮点
            //Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White * 0.7f, Color.White * 0.4f,
            //    0.5f + factor * 0.1f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale * 0.5f, Vector2.One * 2);

            return false;
        }
    }

    public abstract class BaseNightmareProj : ModProjectile
    {
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            NightmarePlantera.NightmareHit(target);
        }
    }

}
