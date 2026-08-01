using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B9Bullet : ModItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override void SetDefaults()
        {
            Item.ammo = AmmoID.Bullet;
            Item.damage = 10;
            Item.knockBack = 3f;
            Item.maxStack = Item.CommonMaxStack;
            Item.shootSpeed = 5;
            Item.consumable = true;

            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 0, 0,5);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<B9BulletProj>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(300)
                .AddIngredient<B9Alloy>()
                .AddIngredient(ItemID.SoulofLight)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class B9BulletProj : ModProjectile
    {
        public override string Texture => AssetDirectory.SteelItems + nameof(B9Bullet);
    }

    public class B9Laser : ModProjectile
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

    }
}
