using Coralite.Content.Projectiles.RedJadeProjectiles;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJadeItems
{
    public class RedJadeBlade : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("赤玉刃");

            Tooltip.SetDefault("射出赤玉碎片，偶尔会产生爆炸");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 17;
            Item.useTime = 35;
            Item.useAnimation = 18;
            Item.knockBack = 4f;

            Item.autoReuse = true;
            Item.useTurn = false;

            Item.shoot = ModContent.ProjectileType<RedJadeStrike>();
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * 10f, type, damage, knockback, player.whoAmI, Main.rand.Next(4));
            return false;
        }
    }
}
