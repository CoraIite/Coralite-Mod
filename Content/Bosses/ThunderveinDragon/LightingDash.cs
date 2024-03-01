using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// 使用ai0传入冲刺时间，ai1传入主人
    /// 使用ai2传入闪电每个点间的间隔
    /// </summary>
    public class LightingDash : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float DashTime => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[1];
        public ref float PointDistance => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float ThunderWidth => ref Projectile.localAI[1];
        public ref float ThunderAlpha => ref Projectile.localAI[2];

        private ThunderTrail[] thunderTrails;

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 40;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Resize((int)PointDistance, 40);
            Projectile.velocity = Projectile.Center;
            thunderTrails = new ThunderTrail[3];
            for (int i = 0; i < 3; i++)
            {
                thunderTrails[i] = new ThunderTrail(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LaserBody2")
                    , ThunderWidthFunc, ThunderColorFunc);
                thunderTrails[i].CanDraw = false;
                thunderTrails[i].SetRange((5, 20));
                thunderTrails[i].BasePositions = new Vector2[3]
                {
                    Projectile.Center,Projectile.Center,Projectile.Center
                };
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),Projectile.velocity,Projectile.Center,Projectile.width,ref a);
        }

        public float ThunderWidthFunc(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth;
        }

        public Color ThunderColorFunc(float factor)
        {
            return new Color(255, 202, 101, 0) * ThunderAlpha;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner, Projectile.Kill))
                return;

            const int DelayTime = 30;

            if (Timer < DashTime)
            {
                Projectile.Center = owner.Center;
                Vector2 pos2 = Projectile.velocity;
                List<Vector2> pos = new List<Vector2>
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
                        trail.RandomThunder();
                    }
                }

                ThunderWidth = 20;
                ThunderAlpha = Timer / DashTime;
            }
            else if ((int)Timer==(int)DashTime)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
            else
            {
                float factor = (Timer - DashTime) / (DelayTime);
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                ThunderWidth = 20 + sinFactor * 30;
                ThunderAlpha = 1 - Coralite.Instance.X2Smoother.Smoother(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange((5, 25 + sinFactor * PointDistance / 2));
                    trail.SetExpandWidth((1 - factor) * PointDistance / 2);

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

        public override bool PreDraw(ref Color lightColor)
        {
            if (thunderTrails!=null)
            {
                foreach (var trail in thunderTrails)
                {
                    trail?.DrawThunder(Main.spriteBatch);
                }
            }
            return false;
        }
    }
}
