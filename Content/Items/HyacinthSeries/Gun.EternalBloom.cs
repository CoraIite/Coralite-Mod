using Coralite.Content.Items.Materials;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class EternalBloom : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 62;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.knockBack = 5.5f;
            Item.shootSpeed = 14f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ProjectileType<WoodWaxHeldProj>();
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = CoraliteSoundID.Gun3_Item41;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -8);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<EternalBloomHeldProj>(), 0, knockback, player.whoAmI);
            if (type == ProjectileID.Bullet)
            {
                type = Main.rand.NextBool(4) ? ProjectileType<ThornBall>() : ProjectileType<SeedPlantera>();

                Projectile.NewProjectile(source, position, velocity
                     , type, damage, knockback, player.whoAmI);
                return false;
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Floette>()
            .AddIngredient<RegrowthTentacle>(5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}
