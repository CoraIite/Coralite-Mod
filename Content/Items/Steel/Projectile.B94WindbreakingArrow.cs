using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B94WindbreakingArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        // Store the target NPC using Projectile.ai[0]
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        private float previousDistance = float.MaxValue;
        private Vector2 previousTargetPosition;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 16;

            Projectile.friendly = true;
            Projectile.arrow = true;
        }

        public override void AI()
        {
            // Initial rotation based on velocity
            Projectile.rotation = Projectile.velocity.ToRotation();

            float maxDetectRadius = 400f;

            if (DelayTimer < 10)
            {
                DelayTimer++;
                return;
            }

            int alpha = 0;
            float scale = 1.2f;

            if (HomingTarget == null)
            {
                alpha = 150;
                scale = 0.9f;

                Projectile.velocity.Y += 0.05f;
                if (Projectile.velocity.Y > 12)
                    Projectile.velocity.Y = 12;

                HomingTarget = Helper.FindClosestEnemy(Projectile.Center, maxDetectRadius, IsValidTarget);
                if (HomingTarget != null)
                {
                    previousTargetPosition = HomingTarget.Center;
                    previousDistance = Vector2.Distance(Projectile.Center, previousTargetPosition);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustPerfect(Vector2.Lerp(Projectile.Center, Projectile.oldPosition + Projectile.Size / 2, i / 3f)
                     , DustID.BlueTorch, Vector2.Zero, alpha, Scale: scale);
                d.noGravity = true;
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
                HomingTarget = null;

            if (HomingTarget == null)
                return;

            float currentDistance = Vector2.Distance(Projectile.Center, HomingTarget.Center);
            if (currentDistance > previousDistance)
            {
                HomingTarget = null;
                return;
            }

            previousDistance = currentDistance;

            // Smoothly rotate toward the target
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5)).ToRotationVector2() * length;

            // Update rotation to follow velocity direction
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public bool IsValidTarget(NPC target)
        {
            return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height)
                && target.Distance(Projectile.Center) < 800;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Titanium, Projectile.velocity.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(0.1f, 0.25f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickFrameDraw(new Rectangle(0, HomingTarget == null ? 0 : 1, 2, 1), lightColor, 1.57f);
            return false;
        }
    }
}
