using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public class ShadowBallShadowMissile : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.Blank;

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 18;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 180;
    }

    public override void AI()
    {
        int targetIndex = (int)Projectile.ai[0];
        if (Main.player.IndexInRange(targetIndex) && !Main.player[targetIndex].dead)
            Projectile.velocity = Vector2.Lerp(Projectile.velocity,
                (Main.player[targetIndex].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 11, 0.045f);

        Projectile.rotation = Projectile.velocity.ToRotation();
    }
}
