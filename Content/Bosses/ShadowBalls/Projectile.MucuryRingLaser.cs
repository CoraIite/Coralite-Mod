using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class MucuryRingLaser : GammaRayBurstRingLaser
    {
        public override void Initialize()
        {
            base.Initialize();
            Projectile.rotation = (EndPoint - Projectile.Center).ToRotation();
        }

        public override void SetRot(NPC owner)
        {
            Projectile.rotation = (EndPoint - Projectile.Center).ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Draw();
            return false;   
        }
    }
}
