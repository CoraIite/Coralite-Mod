using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class HypnotizeFog : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 128;
            Projectile.timeLeft = 800;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;

            Color color = Main.rand.Next(0, 2) switch
            {
                0 => new Color(110, 68, 200),
                _ => new Color(122, 110, 134)
            };

            if (Main.rand.NextBool())
                Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    Helpers.Helper.NextVec2Dir() * Main.rand.NextFloat(2, 6), CoraliteContent.ParticleType<Fog>(), color, Main.rand.NextFloat(4f, 6f));
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            NightmarePlantera.NightmareHit(target);
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
                return;

            if (np.ai[0] == (int)NightmarePlantera.AIPhases.Sleeping_P1)
                (np.ModNPC as NightmarePlantera).SetPhase1Exchange();

        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
