using Coralite.Core;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleStaff : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 25;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.knockBack = 3f;
            Item.mana = 16;
            Item.crit = 0;
            Item.reuseDelay = 0;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<IcicleStaffHeldProj>();

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
                Helpers.Helper.PlayPitched("Icicle/IcicleStaff", 0.2f, 0f, player.Center);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IcicleCrystal>(2)
            .AddTile(TileID.IceMachine)
            .Register();
        }
    }
}
