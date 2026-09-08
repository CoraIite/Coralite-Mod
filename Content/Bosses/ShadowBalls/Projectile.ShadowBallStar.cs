using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls;

public class ShadowBallStar : CoraliteBossHostileProj
{
    public override string Texture => AssetDirectory.Blank;

    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 14;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 150;
    }

    public override void AI()
    {
        if (Projectile.ai[0] >= 0 && Projectile.ai[0].GetNPCOwner<ShadowBall>(out NPC owner))
        {
            if (Projectile.timeLeft > 100)
                Projectile.velocity *= 0.94f;
            else
            {
                Projectile.ai[1] += 0.025f;
                Vector2 orbitPosition = owner.Center + Projectile.ai[1].ToRotationVector2() * 170;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity,
                    (orbitPosition - Projectile.Center).SafeNormalize(Vector2.Zero) * 7, 0.1f);
            }
        }
        else
            Projectile.velocity *= 0.94f;
    }

    public override bool? CanDamage()
        => Projectile.ai[0] < 0 || Projectile.timeLeft < 100 ? base.CanDamage() : false;
}
