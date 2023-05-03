using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Terraria.DataStructures;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class HyacinthHeldProj : BaseGunHeldProj
    {
        public HyacinthHeldProj() : base(0.2f, 16, -4, AssetDirectory.Weapons_Shoot) { }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.8f;

        }
    }
}