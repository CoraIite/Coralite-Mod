using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    [AutoLoadTexture(Path = AssetDirectory.ZacurrentDragon)]
    public class PurpleElectricBall : BaseZacurrentProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "LightingBall";

        public ThunderTrail[] circles;

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float Angle => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        public static ATex BallBack { get; set; }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 110;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public float ThunderWidthFunc2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth * 1.2f;
        }

        public float GetAlphaFade(float factor)
        {
            return ThunderAlpha * (1 - factor);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            foreach (var circle in circles)
                circle.UpdateTrail(Projectile.velocity);

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

                        float baseRot = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < trailPointCount; i++)
                        {
                            vec[i] = Projectile.Center + ((baseRot + (i * angle)).ToRotationVector2()
                                * width);
                        }

                        circle.BasePositions = vec;
                        circle.RandomThunder();
                    }
                }
            }

            if (ThunderAlpha < 1)
            {
                ThunderWidth = 16;
                ThunderAlpha += 1 / 30f;
                if (ThunderAlpha > 1)
                    ThunderAlpha = 1;
            }

            Projectile.SpawnTrailDust(30f, DustID.PortalBoltTrail, Main.rand.NextFloat(0.1f, 0.4f),
                newColor: ZacurrentDragon.ZacurrentDustPurple, Scale: Main.rand.NextFloat(1f, 1.3f));

            const int readyTime = 60;
            const int maxTime = 260;
            if (Timer == readyTime)
            {
                if (Angle == 0)
                    Angle = Main.rand.NextFloat();
                Projectile.velocity *= 0;
            }
            if (Timer > readyTime)
            {
                SpawnThunderDusts((Timer - readyTime) / maxTime);

                if (Timer % 20 == 0)
                {
                    float scale = 3 - 1.5f * (Timer - readyTime) / maxTime;
                    var p = PRTLoader.NewParticle<RoaringWave>(Projectile.Center, Vector2.Zero, ZacurrentDragon.ZacurrentPurple, 3f);
                    p.ScaleMul = 0.9f;
                }

                if (Timer > maxTime)
                {
                    Helper.PlayPitched("Electric/ElectricShoot", 0.4f, 0f, Projectile.Center);
                    SpawnThunders();
                    Projectile.Kill();
                }
            }
            Timer++;
        }

        public override void Initialize()
        {
            circles = new ThunderTrail[5];
            ATex thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB");

            for (int i = 0; i < circles.Length; i++)
            {
                if (i < 2)
                    circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Purple, GetAlpha);
                else
                    circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Pink, GetAlpha);

                circles[i].SetRange((0, 10));
                circles[i].SetExpandWidth(6);
            }

            float cacheLength = Projectile.velocity.Length() / 2;

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

        public virtual void SpawnThunderDusts(float factor)
        {
            float length = Helper.Lerp(20, 1200, factor);
            for (int i = 0; i < 5; i++)
            {
                Vector2 dir = (Angle + (i * MathHelper.TwoPi / 5)).ToRotationVector2();
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6) + (dir * Main.rand.NextFloat(20, length)), DustID.PortalBoltTrail
                    , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: ZacurrentDragon.ZacurrentDustPurple,
                    Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }
        }

        public virtual void SpawnThunders()
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 dir = (Angle + (i * MathHelper.TwoPi / 5)).ToRotationVector2();
                Vector2 pos = Projectile.Center + (dir * 40);
                Projectile.NewProjectileFromThis<PurpleElectricBallThunder>(pos + (dir * 1000), pos, Projectile.damage, 0, 15,
                    (int)OwnerIndex, 70);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Timer < 60)
            {
                Timer = 60;
                if (Angle == 0)
                    Angle = Main.rand.NextFloat();
                Projectile.velocity *= 0;
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            PRTLoader.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<LightningParticle>(), Scale: 2.5f);

            float baseRot = Main.rand.NextFloat(6.282f);
            for (int i = 0; i < 5; i++)
            {
                PRTLoader.NewParticle(Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 5)).ToRotationVector2() * Main.rand.NextFloat(20, 30))
                    , Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>());
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BallBack.Value.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition
                , Color.Black * 0.6f * ThunderAlpha, 0, Projectile.scale * 0.5f);
            Projectile.QuickDraw(Color.White, 0f);

            if (circles != null)
                foreach (var circle in circles)
                    circle.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D exTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightFog").Value;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = ZacurrentDragon.ZacurrentPurpleAlpha;
            c.A = (byte)(ThunderAlpha * 250);
            var origin = exTex.Size() / 2;
            var scale = Projectile.scale * 0.5f;

            spriteBatch.Draw(exTex, pos, null, c, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);
            spriteBatch.Draw(exTex, pos, null, c * 0.5f, Projectile.rotation - (Main.GlobalTimeWrappedHourly / 2), origin, scale, 0, 0);
        }

    }

    public class PurpleElectricBallThunder : PurpleElectricBreath
    {
        public override void SetStartPos(NPC owner)
        {
        }
    }
}
