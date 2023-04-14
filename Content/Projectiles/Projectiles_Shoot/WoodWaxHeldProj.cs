using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class WoodWaxHeldProj : BaseGunHeldProj
    {
        public WoodWaxHeldProj() : base(0.6f, 10,-8, AssetDirectory.Weapons_Shoot) { }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.6f, 0.1f));
        }
    }
}
