using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.RedJadeProjectiles
{
    public class RedJadeBeam : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank48x48";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0)//用于同步输入的ai0，这个ai0是用于控制弹幕是否能爆炸的
            {
                Projectile.ai[1] = 1;
                if (Projectile.ai[0] == 0)
                    Projectile.scale = 1.6f;

                Projectile.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, Projectile.scale);
                    dust.noGravity = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
                Helper.PlayPitched("RedJade/RedJadeBoom", 0.4f, 0f, Projectile.Center);
                Color red = new Color(221, 50, 50);
                Color grey = new Color(91, 93, 102);
                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(5, 5), 0, red, Main.rand.NextFloat(1f, 1.5f));
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(3, 3), 0, grey, Main.rand.NextFloat(0.8f, 1.2f));
                }
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
                for (int i = 0; i < 12; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Main.rand.NextVector2Circular(4, 4), 0, default, Main.rand.NextFloat(1f, 1.3f));
                    dust.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
