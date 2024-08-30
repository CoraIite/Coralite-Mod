using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// 使用速度传入目标点位
    /// ai0传入闪电降下的时间
    /// </summary>
    public class ThunderFalling : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float LightingTime => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];

        protected ThunderTrail[] thunderTrails;

        const int DelayTime = 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2800;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            if (Timer > LightingTime + DelayTime / 2)
                return false;

            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.velocity, Projectile.Center, Projectile.width, ref a);
        }

        public virtual float ThunderWidthFunc(float factor)
        {
            return ThunderWidth * factor;
        }

        public override void AI()
        {
            if (thunderTrails == null)
            {
                Projectile.Resize((int)PointDistance, 40);
                thunderTrails = new ThunderTrail[3];
                Asset<Texture2D> trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc, ThunderColorFunc_Yellow, GetAlpha);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].SetRange((0, 15));
                    thunderTrails[i].BasePositions =
                    [
                    Projectile.Center,Projectile.Center,Projectile.Center
                    ];
                }
            }

            if (Timer < LightingTime)
            {
                float factor = Timer / LightingTime;
                Vector2 targetPos = Vector2.Lerp(Projectile.Center, Projectile.velocity, factor);
                Vector2 pos2 = targetPos;

                List<Vector2> pos = new()
                {
                    targetPos
                };
                if (Vector2.Distance(targetPos, Projectile.Center) < PointDistance)
                    pos.Add(Projectile.Center);
                else
                    for (int i = 0; i < 40; i++)
                    {
                        pos2 = pos2.MoveTowards(Projectile.Center, PointDistance);
                        if (Vector2.Distance(pos2, Projectile.Center) < PointDistance)
                        {
                            pos.Add(Projectile.Center);
                            break;
                        }
                        else
                            pos.Add(pos2);
                    }

                foreach (var trail in thunderTrails)
                {
                    trail.BasePositions = pos.ToArray();
                    trail.SetExpandWidth(4);
                }

                if (Timer % 4 == 0)
                {
                    foreach (var trail in thunderTrails)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        if (trail.CanDraw)
                            trail.RandomThunder();
                    }
                }

                SpawnDusts();
                ThunderWidth = 50 + 70 * factor;
                ThunderAlpha = factor;
            }
            else if ((int)Timer == (int)LightingTime)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
            else
            {
                float factor = (Timer - LightingTime) / (DelayTime);
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                ThunderWidth = 20 + (1 - factor) * 100;
                ThunderAlpha = 1 - Coralite.Instance.X2Smoother.Smoother(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange((0, 10 + (1 - factor) * PointDistance / 2));
                    trail.SetExpandWidth((1 - factor) * PointDistance / 3);

                    if (Timer % 6 == 0)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                if (Timer > LightingTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        public virtual void SpawnDusts()
        {
            float factor = Timer / LightingTime;
            Vector2 targetPos = Vector2.Lerp(Projectile.Center, Projectile.velocity, factor);

            Vector2 pos = Vector2.Lerp(targetPos, Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
                + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
            if (Main.rand.NextBool())
            {
                Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
            }
            else
            {
                Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.3f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (thunderTrails != null)
            {
                foreach (var trail in thunderTrails)
                {
                    trail?.DrawThunder(Main.instance.GraphicsDevice);
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 使用速度传入目标点位
    /// ai0传入闪电降下的时间
    /// </summary>
    public class StrongThunderFalling : ThunderFalling
    {
        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinYellowAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinOrangeAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }
    }

    /// <summary>
    /// 使用速度传入目标点位
    /// ai0传入闪电降下的时间
    /// ai1传入主人
    /// </summary>
    public class EndThunder : StrongThunderFalling
    {
        const int DelayTime = 50;

        public ref float OwnerIndex => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 80;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override float ThunderWidthFunc(float factor)
        {
            return ThunderWidth;
        }

        public override void SpawnDusts()
        {
            float factor = Timer / LightingTime;
            Vector2 targetPos = Vector2.Lerp(Projectile.Center, Projectile.velocity, factor);

            Vector2 pos = Vector2.Lerp(targetPos, Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
                + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
            if (Main.rand.NextBool())
            {
                Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
            }
            else
            {
                Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.3f));
            }
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner, Projectile.Kill))
                return;

            if (thunderTrails == null)
            {
                //Projectile.Resize((int)PointDistance, 40);
                thunderTrails = new ThunderTrail[3];
                Asset<Texture2D> trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc, ThunderColorFunc_Yellow, GetAlpha);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].SetRange((0, 50));
                    thunderTrails[i].BasePositions = new Vector2[3]
                    {
                        Projectile.Center,Projectile.Center,Projectile.Center
                    };
                }
            }

            if (Timer < LightingTime)
            {
                float factor = Timer / LightingTime;
                Vector2 center = Projectile.Center;
                Vector2 targetPos = Vector2.Lerp(center, Projectile.velocity, factor);
                Vector2 pos2 = targetPos;

                List<Vector2> pos = new()
                {
                    targetPos
                };
                if (Vector2.Distance(targetPos, center) < PointDistance)
                    pos.Add(center);
                else
                    for (int i = 0; i < 40; i++)
                    {
                        pos2 = pos2.MoveTowards(center, PointDistance);
                        if (Vector2.Distance(pos2, center) < PointDistance)
                        {
                            pos.Add(center);
                            break;
                        }
                        else
                            pos.Add(pos2);
                    }

                foreach (var trail in thunderTrails)
                {
                    trail.BasePositions = pos.ToArray();
                    trail.SetExpandWidth(20);
                }

                if (Timer % 4 == 0)
                {
                    foreach (var trail in thunderTrails)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        if (trail.CanDraw)
                            trail.RandomThunder();
                    }
                }

                SpawnDusts();

                ThunderWidth = 70 + 70 * factor;
                ThunderAlpha = factor;
            }
            else if ((int)Timer == (int)LightingTime)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
            else
            {
                SpawnDusts();

                float factor = (Timer - LightingTime) / (DelayTime);
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                ThunderWidth = 20 + (1 - factor) * 120;
                ThunderAlpha = 1 - Coralite.Instance.X2Smoother.Smoother(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange((0, 10 + (1 - factor) * PointDistance / 2));
                    trail.SetExpandWidth((1 - factor) * PointDistance / 3);

                    if (Timer % 6 == 0)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                if (Timer > LightingTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }
    }
}
