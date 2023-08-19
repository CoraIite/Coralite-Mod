using Microsoft.Xna.Framework;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareSparkle_Normal : BaseNightmareSparkle
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 200;

            Projectile.hostile = true;
            ShineColor = new Color(153, 88, 156, 230);
            mainSparkleScale = new Vector2(1.5f, 3f);
            circleSparkleScale = 0.5f;
        }
    }
}
