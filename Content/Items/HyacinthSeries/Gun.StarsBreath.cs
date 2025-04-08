using Coralite.Content.Items.Materials;
using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class StarsBreath : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.knockBack = 3;
            Item.shootSpeed = 12f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileType<SnowBullet>();
            Item.useAmmo = AmmoID.Bullet;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        /*         public override bool CanUseItem(Player player)
                {
                    if (Main.myPlayer == player.whoAmI)
                    {
                        int oldStyle = shootStyle;
                        List<int> shootList = new List<int> { 0, 1 };
                        shootStyle = Main.rand.NextFromList(shootList.ToArray());

                        if (shootStyle == oldStyle)
                        {
                            shootCount++;
                            if (shootCount > 2)
                            {
                                shootCount = 0;
                                shootList.Remove(shootStyle);
                                shootStyle = Main.rand.NextFromList(shootList.ToArray());
                            }
                        }
                        else
                            shootCount = 0;

                        switch (shootStyle)
                        {
                            default:
                            case 0:     //紫色双螺旋
                                Item.useTime = 14;
                                Item.useAnimation = 14;
                                Item.knockBack = 2;
                                break;
                            case 1:     //白色贯穿
                                Item.useTime = 20;
                                Item.useAnimation = 20;
                                Item.knockBack = 4;
                                break;
                        }
                    }
                    return true;
                } */

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(velocity);
                //int projType = ProjectileType<PlatycodonBullet1>();

                /*                 switch (shootStyle)
                                {
                                    default:
                                    case 0:     //紫色双螺旋
                                        Projectile.NewProjectile(source, player.Center, dir.RotatedBy(-0.36f) * 14, projType, damage, knockback, player.whoAmI, 0, -1);
                                        Projectile.NewProjectile(source, player.Center, dir.RotatedBy(0.36f) * 14, projType, damage, knockback, player.whoAmI, 1, 1);
                                        SoundEngine.PlaySound(CoraliteSoundID.Gun_Item11, player.Center);
                                        break;
                                    case 1:     //白色贯穿
                                        Projectile.NewProjectile(source, player.Center, dir * 14, ProjectileType<PlatycodonBullet2>(), damage, knockback, player.whoAmI);
                                        SoundEngine.PlaySound(CoraliteSoundID.Gun2_Item40, player.Center);
                                        break;
                                } */
                /*                 Projectile.NewProjectile(source, player.Center, dir.RotatedBy(-0.4f) * 13.5f, projType, damage, knockback, player.whoAmI, 0, -1);
                                Projectile.NewProjectile(source, player.Center, dir.RotatedBy(0.4f) * 13.5f, projType, damage, knockback, player.whoAmI, 1, 1);
                                Projectile.NewProjectile(source, player.Center, dir * 15, ProjectileType<PlatycodonBullet2>(), damage, knockback, player.whoAmI);
                 */
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(source, player.Center, velocity.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)), type, (int)(damage * 0.6f), knockback, player.whoAmI, 1, 1);
                }
                SoundEngine.PlaySound(CoraliteSoundID.Gun2_Item40, player.Center);

                Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<StarsBreathBullet>(), damage, knockback, player.whoAmI, Main.rand.Next(3));
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<StarsBreathHeldProj>(), damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Snowdrop>()
            .AddIngredient<Rosemary>()
            .AddIngredient<AncientCore>()
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}