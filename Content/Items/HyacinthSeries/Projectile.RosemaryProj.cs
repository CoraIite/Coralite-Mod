using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class RosemaryHeldProj : BaseGunHeldProj
    {
        public RosemaryHeldProj() : base(0.05f, 14, -2, AssetDirectory.HyacinthSeriesItems) { }

        public override void Initialize()
        {
            Projectile.timeLeft = Owner.itemTime + 1;
            MaxTime = Owner.itemTime + 1;
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                TargetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (OwnerDirection > 0 ? 0f : MathHelper.Pi);
                if (TargetRot == 0f)
                    TargetRot = 0.0001f;
            }

            Projectile.netUpdate = true;
        }
    }

    public class RosemaryHeldProj2 : BaseGunHeldProj
    {
        public RosemaryHeldProj2() : base(0.05f, 16, -2, AssetDirectory.HyacinthSeriesItems) { }

        public override void Initialize()
        {
            Projectile.timeLeft = Owner.itemTime + 1;
            MaxTime = Owner.itemTime + 1;
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                TargetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (OwnerDirection > 0 ? 0f : MathHelper.Pi);
                if (TargetRot == 0f)
                    TargetRot = 0.0001f;
            }

            Projectile.localAI[1] += 1;
            Projectile.netUpdate = true;
        }

        public override void ModifyAI(float factor)
        {
            if (Projectile.localAI[1] > 2)
                return;
            if (Projectile.timeLeft < 2)
                Initialize();
        }
    }

    /// <summary>
    /// 使用ai0控制是否能产生特殊弹幕，为0时能产生花雾弹幕
    /// </summary>
    public class RosemaryBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ArethusaPetal>(), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.05f, 0.15f));
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Color(255, 179, 251).ToVector3() * 0.5f);   //粉色的魔法数字
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Ice_Purple, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.15f, 0.25f), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                    dust.noGravity = true;
                }

            if (Main.myPlayer != Projectile.owner || Projectile.ai[0] != 0)
                return;

            if (hit.Crit)
            {
                SpawnFogProj();
                return;
            }

            if (!target.active)
            {
                SpawnFogProj();
                return;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Ice_Purple, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.15f, 0.25f), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                    dust.noGravity = true;
                }

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }

        public void SpawnFogProj()
        {
            int length = Main.rand.Next(160, 200);
            float rotate = Main.rand.NextFloat(6.282f);

            Vector2 dir = rotate.ToRotationVector2();
            Vector2 targetCenter = Projectile.Center + dir * length;
            for (int i = 0; i < 8; i++)
            {
                if (Collision.CanHitLine(Projectile.Center, 1, 1, targetCenter, 1, 1))
                    break;

                rotate += 0.785f;
                dir = rotate.ToRotationVector2();
                targetCenter = Projectile.Center + dir * length;
            }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                float roughlySpeed = length / 12f;
                FlowLine.Spawn(Projectile.Center + dir * 8, dir * roughlySpeed, 2, 12, 0.04f, new Color(95, 120, 233, 100));
                for (int i = -1; i < 4; i += 2)
                    FlowLine.Spawn(Projectile.Center + dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * (32 + i * 8), dir * roughlySpeed * 0.5f, 1, 12, Math.Sign(i) * 0.1f, new Color(255, 179, 251, 60));
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), targetCenter, Vector2.Zero,
                ModContent.ProjectileType<RosemaryFog>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
        }
    }

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
                        NPC target = Helper.FindClosestEnemy(Projectile.Center, 440,
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
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 4);

            Main.spriteBatch.Draw(mainTex, center, mainTex.Frame(1, 2, 0, 0), Color.White * Alpha, 0f, origin, 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(mainTex, center, mainTex.Frame(1, 2, 0, 1), new Color(255, 219, 253) * visualAlpha, 0f, origin, visualScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
