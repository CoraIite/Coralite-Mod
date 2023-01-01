
using Coralite.Core;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items
{
    public class Test : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;
        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.shoot = ProjectileID.Bubble;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Bullet;
            Item.useAnimation = 20;
            Item.shootSpeed = 10f;
            Item.autoReuse = true;
        }
    }
}
