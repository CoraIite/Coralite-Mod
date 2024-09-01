using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleSwordHeldProj : BaseSwingProj
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleSword";

        public ref float Combo => ref Projectile.ai[0];

        public IcicleSwordHeldProj() : base(new Vector2(46, 56).ToRotation())
        {

        }

        public override void SetDefs()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 22;
            Projectile.width = 34;
            Projectile.height = 68;
            distanceToOwner = 2;
            minTime = 0;
            onHitFreeze = 4;
            Projectile.coldDamage = true;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 1;

            switch (Combo)
            {
                default:
                case 0:
                    startAngle = 2.2f;
                    totalAngle = 3.6f;
                    maxTime = Owner.itemTimeMax * 2;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    break;
                case 1:
                    startAngle = 1.4f;
                    totalAngle = 3.8f;
                    maxTime = Owner.itemTimeMax * 2;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    Projectile.scale = 0.9f;

                    break;
                case 2:
                    startAngle = -1.6f;
                    totalAngle = -4.2f;
                    maxTime = (int)(Owner.itemTimeMax * 1.5f);

                    Smoother = Coralite.Instance.SqrtSmoother;
                    break;
            }

            base.Initializer();
        }

        protected override void OnSlash()
        {
            if (Timer < 3 * maxTime / 4f)
            {
                Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
                Dust dust = Dust.NewDustPerfect(Top - (24 * RotateVec2) + Main.rand.NextVector2Circular(30, 30), DustID.ApprenticeStorm,
                       dir * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(Top - (14 * RotateVec2) + Main.rand.NextVector2Circular(10, 10), DustID.ApprenticeStorm,
                       dir * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            switch (Combo)
            {
                default:
                case 0:
                    if (Timer < maxTime / 4f)
                        Projectile.scale += 0.04f;
                    else
                        Projectile.scale -= 0.01f;

                    break;
                case 1:
                    if (Timer < maxTime / 8f)
                        Projectile.scale += 0.10f;
                    else
                        Projectile.scale -= 0.015f;

                    break;
                case 2:
                    if (Timer < maxTime / 2f)
                        Projectile.scale += 0.03f;
                    else
                        Projectile.scale -= 0.03f;

                    break;
            }
            base.OnSlash();
        }

        protected override float GetStartAngle() => Owner.direction > 0 ? 0f : MathHelper.Pi;

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FrostStaff, RotateVec2.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(6f, 8f));
                    dust.noGravity = true;
                }
        }
    }
}
