using Coralite.Content.ModPlayers;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleBow : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 16;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.knockBack = 3f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.useAmmo = AmmoID.Arrow;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<IcicleBowHeldProj>();
            Item.UseSound = CoraliteSoundID.Bow_Item5;

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
                float rot = dir.ToRotation();
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IcicleBowHeldProj>(), damage, knockback, player.whoAmI, rot, 0);
                Projectile.NewProjectile(source, player.Center, dir * 13, ProjectileType<IcicleArrow>(), damage, knockback, player.whoAmI);
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