using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// 使用ai0控制持续时间
    /// </summary>
    public class PurpleGravitationThunderBall : BaseZacurrentProj, IPostDrawAdditive
    {
        public override string Texture => AssetDirectory.ZacurrentDragon + "LightingBall";

        public ThunderTrail[] circles;
        public ThunderTrail[] trails;

        public const int chasingTime = 70;

        public ref float MaxTime => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];

        private float thunderRange = 110;
        private float selfAlpha;
        private float selfScale;
        public bool canDrawThunder = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 110;
            Projectile.hostile = true;
            Projectile.timeLeft = 60 * 12;
            Projectile.tileCollide = false;
        }

        public float ThunderWidthFunc2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth * 1.2f;
        }

        public float GetAlphaFade(float factor)
        {
            return ThunderAlpha * (1 - factor);
        }

        public override Color ThunderColorFunc_Purple(float factor)
        {
            return Color.Lerp(ZacurrentDragon.ZacurrentPurple, ZacurrentDragon.ZacurrentPink, MathF.Sin(factor * MathHelper.Pi));
        }

        public override Color ThunderColorFunc2_Pink(float factor)
        {
            return Color.Lerp(ZacurrentDragon.ZacurrentPink, ZacurrentDragon.ZacurrentPurple, MathF.Sin(factor * MathHelper.Pi));
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            UpdateThunder();

            Timer++;
            if (ThunderAlpha < 1)
            {
                ThunderWidth = 18;
                ThunderAlpha += 1 / 10f;
                if (ThunderAlpha > 1)
                    ThunderAlpha = 1;
            }

            Player player = Main.player[Projectile.owner];
            if (Timer < chasingTime)
                Chase(player);
            else if (Timer == chasingTime)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, Projectile.Center);
                ThunderAlpha = 1;
                Projectile.velocity *= 0;
                thunderRange = 60;
            }
            else if (Timer < chasingTime + 50)
            {
                float factor = (Timer - chasingTime) / 50f;
                float length = Helper.Lerp(80, 1400, factor);

                for (int i = 0; i < 5; i++)
                {
                    PRTLoader.NewParticle(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length),
                        Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(), Scale: Main.rand.NextFloat(0.9f, 1.3f));
                }
            }
            else if (Timer == chasingTime + 50)
            {
                float factor = (Timer - chasingTime) / 50f;
                float length = Helper.Lerp(80, 1400, factor);

                for (int i = 0; i < 5; i++)
                {
                    PRTLoader.NewParticle(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length),
                        Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(), Scale: Main.rand.NextFloat(0.9f, 1.3f));
                }
            }
            else
            {
                if (selfAlpha < 1)
                {
                    selfAlpha += 1 / 10f;
                    selfScale += 0.5f / 10f;
                }
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(80, 80),
                        ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ZacurrentDragon.ZacurrentPurple, Scale: Main.rand.NextFloat(0.1f, 0.3f));
                }

                player.velocity += (Projectile.Center - player.Center).SafeNormalize(Vector2.Zero) * 0.15f;

                //吸玩家
                if (Timer % 45 == 0)
                {
                    player.velocity += (Projectile.Center - player.Center).SafeNormalize(Vector2.Zero) * 3f;

                    for (int i = 0; i < 35; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir(80, 200);
                        Dust.NewDustPerfect(Projectile.Center + dir,
                            ModContent.DustType<LightningShineBall>(), -dir.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3, 7), newColor: ZacurrentDragon.ZacurrentPurple, Scale: Main.rand.NextFloat(0.1f, 0.3f));
                    }
                }

                if (Timer > chasingTime + 50 + MaxTime)
                    Projectile.Kill();
            }
        }

        private void Chase(Player player)
        {
            Projectile.velocity += (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 2;
            if (Projectile.velocity.Length() > 15)
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 15;

            if (Vector2.Distance(Projectile.Center, player.Center) < 250)
            {
                Projectile.velocity *= 0.7f;
            }

            thunderRange = Helper.Lerp(110, 30, Timer / chasingTime);
            Projectile.SpawnTrailDust(30f, DustID.PortalBoltTrail, Main.rand.NextFloat(0.1f, 0.4f),
                newColor: ZacurrentDragon.ZacurrentDustPurple, Scale: Main.rand.NextFloat(1f, 1.3f));
        }

        public override void Initialize()
        {
            circles = new ThunderTrail[5];
            trails = new ThunderTrail[3];
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

            for (int i = 0; i < trails.Length; i++)
            {
                trails[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc_Purple, GetAlphaFade);
                trails[i].SetRange((0, 6));
                trails[i].SetExpandWidth(4);
            }

            float cacheLength = Projectile.velocity.Length() / 2;
            foreach (var trail in trails)
            {
                if (cacheLength < 3)
                    trail.CanDraw = false;
                else
                {
                    Vector2[] vec = new Vector2[(int)cacheLength];
                    Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * 5);
                    Vector2 dir = -Projectile.velocity;
                    vec[0] = basePos;

                    for (int i = 1; i < (int)cacheLength; i++)
                    {
                        vec[i] = basePos + (dir * i);
                    }

                    trail.BasePositions = vec;
                    trail.RandomThunder();
                }
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

        public void UpdateThunder()
        {
            foreach (var circle in circles)
                circle.UpdateTrail(Projectile.velocity);
            foreach (var trail in trails)
                trail.UpdateTrail(Projectile.velocity);

            if (Timer < chasingTime && Timer % 5 == 0)
            {
                float cacheLength = Projectile.velocity.Length() * 0.55f;

                foreach (var trail in trails)
                {
                    if (cacheLength < 3)
                        trail.CanDraw = false;
                    else
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        if (trail.CanDraw)
                        {
                            Vector2[] vec = new Vector2[(int)cacheLength];
                            Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * 10);
                            Vector2 dir = -Projectile.velocity * 0.55f;
                            vec[0] = basePos;

                            for (int i = 1; i < (int)cacheLength; i++)
                            {
                                vec[i] = basePos + (dir * i);
                            }

                            trail.BasePositions = vec;
                            trail.RandomThunder();
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
                        int width = (int)thunderRange;
                        float angle = MathHelper.TwoPi / (5 + (30 * Helper.Lerp(0, 1, width / (float)Projectile.width)));
                        int trailPointCount = (int)thunderRange / 5;
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
            PurpleElectricBall.BallBack.Value.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition
                , Color.Black * 0.75f * ThunderAlpha, 0, Projectile.scale * 0.4f);
            Projectile.QuickDraw(Color.White, 0f);

            if (!canDrawThunder)
                return false;

            if (circles != null)
                foreach (var circle in circles)
                    circle.DrawThunder(Main.instance.GraphicsDevice);
            if (Timer < chasingTime && trails != null)
                foreach (var trail in trails)
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

            spriteBatch.Draw(exTex, pos, null, c, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);
            spriteBatch.Draw(exTex, pos, null, c * 0.5f, Projectile.rotation - (Main.GlobalTimeWrappedHourly / 2), origin, scale, 0, 0);
        }
    }
}
