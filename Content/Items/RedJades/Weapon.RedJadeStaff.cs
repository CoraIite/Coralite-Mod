using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeStaff : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public int shootCount;
        /// <summary> 使用多少次后进行强化大爆炸的射击 </summary>
        public int useBigBoom = 8;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("赤岩杖");

            // Tooltip.SetDefault("射出赤玉光束");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 18;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 5f;
            Item.maxStack = 1;
            Item.mana = 9;
            Item.shootSpeed = 9.5f;

            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<RedJadeStaffHeldProj>();
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Rapier;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;
            if (shootCount >= useBigBoom) //射出大爆炸弹幕
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI,1);

                shootCount = 0;
                useBigBoom = Main.rand.Next(6, 9);
            }
            else
            {
                Helper.PlayPitched("RedJade/RedJadeBeam", 0.16f, 0f, player.Center);
                Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * 11f, ModContent.ProjectileType<RedJadeBeam>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }

            shootCount++;
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
