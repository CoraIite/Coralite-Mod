using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// ai0传入闪电时间，ai1传入主人
    /// </summary>
    public class ThunderveinExchangePhaseAnmi : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Blank;

        const int DelayTime = 30;

        public ref float LightingTime => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        public ThunderTrail[] circles;
        public ThunderTrail[] outers;
        public ThunderTrail[] lightning;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 330;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool ShouldUpdatePosition() => false;

        public float ThunderWidthFunc2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth * 1.5f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinYellowAlpha, MathF.Sin(factor * MathHelper.Pi));
        }

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinOrangeAlpha, MathF.Sin(factor * MathHelper.Pi));
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner, Projectile.Kill))
                return;

            Projectile.Center = owner.Center;

            if (circles == null)
            {
                ThunderAlpha = 1;
                circles = new ThunderTrail[4];
                outers = new ThunderTrail[6];
                lightning = new ThunderTrail[4];
                Asset<Texture2D> thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB2");

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

                for (int i = 0; i < lightning.Length; i++)
                {
                    if (i < 2)
                        lightning[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        lightning[i] = new ThunderTrail(thunderTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    lightning[i].SetRange((5, 30));
                    lightning[i].SetExpandWidth(8);
                }

                foreach (var outer in outers)
                {
                    outer.CanDraw = true;
                    Vector2[] vec = new Vector2[5];
                    float length = Main.rand.NextFloat(2, 32);
                    Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * length);
                    Vector2 dir = (basePos - Projectile.Center).SafeNormalize(Vector2.Zero);
                    vec[0] = basePos;

                    for (int i = 1; i < 5; i++)
                    {
                        vec[i] = basePos + (dir * i * (Projectile.width - length) / 10);
                    }

                    outer.BasePositions = vec;
                    outer.RandomThunder();
                }

                foreach (var light in lightning)
                {
                    light.CanDraw = true;
                    Vector2[] vec = new Vector2[10];
                    float length = Projectile.width / 2;
                    float perLength = Main.rand.Next(40, 55);
                    Vector2 dir = (-1.57f + Main.rand.NextFloat(-0.6f, 0.6f)).ToRotationVector2();
                    Vector2 basePos = Projectile.Center + (dir * (Projectile.width / 2));
                    vec[0] = basePos;

                    for (int i = 1; i < 10; i++)
                    {
                        vec[i] = basePos + (dir * i * perLength);
                    }

                    light.BasePositions = vec;
                    light.RandomThunder();
                }

                foreach (var circle in circles)
                {
                    circle.CanDraw = true;
                    int trailPointCount = Main.rand.Next(10, 20);
                    Vector2[] vec = new Vector2[trailPointCount];

                    float baseRot = Main.rand.NextFloat(6.282f);
                    for (int i = 0; i < trailPointCount; i++)
                    {
                        vec[i] = Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 20)).ToRotationVector2()
                            * ((Projectile.width / 2) + Main.rand.NextFloat(-8, 8)));
                    }

                    circle.BasePositions = vec;
                    circle.RandomThunder();
                }
            }

            if (Timer > 10 && Timer % 4 == 0)
            {
                foreach (var outer in outers)
                {
                    bool canDraw = Main.rand.NextBool();
                    outer.CanDraw = canDraw;
                    if (canDraw)//从内向外散发出去的闪电
                    {
                        Vector2[] vec = new Vector2[5];
                        float length = Main.rand.NextFloat(2, 32);
                        Vector2 basePos = Projectile.Center + (Helper.NextVec2Dir() * length);
                        Vector2 dir = (basePos - Projectile.Center).SafeNormalize(Vector2.Zero);
                        vec[0] = basePos;

                        for (int i = 1; i < 5; i++)
                        {
                            vec[i] = basePos + (dir * i * (Projectile.width - length) / 10);
                        }

                        outer.BasePositions = vec;
                        outer.RandomThunder();
                    }
                }

                foreach (var light in lightning)
                {
                    bool canDraw = Main.rand.NextBool();
                    light.CanDraw = canDraw;
                    if (canDraw)//从内向外散发出去的闪电
                    {
                        Vector2[] vec = new Vector2[10];
                        float length = Projectile.width / 2;
                        float perLength = Main.rand.Next(40, 55);
                        Vector2 dir = (-1.57f + Main.rand.NextFloat(-0.6f, 0.6f)).ToRotationVector2();
                        Vector2 basePos = Projectile.Center + (dir * (Projectile.width / 2));
                        vec[0] = basePos;

                        for (int i = 1; i < 10; i++)
                        {
                            vec[i] = basePos + (dir * i * perLength);
                        }

                        light.BasePositions = vec;
                        light.RandomThunder();
                    }
                }
            }

            if (Timer > 10 && Timer % 6 == 0)
                foreach (var circle in circles)
                {
                    bool canDraw = Main.rand.NextBool();
                    circle.CanDraw = canDraw;
                    if (canDraw)//从内向外散发出去的闪电
                    {
                        int trailPointCount = Main.rand.Next(10, 20);
                        Vector2[] vec = new Vector2[trailPointCount];

                        float baseRot = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < trailPointCount; i++)
                        {
                            vec[i] = Projectile.Center + ((baseRot + (i * MathHelper.TwoPi / 20)).ToRotationVector2()
                                * ((Projectile.width / 2) + Main.rand.NextFloat(-8, 8)));
                        }

                        circle.BasePositions = vec;
                        circle.RandomThunder();
                    }
                }

            if (Timer < LightingTime)
            {
                float factor = Timer / LightingTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);

                ThunderWidth = 30 + (sinFactor * 30);
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

        public override bool PreDraw(ref Color lightColor)
        {
            if (circles != null)
                foreach (var circle in circles)
                    circle.DrawThunder(Main.instance.GraphicsDevice);
            if (lightning != null)
                foreach (var light in lightning)
                    light.DrawThunder(Main.instance.GraphicsDevice);
            if (outers != null)
                foreach (var outer in outers)
                    outer.DrawThunder(Main.instance.GraphicsDevice);
            return false;
        }
    }
}
