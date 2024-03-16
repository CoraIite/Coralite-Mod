using Terraria;

namespace Coralite.lib
{
    public class AI_SharpTears
    {
        public void AI_157_SharpTears(Projectile Projectile)
        {
            int num = 5;
            float scaleFactor = 1f;
            int num2 = 30;
            int num3 = 30;
            int num4 = 2;
            int num5 = 2;
            int _20 = 20;
            int _30 = 30;
            int maxValue = 6;

            bool flag = Projectile.ai[0] < _20;
            bool flag2 = Projectile.ai[0] >= _20;
            bool flag3 = Projectile.ai[0] >= _30;

            Projectile.ai[0] += 1f;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.frame = Main.rand.Next(maxValue);//随机变贴图..啊这

                //生成粒子
                for (int i = 0; i < num2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), num, Projectile.velocity * scaleFactor * MathHelper.Lerp(0.2f, 0.7f, Main.rand.NextFloat()));
                    dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                }

                for (int j = 0; j < num3; j++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), num, Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * scaleFactor * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                    dust2.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust2.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                    dust2.fadeIn = 1f;
                }

                //if (type == 961)
                //    SoundEngine.PlaySound(SoundID.DeerclopsIceAttack, Projectile.Center);
                //else
                //    SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            }

            if (flag)
            {
                Projectile.Opacity += 0.1f;
                Projectile.scale = Projectile.Opacity * Projectile.ai[1];
                for (int k = 0; k < num4; k++)
                {
                    Dust dust3 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f), num, Projectile.velocity * scaleFactor * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                    dust3.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust3.velocity *= 0.5f;
                    dust3.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                }
            }

            if (flag2)
            {
                Projectile.Opacity -= 0.2f;
                for (int l = 0; l < num5; l++)
                {
                    Dust dust4 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f), num, Projectile.velocity * scaleFactor * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                    dust4.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust4.velocity *= 0.5f;
                    dust4.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                }
            }

            if (flag3)
                Projectile.Kill();

            //if (type == 756)
            //    Lighting.AddLight(base.Center, new Vector3(0.5f, 0.1f, 0.1f) * scale);
        }
    }
}
