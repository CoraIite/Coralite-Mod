using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// ai0传入闪电时间，ai1传入主人
    /// </summary>
    public class DischargingBurst : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Blank;

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

        public override bool ShouldUpdatePosition() => false;

        public float ThunderWidthFunc(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth;
        }

        public float ThunderWidthFunc2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth*1.5f;
        }

        public Color ThunderColorFunc(float factor)
        {
            return new Color(255, 202, 101, 0) * ThunderAlpha;
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

            const int DelayTime = 30;
            if (circles == null)
            {
                ThunderAlpha = 1;
                circles = new ThunderTrail[4];
                outers = new ThunderTrail[6];
                Asset<Texture2D> thunderTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LaserBody2");

                for (int i = 0; i < circles.Length; i++)
                {
                    circles[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc);
                    circles[i].SetRange((5, 10));
                    circles[i].SetExpandWidth(8);
                }

                for (int i = 0; i < outers.Length; i++)
                {
                    outers[i] = new ThunderTrail(thunderTex, ThunderWidthFunc, ThunderColorFunc);
                    outers[i].SetRange((5, 30));
                    outers[i].SetExpandWidth(8);
                }

                foreach (var outer in outers)
                {
                    outer.CanDraw = true;
                    Vector2[] vec = new Vector2[6];
                    float length = Main.rand.NextFloat(2, 32);
                    Vector2 basePos = Projectile.Center + Helper.NextVec2Dir() * length;
                    Vector2 dir = (basePos - Projectile.Center).SafeNormalize(Vector2.Zero);
                    vec[0] = basePos;

                    for (int i = 1; i < 6; i++)
                    {
                        vec[i] = basePos + dir * i * (Projectile.width - length) / 10;
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
                        vec[i] = Projectile.Center + (baseRot + i * MathHelper.TwoPi / 40).ToRotationVector2()
                            * (Projectile.width / 2 + Main.rand.NextFloat(-8, 8));
                    }

                    circle.BasePositions = vec;
                    circle.RandomThunder();
                }
            }

            if (Timer > 10 && Timer % 4 == 0)
                foreach (var outer in outers)
                {
                    bool canDraw = Main.rand.NextBool();
                    outer.CanDraw = canDraw;
                    if (canDraw)//从内向外散发出去的闪电
                    {
                        Vector2[] vec = new Vector2[6];
                        float length = Main.rand.NextFloat(2, 32);
                        Vector2 basePos = Projectile.Center + Helper.NextVec2Dir() * length;
                        Vector2 dir = (basePos - Projectile.Center).SafeNormalize(Vector2.Zero);
                        vec[0] = basePos;

                        for (int i = 1; i < 6; i++)
                        {
                            vec[i] = basePos + dir * i * (Projectile.width - length) / 10;
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
                            vec[i] = Projectile.Center + (baseRot + i * MathHelper.TwoPi / 40).ToRotationVector2()
                                * (Projectile.width / 2 + Main.rand.NextFloat(-8, 8));
                        }

                        circle.BasePositions = vec;
                        circle.RandomThunder();
                    }
                }

            if (Timer < LightingTime)
            {
                float factor = Timer  / LightingTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);

                ThunderWidth = 30 + sinFactor * 30;
            }
            else
            {
                float factor = (Timer - LightingTime) / DelayTime;
                float x2Factor = Coralite.Instance.X2Smoother.Smoother(factor);

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
            if (outers != null)
                foreach (var outer in outers)
                    outer.DrawThunder(Main.instance.GraphicsDevice);
            return false;
        }
    }
}
