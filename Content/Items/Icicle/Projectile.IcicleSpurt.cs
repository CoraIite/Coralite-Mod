using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    /// <summary>
    /// ai0 用于存储玩家方向
    /// ai1 用于存储玩家的使用物品时间
    /// </summary>
    public class IcicleSpurt : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public ref float OwnerDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];

        public ref float Alpha => ref Projectile.localAI[1];
        public bool fadeIn = true;
        public bool canDamage = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (fadeIn)
            {
                if (Alpha == 0f)
                {
                    Projectile.timeLeft = (int)MaxTime;
                    Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                }

                Alpha += 0.15f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                    fadeIn = false;
                }
            }

            if (canDamage)
            {
                Vector2 targetDir = (Projectile.rotation - 1.57f).ToRotationVector2();
                for (int i = 0; i < 2; i++)
                {
                    if (Framing.GetTileSafely(Projectile.Center + (targetDir * i * 16)).HasReallySolidTile())
                    {
                        Projectile.timeLeft = 10;
                        canDamage = false;
                        Projectile.netUpdate = true;
                        break;
                    }
                }

                Color lightColor = Lighting.GetColor((Projectile.Center / 16).ToPoint());
                PRTLoader.NewParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 0.15f, CoraliteContent.ParticleType<Fog>(), lightColor * Alpha, Main.rand.NextFloat(0.6f, 0.8f));
                if (Projectile.timeLeft % 3 == 0)
                    PRTLoader.NewParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 0.3f, CoraliteContent.ParticleType<SnowFlower>(), lightColor * Alpha, Main.rand.NextFloat(0.2f, 0.4f));
            }

            Lighting.AddLight(Projectile.Center, Coralite.IcicleCyan.ToVector3());

            if (Projectile.timeLeft < 10)
            {
                Projectile.velocity *= 0.65f;
                if (Alpha > 0f)
                {
                    if (!fadeIn)
                        Alpha -= 0.1f;
                    Projectile.timeLeft += 1;
                }
            }
        }

        public override bool? CanDamage() => canDamage;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
            if (Projectile.damage > 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 center = Projectile.Center - new Vector2(0, Main.rand.Next(140, 220)).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
                Vector2 velocity = (Projectile.Center + Main.rand.NextVector2Circular(8, 8) - center).SafeNormalize(Vector2.UnitY) * 12;
                Projectile.NewProjectileFromThis<IcicleFalling>(center, velocity,
                     (int)(Projectile.damage * 0.9f), Projectile.knockBack);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            SpriteEffects effects = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor * Alpha, Projectile.rotation, mainTex.Size() / 2, 1.2f, effects, 0f);
            return false;
        }
    }
}