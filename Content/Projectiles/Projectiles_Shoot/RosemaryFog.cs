using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    /// <summary>
    /// ai0用于控制阶段，ai1用于控制发射角度
    /// </summary>
    public class RosemaryFog : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Rotation => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float Alpha => ref Projectile.localAI[1];

        public int justATimer;
        public float visualAlpha = 0f;
        public float visualScale = 0.8f;
        public bool fadeIn = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Color(255, 179, 251).ToVector3() * 0.5f);   //粉色的魔法数字

            if (fadeIn)
            {
                Alpha += 0.02f;
                visualAlpha += 0.0134f;
                if (Alpha > 0.6f)
                {
                    Alpha = 0.6f;
                    visualAlpha = 0.4f;
                    fadeIn = false;
                }
                return;
            }
            else if (State == 1)
            {
                Alpha -= 0.04f;
                if (Timer < 13)  //射出3发弹幕
                {
                    if (Timer % 6 == 0 && Main.myPlayer == Projectile.owner)
                    {
                        NPC target = Helpers.ProjectilesHelper.FindClosestEnemy(Projectile.Center, 440,
                            npc => npc.active && !npc.friendly && npc.CanBeChasedBy() && Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1));

                        if (target is not null)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 14, ModContent.ProjectileType<RosemaryBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                        else
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Rotation.ToRotationVector2() * 14, ModContent.ProjectileType<RosemaryBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
                    }
                }

                if (visualAlpha < 0.05f)
                    visualAlpha = 0.05f;

                if (Alpha < 0.05f)
                    Projectile.Kill();

                Timer += 1;
            }

            if (Projectile.timeLeft < 20)
            {
                Alpha -= 0.04f;
                if (visualAlpha < 0.05f)
                    visualAlpha = 0.05f;
            }

            if (justATimer % 3 == 0)
                Particle.NewParticle(Projectile.Center, new Vector2(0, 3f).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)), CoraliteContent.ParticleType<Fog>(), new Color(95, 120, 233, 255), Main.rand.NextFloat(0.5f, 0.7f));

            if (Main.rand.NextBool(15))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Ice_Purple, new Vector2(0, 4f).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                dust.noGravity = true;
            }

            //只是视觉效果
            visualAlpha -= 0.015f;
            visualScale += 0.02f;
            if (visualAlpha < 0.02f)
            {
                visualAlpha = 0.4f;
                visualScale = 0.8f;
            }

            justATimer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 4);

            Main.spriteBatch.Draw(mainTex, center, mainTex.Frame(1, 2, 0, 0), Color.White * Alpha, 0f, origin, 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(mainTex, center, mainTex.Frame(1, 2, 0, 1), new Color(255, 219, 253) * visualAlpha, 0f, origin, visualScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}