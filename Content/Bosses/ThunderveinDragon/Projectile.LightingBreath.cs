﻿using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// 使用ai0传入冲刺时间，ai1传入主人
    /// 使用ai2传入闪电每个点间的间隔
    /// 使用速度传入中心点的位置，位置传入末端的位置
    /// </summary>
    public class LightingBreath : LightningDash
    {
        const int DelayTime = 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2800;
        }

        public override bool? CanDamage()
        {
            if (Timer > DashTime + (DelayTime / 2))
                return false;

            return null;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner, Projectile.Kill))
                return;

            if (thunderTrails == null)
            {
                Projectile.Resize((int)PointDistance, 40);

                thunderTrails = new ThunderTrail[3];
                for (int i = 0; i < 3; i++)
                {
                    thunderTrails[i] = new ThunderTrail(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB2")
                        , ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].SetRange((5, 20));
                    thunderTrails[i].BasePositions =
                    [
                    Projectile.Center,Projectile.Center,Projectile.Center
                    ];
                }
            }

            if (Timer < DashTime)
            {
                SpawnDusts();
                Vector2 pos2 = Projectile.velocity;
                Vector2 normal = (Projectile.velocity - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(1.57f);

                List<Vector2> pos = new()
                {
                    Projectile.velocity
                };
                if (Vector2.Distance(Projectile.velocity, Projectile.Center) < PointDistance)
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
                        {
                            float f1 = (float)Main.timeForVisualEffects * 0.5f;
                            float f2 = i * 0.4f;
                            float factor2 = MathF.Sin(f1 + f2) + MathF.Cos(f2 + (f1 / 2));
                            pos.Add(pos2 + (normal * factor2 * 8));
                        }
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
                        trail.RandomThunder();
                    }
                }

                float factor = Timer / DashTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);

                ThunderWidth = 30 + (sinFactor * 30);
                ThunderAlpha = Timer / DashTime;
            }
            else if ((int)Timer == (int)DashTime)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
            else
            {
                float factor = (Timer - DashTime) / DelayTime;
                ThunderWidth = 30 * (1 - factor);
                ThunderAlpha = 1 - Coralite.Instance.X2Smoother.Smoother(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange(GetRange(factor));
                    trail.SetExpandWidth(GetExpandWidth(factor));

                    if (Timer % 6 == 0)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                if (Timer > DashTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        public override void SpawnDusts()
        {
            if (Main.rand.NextBool())
            {
                Vector2 pos = Vector2.Lerp(Projectile.velocity, Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
                    + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
                if (Main.rand.NextBool())
                {
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
                }
                else
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.3f));
                }
            }
        }

        public virtual (float, float) GetRange(float factor)
        {
            float sinFactor = MathF.Sin(factor * MathHelper.Pi);

            return (5, 20 + (sinFactor * PointDistance / 2));
        }

        public virtual float GetExpandWidth(float factor)
        {
            return (1 - factor) * PointDistance / 3;
        }
    }

    /// <summary>
    /// 使用ai0传入冲刺时间，ai1传入主人
    /// 使用ai2传入闪电每个点间的间隔
    /// 使用速度传入中心点的位置，位置传入末端的位置
    /// </summary>
    public class StrongerLightingBreath : LightingBreath
    {
        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinYellowAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinOrangeAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }

        public override (float, float) GetRange(float factor)
        {
            return (0, 5 + ((1 - factor) * PointDistance / 3));
        }

        public override float GetExpandWidth(float factor)
        {
            return (1 - factor) * PointDistance / 4;
        }
    }
}
