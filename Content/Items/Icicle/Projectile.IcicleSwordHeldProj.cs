using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleSwordHeldProj : BaseSwingProj
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleSword";

        public ref float Combo => ref Projectile.ai[0];

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 22;
            Projectile.width = 34;
            Projectile.height = 68;
            distanceToOwner = 2;
            minTime = 0;
            onHitFreeze = 4;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
            Smoother = Coralite.Instance.HeavySmootherInstance;
            maxTime = Owner.itemTimeMax * 2;
            Projectile.extraUpdates = 1;
            startAngle = 2.2f;
            totalAngle = 4.2f;

            base.Initializer();
        }

        protected override float GetStartAngle() => Owner.direction > 0 ? 0f : 3.141f;

        protected override void OnHitEvent(NPC target)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FrostStaff, RotateVec2.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f))*Main.rand.NextFloat(6f,8f));
                dust.noGravity = true;
            }
        }
    }
}
