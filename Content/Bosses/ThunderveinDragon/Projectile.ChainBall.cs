using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// 使用ai0传入状态，0为交错，1为旋转
    /// </summary>
    public class ChainBall : BaseThunderProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "LightingBall";

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];

        public ThunderTrail[] circles1;
        public ThunderTrail[] circles2;
        public ThunderTrail[] chains;

        public Vector2 ballVec;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 110;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurple, ThunderveinDragon.ThunderveinYellow, MathF.Sin(factor * MathHelper.Pi));
        }

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurple, ThunderveinDragon.ThunderveinOrange, MathF.Sin(factor * MathHelper.Pi));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Point pos1 = (Projectile.Center + ballVec).ToPoint();
            Point pos2 = (Projectile.Center - ballVec).ToPoint();

            int width = Projectile.width / 2;
            int height = Projectile.height / 2;

            float a = 0f;

            return new Rectangle(pos1.X - width, pos1.Y - height, Projectile.width, Projectile.height).Intersects(targetHitbox)
                || new Rectangle(pos2.X - width, pos2.Y - height, Projectile.width, Projectile.height).Intersects(targetHitbox)
                || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + ballVec, Projectile.Center - ballVec, Projectile.width / 4, ref a);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            InitTrails();
            UpdateTrails();

            //随机拖尾

            Timer++;

            RandomTrails();

            if (ThunderAlpha < 1)
            {
                ThunderWidth = 16;
                ThunderAlpha += 1 / 30f;
                if (ThunderAlpha > 1)
                    ThunderAlpha = 1;
            }

            switch (State)
            {
                default:
                case 0://交错
                    {
                        ballVec = (Projectile.rotation + 1.57f).ToRotationVector2() * MathF.Sin(Timer * 0.05f) * 140;
                        if (Timer > 45)
                            for (int i = -1; i < 2; i += 2)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center + (i * ballVec) + Main.rand.NextVector2Circular(24, 24), DustID.PortalBoltTrail, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.5f),
                                    newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.3f));
                                d.noGravity = true;
                            }
                    }
                    break;
                case 1://螺旋
                    {
                        float rot = Timer * 0.04f;
                        ballVec = rot.ToRotationVector2() * MathHelper.Clamp(Timer / 120f, 0, 1) * 220;

                        if (Timer > 45)
                            for (int i = -1; i < 2; i += 2)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center + (i * ballVec) + Main.rand.NextVector2Circular(24, 24), DustID.PortalBoltTrail, (rot - (i * 1.57f)).ToRotationVector2() * Main.rand.NextFloat(2f, 6f),
                                    newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.3f));
                                d.noGravity = true;
                            }
                    }
                    break;
            }
        }

        public void InitTrails()
        {
            if (circles1 == null)
            {
                circles1 = new ThunderTrail[5];
                circles2 = new ThunderTrail[5];
                chains = new ThunderTrail[3];
                Asset<Texture2D> thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");

                for (int i = 0; i < circles1.Length; i++)
                {
                    if (i < 2)
                        circles1[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    else
                        circles1[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);

                    circles1[i].SetRange((0, 8));
                    circles1[i].SetExpandWidth(6);
                }

                for (int i = 0; i < circles2.Length; i++)
                {
                    if (i < 2)
                        circles2[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    else
                        circles2[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);

                    circles2[i].SetRange((0, 8));
                    circles2[i].SetExpandWidth(6);
                }

                for (int i = 0; i < chains.Length; i++)
                {
                    chains[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    chains[i].SetRange((0, 10));
                    chains[i].SetExpandWidth(6);
                }

                int cacheLength = 5;
                float length = 2 * ballVec.Length() / cacheLength;
                foreach (var trail in chains)
                {
                    if (cacheLength < 3)
                        trail.CanDraw = false;
                    else
                    {
                        Vector2[] vec = new Vector2[cacheLength];
                        Vector2 basePos = Projectile.Center + ballVec;
                        Vector2 dir = -ballVec.SafeNormalize(Vector2.Zero);
                        vec[0] = basePos;

                        for (int i = 1; i < cacheLength; i++)
                        {
                            vec[i] = basePos + (dir * i);
                        }

                        trail.BasePositions = vec;
                        trail.RandomThunder();
                    }
                }

                foreach (var circle in circles1)
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

                foreach (var circle in circles2)
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
            foreach (var circle in circles1)
                circle.UpdateTrail(Projectile.velocity);
            foreach (var circle in circles2)
                circle.UpdateTrail(Projectile.velocity);
            foreach (var trail in chains)
                trail.UpdateTrail(Projectile.velocity);
        }

        public void RandomTrails()
        {
            if (Timer % 5 == 0)
            {
                int cacheLength = 7;
                float length = 2 * ballVec.Length() / cacheLength;
                if (length > 12)
                    foreach (var trail in chains)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        if (trail.CanDraw)
                        {
                            Vector2[] vec = new Vector2[cacheLength];
                            Vector2 basePos = Projectile.Center + ballVec;
                            Vector2 dir = -ballVec.SafeNormalize(Vector2.Zero);
                            vec[0] = basePos;

                            for (int i = 1; i < cacheLength; i++)
                            {
                                vec[i] = basePos + (dir * i * length);
                            }

                            trail.BasePositions = vec;
                            trail.RandomThunder();
                        }
                    }
                else
                    foreach (var trail in chains)
                        trail.CanDraw = false;
            }

            if (Timer % 4 == 0)
            {
                if (Timer % 8 == 0)
                    ElectricParticle_Follow.Spawn(Projectile.Center + ballVec, Main.rand.NextVector2Circular(30, 30),
                        () => Projectile.Center + ballVec, Main.rand.NextFloat(0.5f, 0.75f));
                else
                    ElectricParticle_Follow.Spawn(Projectile.Center - ballVec, Main.rand.NextVector2Circular(30, 30),
                        () => Projectile.Center - ballVec, Main.rand.NextFloat(0.5f, 0.75f));

                foreach (var circle in circles1)
                {
                    circle.CanDraw = Main.rand.NextBool();
                    if (circle.CanDraw)
                    {
                        int width = Main.rand.Next(Projectile.width / 5, Projectile.width / 2);
                        float angle = MathHelper.TwoPi / (20 + (15 * Helper.Lerp(width / (float)(Projectile.width / 2), 0, 1)));
                        int trailPointCount = Main.rand.Next(5, 20);
                        Vector2[] vec = new Vector2[trailPointCount];

                        Vector2 basePos = Projectile.Center + ballVec;
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

                foreach (var circle in circles2)
                {
                    circle.CanDraw = Main.rand.NextBool();
                    if (circle.CanDraw)
                    {
                        int width = Main.rand.Next(Projectile.width / 5, Projectile.width / 2);
                        float angle = MathHelper.TwoPi / (20 + (15 * Helper.Lerp(width / (float)(Projectile.width / 2), 0, 1)));
                        int trailPointCount = Main.rand.Next(5, 20);
                        Vector2[] vec = new Vector2[trailPointCount];

                        Vector2 basePos = Projectile.Center - ballVec;
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
            for (int j = -1; j < 2; j += 2)
            {
                PRTLoader.NewParticle(Projectile.Center + (j * ballVec), Vector2.Zero, CoraliteContent.ParticleType<LightningParticle>(), Scale: 2.5f);

                float baseRot = Main.rand.NextFloat(6.282f);
                for (int i = 0; i < 5; i++)
                {
                    PRTLoader.NewParticle(Projectile.Center + (j * ballVec) + ((baseRot + (i * MathHelper.TwoPi / 5)).ToRotationVector2() * Main.rand.NextFloat(20, 30))
                        , Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>());
                }
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(Projectile.Center + ballVec, Color.White, 0f);
            Projectile.QuickDraw(Projectile.Center - ballVec, Color.White, 0f);

            if (circles1 != null)
                foreach (var circle in circles1)
                    circle.DrawThunder(Main.instance.GraphicsDevice);
            if (circles2 != null)
                foreach (var circle in circles2)
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
            Color c = ThunderveinDragon.ThunderveinYellowAlpha;
            c.A = (byte)(ThunderAlpha * 250);
            var origin = exTex.Size() / 2;
            var scale = Projectile.scale * 0.5f;

            spriteBatch.Draw(exTex, pos + ballVec, null, c, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);
            spriteBatch.Draw(exTex, pos + ballVec, null, c * 0.5f, Projectile.rotation - (Main.GlobalTimeWrappedHourly / 2), origin, scale, 0, 0);

            spriteBatch.Draw(exTex, pos - ballVec, null, c, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);
            spriteBatch.Draw(exTex, pos - ballVec, null, c * 0.5f, Projectile.rotation - (Main.GlobalTimeWrappedHourly / 2), origin, scale, 0, 0);
        }
    }
}
