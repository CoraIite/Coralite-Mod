using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Shadow
{
    public class InvertedShadow : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("倒影");

            // Tooltip.SetDefault("你的倒影将与你一同攻击");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 17;
            Item.crit = 1;
            Item.shootSpeed = 15f;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.knockBack = 2;

            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noMelee = true;

            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Expert;
            Item.UseSound = SoundID.Item40;
            Item.useAmmo = AmmoID.Bullet;

            Item.shoot = ProjectileID.Bullet;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<DrawInvertedPlayerProj>(), damage, knockback, player.whoAmI);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }


}