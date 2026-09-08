using Coralite.Core;
using Coralite.Core.Systems.BossSystem;

namespace Coralite.Content.Bosses.ShadowBalls;

public class ShadowBallEclipseMoon : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.Blank;

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 70;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 120;
    }

    public override bool? CanDamage()
        => Projectile.timeLeft < 100 ? base.CanDamage() : false;

    public override void AI()
        => Projectile.velocity *= 0.92f;
}
