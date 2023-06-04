using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    /// <summary>
    /// ai0用于控制弹幕颜色
    /// </summary>
    public class StarsBreathBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 70;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }
        
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 31 && Projectile.timeLeft > 5)
            {
                if (Projectile.timeLeft % 10 == 0 && Main.myPlayer == Projectile.owner)
                {
                    float factor = (30 - Projectile.timeLeft) / 10;
                    float scale = 0.4f + 0.1f * factor;
                    Vector2 center = Projectile.Center + Main.rand.NextVector2CircularEdge(8, 8);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, Vector2.Zero, ModContent.ProjectileType<StarsBreathExplosion>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, scale);
                    Particle.NewParticle(center, Vector2.Zero, CoraliteContent.ParticleType<RainbowHalo>(), Color.White, scale + 0.1f);
                    if (factor == 0)
                        PlaySound();
                }
            }

            if (Projectile.timeLeft < 10)
                Projectile.Kill();

        }

        public void PlaySound()
        {
            SoundStyle style = CoraliteSoundID.TerraBlade_Item60;
            style.Volume = 0.9f;
            style.Pitch = 1f;
            SoundEngine.PlaySound(style, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.timeLeft > 32)
                Projectile.timeLeft = 31;

            Projectile.damage = (int)(Projectile.damage * 0.75f);
            if (Projectile.damage < 5)
                Projectile.damage = 5;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 center = Projectile.Center + Main.rand.NextVector2CircularEdge(8, 8);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, Vector2.Zero, ModContent.ProjectileType<StarsBreathExplosion>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, 0.6f);
                Particle.NewParticle(center, Vector2.Zero, CoraliteContent.ParticleType<RainbowHalo>(), Color.White, 0.6f);
                if (Projectile.timeLeft>31)
                    PlaySound();
            }

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 center = Projectile.Center - Main.screenPosition;
            Color shineColor = Projectile.ai[0] switch
            {
                0 => new Color(126, 70, 219),
                1 => new Color(219, 70, 178),
                _ => Color.White
            };
            shineColor *= 0.8f;

            ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, center, new Color(255, 255, 255, 0) * 0.7f, shineColor, 0.5f, 0f, 0.5f, 0.5f, 1f, Projectile.rotation, new Vector2(3, 1.5f), Vector2.One);

            return false;
        }

    }

}