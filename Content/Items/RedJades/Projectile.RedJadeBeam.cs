using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    /// <summary>
    /// 使用ai[0]来控制是否能产生大爆炸，为1能大爆炸
    /// </summary>
    public class RedJadeBeam : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

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
            if (Projectile.localAI[0] == 0)
            {
                Vector2 center = Projectile.Center;
                if (Projectile.ai[0] == 1)
                {
                    Projectile.scale = 1.6f;
                    Projectile.localAI[1] = 588;
                }
                else
                {
                    Projectile.localAI[1] = 596;
                }

                Projectile.Center = center;
                Projectile.localAI[0] = 1;
            }
            if (Projectile.timeLeft < Projectile.localAI[1])
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, Projectile.scale);
                    dust.noGravity = true;
                }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[0] == 1)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeBigBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                else
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RedJadeBoom>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            }

            //SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            //for (int i = 0; i < 12; i++)
            //{
            //    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Main.rand.NextVector2Circular(4, 4), 0, default, Main.rand.NextFloat(1f, 1.3f));
            //    dust.noGravity = true;
            //}
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
