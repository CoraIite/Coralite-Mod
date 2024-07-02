using Coralite.Content.Items.GlobalItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Content.Projectiles.Globals
{
    public partial class CoraliteGlobalProjectile
    {
        public bool EdibleDamage;

        public static void SetEdibleDamage(Projectile i)
        {
            i.GetGlobalProjectile<CoraliteGlobalProjectile>().EdibleDamage = true;
        }

        public static bool IsEdibleDamage(Projectile i)
        {
            return i.GetGlobalProjectile<CoraliteGlobalProjectile>().EdibleDamage;
        }
    }
}
