using Coralite.Core;
using InnoVault.PRT;
using Terraria;

namespace Coralite.Content.Particles
{
    public class LiquidTrailParticle: LightTrailParticle
    {
        public override string Texture => AssetDirectory.Trails+ "LightShotSPA";

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Rotation = Velocity.ToRotation();
        }

        public static LiquidTrailParticle Spawn(Vector2 center, Vector2 velocity, Color newcolor, float scale, Color? targetColor = null, int trailCacheCount = 10)
        {
            if (VaultUtils.isServer)
                return null;

            LiquidTrailParticle p = PRTLoader.NewParticle<LiquidTrailParticle>(center, velocity, newcolor, scale);
            if (p != null)
            {
                p.targetColor = targetColor ?? new Color(0, 60, 250, 0);
                p.InitializeCaches(trailCacheCount);
            }

            return p;
        }

    }
}
