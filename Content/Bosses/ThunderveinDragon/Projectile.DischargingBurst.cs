using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// ai0传入闪电时间，ai1传入主人
    /// </summary>
    public class DischargingBurst : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Blank;

        const int DelayTime = 30;

        public ref float LightingTime => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        public ThunderTrail[] circles;
        public ThunderTrail[] outers;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 630;
        }

        public override bool? CanDamage()
        {
            if (Timer > LightingTime + (DelayTime / 2))
                return false;

            return null;
        }

        public override bool ShouldUpdatePosition() => false;

        public float ThunderWidthFunc2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth * 1.5f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Vector2.Distance(projHitbox.Center.ToVector2(), targetHitbox.Center.ToVector2()) < (Projectile.width / 2);
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner, Projectile.Kill))
                return;

            Projectile.Center = owner.Center;

            InitTrails();
            UpdateTrails();

            if (Timer < LightingTime)
            {
                float factor = Timer / LightingTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);

                ThunderWidth = 30 + (sinFactor * 30);
                SpawnDusts();
            }
            else
            {
                float factor = (Timer - LightingTime) / DelayTime;
                float x2Factor = Helper.X2Ease(factor);

                ThunderWidth = (1 - x2Factor) * 30;
                ThunderAlpha = 1 - x2Factor;
                if (Timer > LightingTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        public void InitTrails()
        {
            if (circles == null)
            {
                ThunderAlpha = 1;
                circles = new ThunderTrail[4];
                outers = new ThunderTrail[6];
                Asset<Texture2D> thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");

                for (int i = 0; i < circles.Length; i++)
                {
                    if (i == 0)
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc_Yellow, GetAlpha);

                    circles[i].SetRange((5, 10));
                    circles[i].SetExpandWidth(8);
                }

                for (int i = 0; i < outers.Length; i++)
                {
                    if (i < 2)
                        outers[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        outers[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    outers[i].SetRange((5, 30));
                    outers[i].SetExpandWidth(8);
                }

                foreach (var outer in outers)
                {
                    outer.CanDraw = true;
                    Vector2[] vec = new Vector2[6];
                    float length = Main.rand.NextFloat(2, 32);
                    Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * length);
                    Vector2 dir = (basePos - Projectile.Center).SafeNormalize(Vector2.Zero);
                    vec[0] = basePos;

                    for (int i = 1; i < 6; i++)
                    {
                        vec[i] = basePos + (dir * i * (Projectile.width - length) / 10);
                    }

                    outer.BasePositions = vec;
                    outer.RandomThunder();
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
            if (Timer > 10 && Timer % 4 == 0)
                foreach (var outer in outers)
                {
                    bool canDraw = Main.rand.NextBool();
                    outer.CanDraw = canDraw;
                    if (canDraw)//从内向外散发出去的闪电
                    {
                        Vector2[] vec = new Vector2[6];
                        float length = Main.rand.NextFloat(2, 32);
                        Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * length);
                        Vector2 dir = (basePos - Projectile.Center).SafeNormalize(Vector2.Zero);
                        vec[0] = basePos;

                        for (int i = 1; i < 6; i++)
                        {
                            vec[i] = basePos + (dir * i * (Projectile.width - length) / 10);
                        }

                        outer.BasePositions = vec;
                        outer.RandomThunder();
                    }
                }

            if (Timer > 10 && Timer % 6 == 0)
                foreach (var circle in circles)
                {
                    bool canDraw = Main.rand.NextBool();
                    circle.CanDraw = canDraw;
                    if (canDraw)//从内向外散发出去的闪电
                    {
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

        public virtual void SpawnDusts()
        {
            if (Timer % 2 == 0)
            {
                PRTLoader.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 5, Projectile.width / 5),
                    Vector2.Zero, CoraliteContent.ParticleType<BigFog>(), Coralite.ThunderveinYellow * Main.rand.NextFloat(0.5f, 0.8f),
                    Main.rand.NextFloat(2f, 2.5f));
                ElectricParticle_Follow.Spawn(Projectile.Center, Helper.NextVec2Dir(Projectile.width / 3, Projectile.width * 0.56f), () => Projectile.Center, Main.rand.NextFloat(0.7f, 1.1f));
            }
            else
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.width / 3)
                    , ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.3f, 0.5f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (circles != null)
                foreach (var circle in circles)
                    circle.DrawThunder(Main.instance.GraphicsDevice);
            if (outers != null)
                foreach (var outer in outers)
                    outer.DrawThunder(Main.instance.GraphicsDevice);
            return false;
        }
    }

    public class StrongDischargingBurst : DischargingBurst
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
}
