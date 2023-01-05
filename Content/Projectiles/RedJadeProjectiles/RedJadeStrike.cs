using Coralite.Content.Dusts;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.RedJadeProjectiles
{
    public  class RedJadeStrike:ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles+Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("赤玉刺");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0)//用于同步输入的ai0，这个ai0是用于控制弹幕是否能爆炸的
            {
                Projectile.ai[1] = 1;
                if (Projectile.ai[0] == 0)
                    Projectile.scale = 1.5f;

                Projectile.netUpdate = true;
            }

            if (Projectile.velocity.Y < 14)
                Projectile.velocity.Y += 0.08f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.GemRuby, -Projectile.velocity * 0.4f,0,default,0.7f);
                    dust.noGravity = true;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeExplore>(), (int)(Projectile.damage*0.8f), Projectile.knockBack, Projectile.owner);
                SoundEngine.PlaySound(SoundID.DD2_GoblinBomb, Projectile.Center);
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
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Main.rand.NextVector2Circular(3, 3), 0, default, Main.rand.NextFloat(1f, 1.3f));
                    dust.noGravity = true;
                }
            }
        }
    }

    public class RedJadeExplore:ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles+"Blank48x48";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 5;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool PreAI()
        {
            return false;
        }
    }
}
