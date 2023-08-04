using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.Rediancie
{
    /// <summary>
    /// 使用ai[0]控制是否发出声音，为0时会发出声音
    /// </summary>
    public class Rediancie_Explosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.timeLeft = 10;
            Projectile.aiStyle = -1;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Helper.RedJadeExplosion(Projectile.Center, false);
        }

        public override void AI()
        {
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
