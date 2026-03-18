using Coralite.Content.Items.ShadowCastle;
using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    /// <summary>
    /// Arethusa：兰花的意思，但是也是希腊神话中阿瑞塞莎的名字，具体神话传说建议百度，另外这个我是在机翻时偶然发现的。
    /// 另外这把武器叫：幽兰
    /// </summary>
    public class Arethusa : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public int shootCount;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(39, 6);
            Item.DefaultToRangedWeapon(ProjectileType<ArethusaHeldProj>(), AmmoID.Bullet, 26, 10f, true);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightRed4, Item.sellPrice(0, 1));

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            shootCount++;
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<ArethusaHeldProj>(), damage, knockback, player.whoAmI, ai2: shootCount > 3 ? 1 : 0);
            if (shootCount > 2)
            {
                Vector2 targetDir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);
                Projectile.NewProjectile(source, player.Center, targetDir * 14, ProjectileType<ArethusaBullet>(), damage, knockback, player.whoAmI);
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_SuperMagicShoot_Item68, player.Center);
                shootCount = 0;
                return false;
            }

            SoundEngine.PlaySound(CoraliteSoundID.Gun_Item11, player.Center);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ShadowWave>()
            .AddIngredient<SunflowerGun>()
            .AddIngredient(ItemID.Musket)
            .AddIngredient(ItemID.Moonglow, 5)
            .AddTile(TileID.Anvils)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.PhoenixBlaster)
            .AddIngredient<SunflowerGun>()
            .AddIngredient(ItemID.Musket)
            .AddIngredient(ItemID.Moonglow, 5)
            .AddTile(TileID.Anvils)
            .Register();

            CreateRecipe()
            .AddIngredient<ShadowWave>()
            .AddIngredient<SunflowerGun>()
            .AddIngredient(ItemID.TheUndertaker)
            .AddIngredient(ItemID.Moonglow, 5)
            .AddTile(TileID.Anvils)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.PhoenixBlaster)
            .AddIngredient<SunflowerGun>()
            .AddIngredient(ItemID.TheUndertaker)
            .AddIngredient(ItemID.Moonglow, 5)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}