using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// ai0记录持有者，ai1记录前摇时间，需要传入owner作为目标
    /// </summary>
    public class AimThunderBall : BaseZacurrentProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "LightingBall";

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];

        public float State;

        public ThunderTrail[] circles;
        public ThunderTrail[] trails;

        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.scale = 0.3f;
            Projectile.width = Projectile.height = 110;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
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

        public override bool? CanDamage()
        {
            if (State==0)
                return false;
            return base.CanDamage();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            UpdateThunder();
            RandTrail();

            UpdateCircle();

            switch (State)
            {
                default:
                case 0://跟随本体一起动
                    {
                        if (!OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner, Projectile.Kill))
                            return;

                        Projectile.Center += owner.position - owner.oldPosition;
                        Projectile.velocity *= 0.96f;

                        Timer++;
                        if (Timer > MaxTime)
                        {
                            State = 1;
                            Timer = 0;
                            Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 6;
                            Projectile.tileCollide = true;
                        }
                    }
                    break;
                case 1:
                    {
                        Timer++;
                        if (Timer > 30)
                        {
                            Projectile.SpawnTrailDust(30f, DustID.PortalBoltTrail, Main.rand.NextFloat(0.1f, 0.4f),
                                newColor: ZacurrentDragon.ZacurrentDustPurple, Scale: Main.rand.NextFloat(1f, 1.3f));
                            ThunderWidth = Main.rand.NextFloat(20, 30);

                            if (Projectile.velocity.Length() < 27)
                                Projectile.velocity *= 1.04f;
                        }
                    }
                    break;
            }

            if (ThunderAlpha < 1)
            {
                ThunderWidth = 16;
                ThunderAlpha += 1 / 30f;
                if (ThunderAlpha > 1)
                    ThunderAlpha = 1;
            }
        }

        private void UpdateCircle()
        {
            if (Timer % 4 == 0)
            {
                ElectricParticle_PurpleFollow.Spawn(Projectile.Center, Main.rand.NextVector2Circular(30, 30),
                    () => Projectile.Center, Main.rand.NextFloat(0.5f, 0.75f));

                foreach (var circle in circles)
                {
                    circle.CanDraw = Main.rand.NextBool();
                    if (circle.CanDraw)
                    {
                        int width = Main.rand.Next(Projectile.width / 3, Projectile.width / 2);
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
        }

        private void UpdateThunder()
        {
            foreach (var circle in circles)
                circle.UpdateTrail(Projectile.velocity);
            foreach (var trail in trails)
                trail.UpdateTrail(Projectile.velocity);
        }

        private void RandTrail()
        {
            if (Timer % 5 == 0)
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
        }

        public float ThunderWidthFunc_Sin2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth / 2;
        }


        public override void Initialize()
        {
            circles = new ThunderTrail[5];
            trails = new ThunderTrail[3];
            ATex thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");

            for (int i = 0; i < circles.Length; i++)
            {
                if (i < 2)
                    circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin2, ThunderColorFunc_Purple, GetAlpha);
                else
                    circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin2, ThunderColorFunc2_Pink, GetAlpha);

                circles[i].SetRange((0, 5));
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
            PurpleElectricBall.BallBack.Value.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition
                , Color.Black * 0.6f * ThunderAlpha, 0, Projectile.scale * 0.5f);
            Projectile.QuickDraw(Color.White, 0f);

            if (circles != null)
                foreach (var circle in circles)
                    circle.DrawThunder(Main.instance.GraphicsDevice);
            if (State > 0 && trails != null)
                foreach (var trail in trails)
                    trail.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }

        public virtual void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D exTex = CoraliteAssets.Halo.LightFog.Value;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = ZacurrentDragon.ZacurrentPinkAlpha;
            c.A = (byte)(ThunderAlpha * 250);
            var origin = exTex.Size() / 2;
            var scale = Projectile.scale * 0.5f;

            spriteBatch.Draw(exTex, pos, null, c, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);
            spriteBatch.Draw(exTex, pos, null, c * 0.5f, Projectile.rotation - (Main.GlobalTimeWrappedHourly / 2), origin, scale, 0, 0);
        }
    }

    public class AimThunderBallRed: AimThunderBall
    {
        public override string Texture => AssetDirectory.ZacurrentDragon + "RedVoltBall";

        public override Color ThunderColorFunc_Purple(float factor)
        {
            return Color.Lerp(ZacurrentDragon.ZacurrentPurple, ZacurrentDragon.ZacurrentRed, factor);
        }

        public override Color ThunderColorFunc2_Pink(float factor)
        {
            return ZacurrentDragon.ZacurrentRed;
        }

        public override void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D exTex = CoraliteAssets.Halo.LightFog.Value;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = ZacurrentDragon.ZacurrentRed;
            c.A = (byte)(ThunderAlpha * 250);
            var origin = exTex.Size() / 2;
            var scale = Projectile.scale * 0.5f;

            spriteBatch.Draw(exTex, pos, null, c, Projectile.rotation + Main.GlobalTimeWrappedHourly, origin, scale, 0, 0);
            spriteBatch.Draw(exTex, pos, null, c * 0.5f, Projectile.rotation - (Main.GlobalTimeWrappedHourly / 2), origin, scale, 0, 0);
        }
    }
}
