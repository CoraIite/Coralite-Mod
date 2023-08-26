using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareSparkle_Normal : BaseNightmareSparkle
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 1200;

            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 48;
            ShineColor = NightmarePlantera.nightmareSparkleColor;
            mainSparkleScale = new Vector2(1.5f, 3f);
            circleSparkleScale = 0.5f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            NightmarePlantera.NightmareHit(target);
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < 24)
            {
                Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.3f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                Dust dust = Dust.NewDustPerfect(Projectile.Center+Main.rand.NextVector2Circular(12,12) , ModContent.DustType<NightmareStar>(),
                    dir * Main.rand.NextFloat(1f, 4f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 3f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
            }
        }
    }
}
