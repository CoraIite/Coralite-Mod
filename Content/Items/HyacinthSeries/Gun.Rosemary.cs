using Coralite.Core;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class Rosemary : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 1.7f;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.useTime = 6;
            Item.useAnimation = 17;
            Item.reuseDelay = 12;
            Item.knockBack = 1;
            Item.shootSpeed = 10f;
            Item.crit = 12;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ProjectileType<RosemaryBullet>();
            Item.useAmmo = AmmoID.Bullet;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.consumeAmmoOnFirstShotOnly = true;
        }

        public override void UseAnimation(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int extraProjType = ProjectileType<RosemaryFog>();
                foreach (var projectile in Main.projectile.Where(p => p.active && p.friendly && p.type == extraProjType && p.owner == player.whoAmI))
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = (Main.MouseWorld - projectile.Center).ToRotation();
                    projectile.netUpdate = true;
                }

                SoundEngine.PlaySound(CoraliteSoundID.TripleGun_Item31, player.Center);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center + new Vector2(0, -8), (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * 14, ProjectileType<RosemaryBullet>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<RosemaryHeldProj>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Arethusa>()
            .AddIngredient(ItemID.SoulofNight, 5)
            .AddIngredient(ItemID.SoulofSight, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();

            CreateRecipe()
            .AddIngredient<Arethusa>()
            .AddIngredient(ItemID.SoulofNight, 5)
            .AddIngredient(ItemID.SoulofMight, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();

            CreateRecipe()
            .AddIngredient<Arethusa>()
            .AddIngredient(ItemID.SoulofNight, 5)
            .AddIngredient(ItemID.SoulofFright, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}