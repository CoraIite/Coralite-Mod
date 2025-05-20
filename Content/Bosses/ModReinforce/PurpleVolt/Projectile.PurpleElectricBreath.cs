using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public class PurpleElectricBreath : BaseZacurrentProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float DashTime => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        const int DelayTime = 20;
        public float fade = 0;

        protected ThunderTrail[] thunderTrails;
        protected Vector2[] randPos;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2800;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage()
        {
            if (Timer > DashTime + (DelayTime / 2))
                return false;

            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.velocity, Projectile.Center, Projectile.width, ref a);
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner, Projectile.Kill))
                return;

            if (Timer < DashTime)
            {
                SpawnDusts();
                SetStartPos(owner);
                Lighting.AddLight(Projectile.velocity, ZacurrentDragon.ZacurrentPurple.ToVector3());

                Vector2 pos2 = Projectile.velocity;
                Vector2 normal = (Projectile.velocity - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(1.57f);

                List<Vector2> pos = new()
                {
                    pos2
                };
                if (Vector2.Distance(pos2, Projectile.Center) < PointDistance)
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

                for (int i = 0; i < thunderTrails.Length; i++)
                {
                    var trail = thunderTrails[i];
                    trail.BasePositions = [.. pos];
                    trail.BasePositions[^1] += randPos[i];
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

                ThunderWidth = 20 + (sinFactor * 20);
                ThunderAlpha = factor;
            }
            else
            {
                float factor = (Timer - DashTime) / DelayTime;
                fade = Helper.X2Ease(factor);

                ThunderWidth = 20 * (1 - factor);
                ThunderAlpha = 1 - Helper.X2Ease(factor);

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

        public virtual void SetStartPos(NPC owner)
        {
            Projectile.velocity = (owner.ModNPC as ZacurrentDragon).GetMousePos();
        }

        public override void Initialize()
        {
            Projectile.Resize((int)PointDistance, 40);

            thunderTrails = new ThunderTrail[3];
            randPos = new Vector2[3];
            for (int i = 0; i < 3; i++)
            {
                thunderTrails[i] = new ThunderTrail(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB2")
                    , f => MathF.Sqrt(f) * ThunderWidth, i == 0 ? ThunderColorFunc2_Pink : ThunderColorFunc_Purple, GetAlpha);
                thunderTrails[i].CanDraw = false;
                thunderTrails[i].SetRange((5, 20));
                thunderTrails[i].SetExpandWidth(4);
                thunderTrails[i].BasePositions =
                [
                Projectile.Center,Projectile.Center,Projectile.Center
                ];

                randPos[i] = Main.rand.NextVector2CircularEdge(30,30);
            }
        }

        public virtual void SpawnDusts()
        {
            if (Main.rand.NextBool())
            {
                Vector2 pos = Vector2.Lerp(Projectile.velocity, Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
                    + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
                ZacurrentDragon.PurpleElectricParticle(pos);
            }
        }

        public override float GetAlpha(float factor)
        {
            if (factor < fade)
                return 0;

            return ThunderAlpha * (factor - fade) / (1 - fade);
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

        public override bool PreDraw(ref Color lightColor)
        {
            if (thunderTrails != null)
                foreach (var trail in thunderTrails)
                    trail?.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }
    }
}
