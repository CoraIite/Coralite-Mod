using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class Snowdrop : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 3;
            Item.shootSpeed = 10f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ProjectileType<SnowBullet>();
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = CoraliteSoundID.Shotgun2_Item38;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);
                int bulletType = ProjectileType<SnowBullet>();
                int snowDamage = (int)(damage * 0.9f);
                for (int i = 0; i < 3; i++) //射出4发白雪弹
                {
                    int timeLeft = Main.rand.Next(24, 28);
                    int index = Projectile.NewProjectile(source, player.Center, dir.RotatedBy(Main.rand.NextFloat(-0.24f, 0.24f)) * 14, bulletType, snowDamage, knockback, player.whoAmI, timeLeft - 2);
                    Main.projectile[index].timeLeft = timeLeft;
                    Main.projectile[index].netUpdate = true;
                }

                int timeLeft2 = Main.rand.Next(24, 28);
                int index2 = Projectile.NewProjectile(source, player.Center, dir.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * 14, bulletType, snowDamage, knockback, player.whoAmI, timeLeft2 - 2);
                Main.projectile[index2].timeLeft = timeLeft2;

                Projectile.NewProjectile(source, player.Center, dir * 12, ProjectileType<SnowdropBud>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<SnowdropHeldProj>(), damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.FrostCore)
            .AddIngredient(ItemID.ChlorophyteBar, 12)
            .AddIngredient(ItemID.SoulofLight, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}