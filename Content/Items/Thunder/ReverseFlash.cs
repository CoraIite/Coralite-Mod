using Coralite.Core;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Items.Thunder
{
    public class ReverseFlash : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }
    }

    public class ReverseFlashProj : ModProjectile
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderProj";

        public ref float State => ref Projectile.ai[0];
    }
}
