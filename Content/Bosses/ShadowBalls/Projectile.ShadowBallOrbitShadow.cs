using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public class ShadowBallOrbitShadow : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.Blank;

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 22;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 120;
    }

    public override void AI()
        => Projectile.velocity = Projectile.velocity.RotatedBy(0.025f) * 0.99f;
}
