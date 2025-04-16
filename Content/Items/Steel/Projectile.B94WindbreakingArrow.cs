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
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
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

                HomingTarget = FindClosestNPC(maxDetectRadius);
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

            // Calculate current distance to the target
            float currentDistance = Vector2.Distance(Projectile.Center, HomingTarget.Center);

            // If the distance increases, stop homing (fly in a straight line)
            if (currentDistance > previousDistance)
            {
                HomingTarget = null; // Stop homing
                return;
            }

            // Update the previous distance
            previousDistance = currentDistance;

            // Smoothly rotate toward the target
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(3)).ToRotationVector2() * length;

            // Update rotation to follow velocity direction
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted. 
                if (IsValidTarget(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target)
        {
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
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
            Projectile.QuickDraw(lightColor, 1.57f);
            return false;
        }
    }
}
