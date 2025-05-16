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
    public class PurpleSmallThunderFall : BaseZacurrentProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float ReadyTime => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        const int DelayTime = 20;
        public float fade = 0;

        protected ThunderTrail[] thunderTrails;
        protected Vector2[] randPos;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
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
            if (Timer < ReadyTime || Timer > ReadyTime + (DelayTime / 2))
                return false;

            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //竖直向上的碰撞箱
            Vector2 dir = -Vector2.UnitY;
            return targetHitbox.Intersects(Utils.CenteredRectangle(Projectile.Center + dir * 800, new Vector2(50, 1600)));
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ZacurrentDragon>(out NPC owner, Projectile.Kill))
                return;

            if (Timer < ReadyTime)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dir = -Vector2.UnitY;
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6) + (dir * Main.rand.NextFloat(20, 1600)), DustID.PortalBoltTrail
                        , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 6f), newColor: ZacurrentDragon.ZacurrentDustPurple,
                        Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

                Lighting.AddLight(Projectile.velocity, ZacurrentDragon.ZacurrentPurple.ToVector3());
            }
            else if (Timer == ReadyTime)
            {
                InitThunder();
            }
            else
            {
                SpawnDusts();

                float factor = (Timer - ReadyTime) / DelayTime;
                fade = Coralite.Instance.X2Smoother.Smoother(factor);

                ThunderWidth = 20 * (1 - factor);
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

                if (Timer > ReadyTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        public void InitThunder()
        {
            Projectile.Resize((int)PointDistance, 40);

            thunderTrails = new ThunderTrail[3];
            randPos = new Vector2[3];

            Vector2 pos2 = Projectile.Center + new Vector2(0, -1600);
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
                        pos.Add(pos2);
                }

            for (int i = 0; i < 3; i++)
            {
                thunderTrails[i] = new ThunderTrail(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB2")
                    , f => MathF.Sqrt(f) * ThunderWidth, i == 0 ? ThunderColorFunc2_Pink : ThunderColorFunc_Purple, GetAlpha);
                thunderTrails[i].CanDraw = true;
                thunderTrails[i].SetRange((5, 20));
                thunderTrails[i].SetExpandWidth(4);
                thunderTrails[i].BasePositions = [.. pos];

                randPos[i] = Main.rand.NextVector2CircularEdge(30, 30);
            }
        }

        public virtual void SpawnDusts()
        {
            if (Main.rand.NextBool())
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center - new Vector2(0, 1500), Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
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
            if (Timer < ReadyTime)
            {
                Vector2 p = Projectile.Center - Main.screenPosition;
                float factor = Timer / ReadyTime;
                float factor2 = Coralite.Instance.SqrtSmoother.Smoother(factor);
                float rot = Projectile.whoAmI + MathHelper.TwoPi / 3 + factor2 * MathHelper.TwoPi;
                float scale = 0.3f - factor * 0.3f;
                CoraliteAssets.Sparkle.CrossSPA.Value.QuickCenteredDraw(Main.spriteBatch, p
                    , ZacurrentDragon.ZacurrentPurple, rot, scale);
                CoraliteAssets.Sparkle.CrossSPA.Value.QuickCenteredDraw(Main.spriteBatch, p
                    , Color.White, rot, scale * 0.6f);
            }

            if (thunderTrails != null)
                foreach (var trail in thunderTrails)
                    trail?.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }
    }
}
