using Coralite.Content.Projectiles.RedJadeProjectiles;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJadeItems
{
    public class RedJadeStaff : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("赤岩杖");

            Tooltip.SetDefault("射出赤玉光束，偶尔会产生爆炸");

        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 21;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.knockBack = 5f;
            Item.maxStack = 1;
            Item.mana = 9;
            Item.shootSpeed = 9.5f;

            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.staff[Type] = true;
            Item.shoot = ModContent.ProjectileType<RedJadeBeam>();
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Helper.PlayPitched("RedJade/RedJadeBeam", 0.16f, 0f, player.Center);
            Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * 9.5f, type, damage, knockback, player.whoAmI, Main.rand.Next(3));
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<RedJade>(14)
            .AddTile<Tiles.RedJades.MagicCraftStation>()
            .Register();
        }
    }
}
