using Coralite.Core;
using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class ReflectableProjectile : HookGroup
    {
        public override void Load()
        {
            On_Projectile.CanBeReflected += On_Projectile_CanBeReflected; ;
        }

        private bool On_Projectile_CanBeReflected(On_Projectile.orig_CanBeReflected orig, Projectile self)
        {
            bool flag = orig.Invoke(self);

            if (flag)
                return true;
            else
                return self.active && self.friendly && !self.hostile && self.damage > 0 && CoraliteSets.Projectiles.Reflectable[self.type];
        }

        public override void Unload()
        {
            On_Projectile.CanBeReflected -= On_Projectile_CanBeReflected; ;
        }
    }
}
