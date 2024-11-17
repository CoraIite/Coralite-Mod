using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class HypnotizeFog : ModProjectile, IDrawNonPremultiplied
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

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(0, 4);
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0.72f)
                Projectile.ai[0] += 0.05f;

            Projectile.velocity *= 0.98f;
            Projectile.rotation += Main.rand.NextFloat(-0.08f, 0.12f);
            Color color = Main.rand.Next(0, 2) switch
            {
                0 => new Color(110, 68, 200),
                _ => new Color(122, 110, 134)
            };

            if (Main.rand.NextBool())
                PRTLoader.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    Helpers.Helper.NextVec2Dir(2, 6), CoraliteContent.ParticleType<Fog>(), color, Main.rand.NextFloat(4f, 6f));

            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), ModContent.DustType<NightmarePetal>(), newColor: NightmarePlantera.nightPurple);
                dust.velocity = (dust.position - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 4);
                dust.noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<DreamErosion>(), 18000);
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
                return;

            if (target.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.nightmareCount < 14)
                    cp.nightmareCount++;

                //设置阶段并秒杀玩家
                if (cp.nightmareCount > 13)
                    (np.ModNPC as NightmarePlantera).ChangeToSuddenDeath(target);

                if (target.whoAmI == Main.myPlayer)
                    Filters.Scene.Activate("NightmareScreen", target.position);
            }

            if (np.ai[0] == (int)NightmarePlantera.AIPhases.Sleeping_P1)
                (np.ModNPC as NightmarePlantera).SetPhase1Exchange();

        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D tex = NightmarePlantera.flowerParticleTex.Value;
            Rectangle frameBox = tex.Frame(5, 3, Projectile.frame, 1);
            Vector2 origin = frameBox.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            float a = Projectile.ai[0];
            Color c = NightmarePlantera.nightPurple;
            c.A = (byte)(c.A * a);
            spriteBatch.Draw(tex, pos, frameBox, c, Projectile.rotation, origin, 1.5f, 0, 0);
        }
    }
}
