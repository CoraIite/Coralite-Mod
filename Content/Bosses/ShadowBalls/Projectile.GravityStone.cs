using Coralite.Core;
using Coralite.Core.Systems.BossSystem;

namespace Coralite.Content.Bosses.ShadowBalls;

/// <summary>
/// Stone projectile placeholder. ai[0] selects the snow biome; ai[1] stores the owning NPC index.
/// </summary>
public class GravityStone : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.Blank;

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60 * 10;
    }

    public override bool? CanDamage() => false;

    public void TurnToShoot(int targetPlayer, int time)
    {
        // TODO: Implement the aimed shot.
    }
}
