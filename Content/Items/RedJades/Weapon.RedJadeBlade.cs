using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeBlade : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public int shootCount;
        /// <summary> 使用多少次后进行强化大爆炸的射击 </summary>
        public int useBigBoom = 8;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 16;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.knockBack = 4f;
            Item.maxStack = 1;

            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<RedJadeBladeHeldProj>();
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Rapier;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (shootCount >= useBigBoom) //使用强力挥舞
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 3);

                shootCount = 0;
                useBigBoom = Main.rand.Next(6, 9);
            }
            else
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, shootCount % 3);
                Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * 10f,
                    ModContent.ProjectileType<RedJadeStrike>(), (int)(damage * 0.75f), knockback, player.whoAmI, Main.rand.Next(3));
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
