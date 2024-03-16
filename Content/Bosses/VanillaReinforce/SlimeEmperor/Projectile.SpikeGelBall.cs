using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class SpikeGelBall : GelBall
    {
        public override void OnKill(int timeLeft)
        {
            //生成粒子
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                //生成一些尖刺弹幕
                int howMany = Helper.ScaleValueForDiffMode(3, 4, 5, 6);
                for (int i = 0; i < howMany; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height),
                        -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1f, 1f)) * Main.rand.NextFloat(10, 14), ModContent.ProjectileType<GelSpike>(), Projectile.damage, 0, Projectile.owner);
                }
            }
        }
    }
}
