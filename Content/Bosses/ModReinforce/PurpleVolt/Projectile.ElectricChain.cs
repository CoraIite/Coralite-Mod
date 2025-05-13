using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// 闪电链弹幕，其实就是电球加强版<br></br>
    /// 使用ai0传入目标弹幕，ai1传入最大时间
    /// </summary>
    public class ElectricChain : BaseZacurrentProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ZacurrentDragon + "LightingBall";

        public ref float ChainProjIndex => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        public ThunderTrail[] circles;
        public ThunderTrail[] chains;

        public override void SetDefaults()
        {
            Projectile.scale = 0.6f;
            Projectile.width = Projectile.height = 110;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2000;
        }

        public override Color ThunderColorFunc_Purple(float factor)
        {
            return Color.Lerp(ZacurrentDragon.ZacurrentPink, ZacurrentDragon.ZacurrentPurple, MathF.Sin(factor * MathHelper.Pi));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            bool collide = projHitbox.Intersects(targetHitbox);

            //如果有连接那就连一下
            if (ChainProjIndex.GetProjectileOwner(out Projectile p))
            {
                float a = 0f;
                collide = collide || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft()
                    , targetHitbox.Size(), Projectile.Center, p.Center, Projectile.width / 4, ref a);
            }

            return collide;
        }

        public override void AI()
        {
            Vector2? targetP = null;

            if (ChainProjIndex.GetProjectileOwner(out Projectile p, () =>
            {
                ChainProjIndex = -1;
                if (chains != null)
                    for (int i = 0; i < chains.Length; i++)
                        chains[i].CanDraw = false;

            }))
            {
                targetP = p.Center;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            InitTrails();
            UpdateTrails();

            //随机拖尾
            Timer++;

            RandomTrails(targetP);

            if (ThunderAlpha < 1)
            {
                ThunderWidth = 16;
                ThunderAlpha += 1 / 30f;
                if (ThunderAlpha > 1)
                    ThunderAlpha = 1;
            }

            if (Timer > MaxTime)
                Projectile.Kill();
        }

        public void InitTrails()
        {
            if (circles == null)
            {
                circles = new ThunderTrail[5];
                chains = new ThunderTrail[3];
                ATex thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");

                for (int i = 0; i < circles.Length; i++)
                {
                    circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Purple, GetAlpha);

                    circles[i].SetRange((0, 5));
                    circles[i].SetExpandWidth(6);
                }

                for (int i = 0; i < chains.Length; i++)
                {
                    chains[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Purple, GetAlpha);
                    chains[i].SetRange((0, 10));
                    chains[i].SetExpandWidth(6);
                }

                foreach (var circle in circles)
                {
                    circle.CanDraw = true;
                    int trailPointCount = Main.rand.Next(15, 30);
                    Vector2[] vec = new Vector2[trailPointCount];

                    float baseRot = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < trailPointCount; i++)
                    {
                        vec[i] = Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 40)).ToRotationVector2()
                            * ((Projectile.width / 2) + Main.rand.NextFloat(-8, 8)));
                    }

                    circle.BasePositions = vec;
                    circle.RandomThunder();
                }
            }
        }

        public void UpdateTrails()
        {
            foreach (var circle in circles)
                circle.UpdateTrail(Projectile.velocity);
            foreach (var trail in chains)
                trail.UpdateTrail(Projectile.velocity);
        }

        public void RandomTrails(Vector2?  targetPos)
        {
            if (targetPos.HasValue)
            {
                if (Timer % 8 == 0)
                {
                    float perLength = 45;
                    float distance = Vector2.Distance(Projectile.Center, targetPos.Value);
                    Vector2 p = targetPos.Value;
                    Vector2 dir = (Projectile.Center - targetPos.Value).SafeNormalize(Vector2.Zero);

                    List<Vector2> points = new();

                    int count = (int)(distance / perLength) + 1;
                    for (int i = 0; i < count; i++)
                    {
                        float distance2 = p.Distance(Projectile.Center);
                        if (distance2 < perLength)
                            perLength = distance2;
                        points.Add(p);
                        p += dir * perLength;
                    }

                    Vector2[] points2 = [.. points];

                    foreach (var trail in chains)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        if (trail.CanDraw)
                        {
                            if (Timer % 16 == 0)
                            {
                                trail.BasePositions = points2;
                                trail.RandomThunder();
                            }
                            else
                                trail.UpdateThunderToNewPosition(points2);
                        }
                    }
                }
            }

            if (Timer % 4 == 0)
            {
                ElectricParticle_PurpleFollow.Spawn(Projectile.Center, Main.rand.NextVector2Circular(30, 30),
                    () => Projectile.Center, Main.rand.NextFloat(0.5f, 0.75f));

                foreach (var circle in circles)
                {
                    circle.CanDraw = Main.rand.NextBool();
                    if (circle.CanDraw)
                    {
                        int width = Main.rand.Next(Projectile.width / 5, Projectile.width / 2);
                        float angle = MathHelper.TwoPi / (20 + (15 * Helper.Lerp(width / (float)(Projectile.width / 2), 0, 1)));
                        int trailPointCount = Main.rand.Next(5, 20);
                        Vector2[] vec = new Vector2[trailPointCount];

                        Vector2 basePos = Projectile.Center;
                        float baseRot = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < trailPointCount; i++)
                        {
                            vec[i] = basePos + ((baseRot + (i * angle)).ToRotationVector2()
                                * width);
                        }

                        circle.BasePositions = vec;
                        circle.RandomThunder();
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            PRTLoader.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<LightningParticlePurple>(), Scale: 2.5f);

            float baseRot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < 5; i++)
            {
                PRTLoader.NewParticle(Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 5)).ToRotationVector2() * Main.rand.NextFloat(20, 30))
                    , Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>());
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(Projectile.Center , Color.White, 0f);

            if (circles != null)
                foreach (var circle in circles)
                    circle.DrawThunder(Main.instance.GraphicsDevice);
            if (chains != null)
                foreach (var trail in chains)
                    trail.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D exTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightFog").Value;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = ZacurrentDragon.ZacurrentPinkAlpha;
            c.A = (byte)(ThunderAlpha * 250);
            var origin = exTex.Size() / 2;
            var scale = Projectile.scale * 0.5f;

            spriteBatch.Draw(exTex, pos , null, c, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);
            spriteBatch.Draw(exTex, pos, null, c * 0.5f, Projectile.rotation - (Main.GlobalTimeWrappedHourly / 2), origin, scale, 0, 0);
        }
    }
}
