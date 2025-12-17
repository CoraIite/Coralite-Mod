using Coralite.Content.Dusts;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    public class Bloodiancie_BigBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        bool span;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 320;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 10;

            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public virtual void Initialize()
        {
            Vector2 center = Projectile.Center;
            Helper.PlayPitched("RedJade/RedJadeBoom", 1f, -1f, center);

            Color red = new(221, 50, 50);
            int type = CoraliteContent.ParticleType<LightBall>();

            for (int i = 0; i < 5; i++)
            {
                PRTLoader.NewParticle(center, Helper.NextVec2Dir(38, 40), type, red, Main.rand.NextFloat(0.15f, 0.2f));
            }
            for (int i = 0; i < 10; i++)
            {
                PRTLoader.NewParticle(center, Helper.NextVec2Dir(24, 30), type, red, Main.rand.NextFloat(0.1f, 0.15f));
                //PRTLoader.NewParticle(center, Helper.NextVec2Dir(24, 30), type, Color.White, Main.rand.NextFloat(0.05f, 0.1f));

                var p = PRTLoader.NewParticle<PixelLine>(center, Helper.NextVec2Dir(8,15), newColor: Coralite.RedJadeRed with { A = 135 }
                      , Scale: 1.5f);
                p.TrailCount = 8;
                p.fadeFactor = 0.88f;

                Dust dust = Dust.NewDustPerfect(center, DustID.GemRuby, Helper.NextVec2Dir(6, 10), Scale: Main.rand.NextFloat(2f, 2.4f));
                dust.noGravity = true;
            }

            //Items.RedJades.RedExplosionParticle.Spawn(center, 1.4f, Coralite.RedJadeRed);
            //Items.RedJades.RedGlowParticle.Spawn(center, 1.3f, Coralite.RedJadeRed, 0.4f);
            //Items.RedJades.RedGlowParticle.Spawn(center, 1.3f, Coralite.RedJadeRed, 0.4f);

            RedGlowParticle2.Spawn(center, 1.3f, (Color.DarkRed * 0.1f) with { A = 80 }, (Color.DarkRed * 0.2f) with { A = 100 }, 0.4f);
            RedGlowParticle2.Spawn(center, 1.1f, Coralite.RedJadeRed * 0.2f, (Coralite.RedJadeRed * 0.8f) with { A = 150 }, 0.4f);

            RedExplosionParticle2.Spawn(center, 1.5f, Coralite.RedJadeRed * 0.3f);
            RedExplosionParticle2.Spawn(center, 1.2f, Coralite.RedJadeRed * 0.7f);

            var modifier = new PunchCameraModifier(center, Helper.NextVec2Dir(), 20, 8f, 14, 1000f);
            Main.instance.CameraModifiers.Add(modifier);
        }

        public override bool PreAI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override bool CanHitPlayer(Player target)
        {
            return Vector2.Distance(Projectile.Center, target.Center) < Projectile.width / 2;
        }
    }
}
