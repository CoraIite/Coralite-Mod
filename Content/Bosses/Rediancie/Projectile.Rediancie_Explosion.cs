using Coralite.Core;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.Rediancie
{
    /// <summary>
    /// 使用ai[0]控制是否发出声音，为0时会发出声音
    /// </summary>
    public class Rediancie_Explosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
        private bool span;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.timeLeft = 10;
            Projectile.aiStyle = -1;

            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (!span)
            {
                Helper.RedJadeExplosion(Projectile.Center, false);
                span = true;
            }
            if (Projectile.localAI[0] == 0)
            {
                if (Projectile.ai[0] == 0)
                    Helper.PlayPitched("RedJade/RedJadeBoom", 0.4f, 0f, Projectile.Center);
                Projectile.localAI[0] = 1;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
