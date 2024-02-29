using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    public class Bloodiancie_BigBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

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

        public override void OnSpawn(IEntitySource source)
        {
            Vector2 center = Projectile.Center;
            Helper.PlayPitched("RedJade/RedJadeBoom", 1f, -1f, center);

            Color red = new Color(221, 50, 50);
            int type = CoraliteContent.ParticleType<LightBall>();

            for (int i = 0; i < 5; i++)
            {
                Particle.NewParticle(center, Helper.NextVec2Dir(38, 40), type, red, Main.rand.NextFloat(0.15f, 0.2f));
            }
            for (int i = 0; i < 10; i++)
            {
                Particle.NewParticle(center, Helper.NextVec2Dir(24, 30), type, red, Main.rand.NextFloat(0.1f, 0.15f));
                Particle.NewParticle(center, Helper.NextVec2Dir(24, 30), type, Color.White, Main.rand.NextFloat(0.05f, 0.1f));
                Dust dust = Dust.NewDustPerfect(center, DustID.GemRuby, Helper.NextVec2Dir(6, 10), Scale: Main.rand.NextFloat(2f, 2.4f));
                dust.noGravity = true;
            }

            Items.RedJades.RedExplosionParticle.Spawn(center, 1.4f, Coralite.Instance.RedJadeRed);
            Items.RedJades.RedGlowParticle.Spawn(center, 1.3f, Coralite.Instance.RedJadeRed, 0.4f);
            Items.RedJades.RedGlowParticle.Spawn(center, 1.3f, Coralite.Instance.RedJadeRed, 0.4f);

            var modifier = new PunchCameraModifier(center, Helper.NextVec2Dir(), 20, 8f, 14, 1000f);
            Main.instance.CameraModifiers.Add(modifier);
        }

        public override bool PreAI() => false;
        public override bool PreDraw(ref Color lightColor) => false;

        public override bool CanHitPlayer(Player target)
        {
            return Vector2.Distance(Projectile.Center, target.Center) < Projectile.width / 2;
        }
    }
}
