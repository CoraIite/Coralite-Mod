using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public class PurpleDash : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float DashTime => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[1];
        public ref float Purple => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];
        public int State;

        public float fade = 0;

        const int DelayTime = 30;

        protected ThunderTrail[] thunderTrails;
        protected LinkedList<Vector2>[] points;

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

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

        public override float GetAlpha(float factor)
        {
            if (factor < fade)
                return 0;

            return ThunderAlpha * (factor - fade) / (1 - fade);
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner, Projectile.Kill))
                return;

            if (thunderTrails == null)
                InitTrails();

            switch (State)
            {
                default:
                case 0://闪电生成并跟踪
                    break;
                case 1://闪电逐渐消失
                    break;
            }

            if (Timer < DashTime)
            {
                SpawnDusts();
                Projectile.Center = owner.Center;
                Vector2 pos2 = Projectile.velocity;
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
                            pos.Add(pos2);
                    }

                foreach (var trail in thunderTrails)
                {
                    pos[0] = Projectile.velocity + Main.rand.NextVector2Circular(24, 24);
                    pos[^1] = Projectile.Center + Main.rand.NextVector2Circular(24, 24);

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
                SpawnDusts();

                float factor = (Timer - DashTime) / DelayTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                ThunderWidth = 20 + (sinFactor * 30);
                ThunderAlpha = 1 - Coralite.Instance.X2Smoother.Smoother(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange((0, 12 + (sinFactor * PointDistance / 2)));
                    trail.SetExpandWidth((1 - factor) * PointDistance / 3);

                    if (Timer % 6 == 0)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                fade = Coralite.Instance.X2Smoother.Smoother((int)(Timer - DashTime), DelayTime);

                if (Timer > DashTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        public void InitTrails()
        {
                Projectile.Resize((int)PointDistance, 40);
                Projectile.velocity = Projectile.Center;
                thunderTrails = new ThunderTrail[3];
                Asset<Texture2D> trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].SetRange((0, 10));
                    thunderTrails[i].BasePositions =
                    [
                        Projectile.Center,Projectile.Center,Projectile.Center
                    ];
                }
        }

        public void UpdateThunder()
        {

        }

        public virtual void SpawnDusts()
        {
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = Vector2.Lerp(Projectile.velocity, Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
                    + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
                if (Main.rand.NextBool())
                {
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
                }
                else
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ZacurrentDragon.ThunderveinYellow, Scale: Main.rand.NextFloat(0.1f, 0.3f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (thunderTrails != null)
            {
                foreach (var trail in thunderTrails)
                {
                    trail?.DrawThunder(Main.instance.GraphicsDevice);
                }
            }
            return false;
        }
    }
}
