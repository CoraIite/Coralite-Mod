using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// 使用ai2传入一共多少点
    /// </summary>
    public class PurpleDash : BaseZacurrentProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float DashTime => ref Projectile.ai[0];
        public ref float OwnerIndex => ref Projectile.ai[1];

        public ref float Timer => ref Projectile.localAI[0];
        public int State;

        public float fade = 0;

        const int DelayTime = 30;

        protected ThunderTrail[] thunderTrails;
        protected LinkedList<Vector2>[] points;
        protected Vector2[] Offsets;

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
            if (State > 0)
                return false;

            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State > 0)
                return false;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (targetHitbox.Intersects(Utils.CenteredRectangle(Projectile.oldPos[i], Projectile.Size)))
                    return true;
            }

            return false;
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

            Projectile.velocity = owner.velocity;

            switch (State)
            {
                default:
                case 0://闪电生成并跟踪
                    {
                        SpawnDusts();
                        Projectile.Center = owner.Center;

                        if (!VaultUtils.isServer)
                            UpdateThunder();

                        ThunderWidth = 30;
                        ThunderAlpha = Timer / DashTime;

                        Timer++;
                        if (Timer > DashTime)
                        {
                            Timer = 0;
                            State = 1;
                            //for (int i = 0; i < thunderTrails.Length; i++)
                            //{
                            //    //thunderTrails[i].CanDraw = i % 2 == 0;
                            //    thunderTrails[i].RandomThunder();
                            //}
                        }
                    }
                    break;
                case 1://闪电逐渐消失
                    {
                        //SpawnDusts();

                        if (!VaultUtils.isServer)
                        {
                            float factor = Timer / DelayTime;
                            float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                            ThunderWidth = 30 + (sinFactor * 40);
                            ThunderAlpha = 1 - Helper.X2Ease(factor);

                            if (Timer % 6 == 0)
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

                            fade = Helper.X2Ease(factor);
                        }

                        Timer++;
                        if (Timer > DelayTime)
                            Projectile.Kill();
                    }
                    break;
            }

            Projectile.UpdateOldPosCache();
        }

        public override void Initialize()
        {
            Projectile.Resize(110, 110);
            Projectile.InitOldPosCache((int)PointDistance);

            if (!VaultUtils.isServer)
            {
                const int trailCount = 4;
                thunderTrails = new ThunderTrail[trailCount];
                points = new LinkedList<Vector2>[trailCount];
                ATex trailTex = CoraliteAssets.Laser.LightingBody2;
                Offsets = new Vector2[trailCount];

                for (int i = 0; i < trailCount; i++)
                {
                    if (i == 0)
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Pink, GetAlpha);
                    else
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc_Purple, GetAlpha);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].PartitionPointCount = 3;
                    thunderTrails[i].SetRange((4, 12));
                    thunderTrails[i].SetExpandWidth(8);
                    thunderTrails[i].BasePositions =
                    [
                        Projectile.Center,Projectile.Center
                    ];

                    points[i] = new();
                    points[i].AddLast(Projectile.Center);
                    points[i].AddLast(Projectile.Center);

                    Offsets[i] = Main.rand.NextVector2CircularEdge(Projectile.width / 2, Projectile.height / 2);
                }
            }
        }

        public void UpdateThunder()
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].AddLast(Projectile.Center + Offsets[i]);
                if (points[i].Count > PointDistance)
                    points[i].RemoveFirst();

                thunderTrails[i].BasePositions = [.. points[i]];
            }

            if (Timer % 5 == 0)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }

            if (Timer != 0 && Timer % 3 == 0)
            {
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i < Offsets.Length; i++)
                {
                    Offsets[i] = dir.RotateByRandom(-MathHelper.PiOver4, MathHelper.PiOver4) * Main.rand.NextFloat(Projectile.width / 4, Projectile.width / 2);
                }
            }
        }

        public virtual void SpawnDusts()
        {
            if (Main.rand.NextBool())
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
                if (Main.rand.NextBool())
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
                else
                    Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ZacurrentDragon.ZacurrentPurple, Scale: Main.rand.NextFloat(0.1f, 0.3f));
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

    /// <summary>
    /// 使用ai2传入一共多少点
    /// </summary>
    public class RedDash : PurpleDash
    {
        public override void SpawnDusts()
        {
            if (Main.rand.NextBool())
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
                if (Main.rand.NextBool())
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Red>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
                else
                    Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ZacurrentDragon.ZacurrentRed, Scale: Main.rand.NextFloat(0.1f, 0.3f));
            }
        }
        public override Color ThunderColorFunc_Purple(float factor)
        {
            return Color.Lerp(ZacurrentDragon.ZacurrentPurple, ZacurrentDragon.ZacurrentRed, factor);
        }

        public override Color ThunderColorFunc2_Pink(float factor)
        {
            return ZacurrentDragon.ZacurrentRed;
        }
    }
}
