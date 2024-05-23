using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class WoodWax : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.knockBack = 4;
            Item.shootSpeed = 10;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 0, 30, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ProjectileType<WoodWaxHeldProj>();
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = CoraliteSoundID.Blowgun_Item64;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<WoodWaxHeldProj>(), damage, knockback, player.whoAmI);
                if (type == ProjectileID.Bullet)
                {
                    Vector2 targetDir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);
                    Projectile.NewProjectile(source, player.Center, targetDir * Main.rand.NextFloat(6f, 8f), ProjectileType<WoodWaxBullet>(), (int)(damage * (5 / 12f)), knockback / 2, player.whoAmI);

                    for (int i = 0; i < 2; i++)
                        Projectile.NewProjectile(source, player.Center, targetDir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(6f, 8f), ProjectileType<WoodWaxBullet>(), (int)(damage * (5 / 12f)), knockback / 2, player.whoAmI);

                    return false;
                }

                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.JungleSpores, 10)
            .AddIngredient(ItemID.Vine, 5)
            .AddIngredient(ItemID.IllegalGunParts)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
