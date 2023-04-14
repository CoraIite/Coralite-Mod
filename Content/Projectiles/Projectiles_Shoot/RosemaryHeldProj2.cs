using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class RosemaryHeldProj2 : BaseGunHeldProj
    {
        public RosemaryHeldProj2() : base(0.05f, 16, -2, AssetDirectory.Weapons_Shoot) { }

        public override void Initialize()
        {
            Projectile.timeLeft = Owner.itemTime + 1;
            MaxTime = Owner.itemTime + 1;
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                TargetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (Owner.direction > 0 ? 0f : 3.141f);
                if (TargetRot == 0f)
                    TargetRot = 0.0001f;
            }

            Projectile.localAI[1] += 1;
            HeldPositionX = heldPositionX;
            Projectile.netUpdate = true;
        }

        public override void ModifyAI(float factor)
        {
            if (Projectile.localAI[1] > 2)
                return;
            if (Projectile.timeLeft < 2)
                Initialize();
        }

    }
}