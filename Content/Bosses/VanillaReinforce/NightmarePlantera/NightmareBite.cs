using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 
    /// </summary>
    public class NightmareBite:ModProjectile
    {
        public override string Texture => AssetDirectory.NightmarePlantera+Name;

        public ref float State => ref Projectile.ai[0];
        public ref float ColorState=>ref Projectile.ai[1];

        /// <summary>
        /// 嘴张开的角度
        /// </summary>
        private float mouseAngle;
        private float alpha;

        public Vector2 OwnerCenter;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 1000;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }

        public override void AI()
        {

            switch ((int)State)
            {
                default:
                case 0:
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
