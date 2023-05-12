using System;
using Coralite.Content.Projectiles.Projectiles_Shoot;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Weapons_Shoot
{
    public class Astrosniper : ModItem
    {
        public override string Texture => AssetDirectory.Weapons_Shoot + Name;

        public int offset;

        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.useTime = 17;
            Item.useAnimation = 34;
            Item.knockBack = 6;
            Item.shootSpeed = 25f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileID.Starfury;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = CoraliteSoundID.Gun2_Item40;


            Item.useTurn = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
        }

        public override void UseAnimation(Player player)
        {
            offset = Main.rand.Next(201);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                //懒得看这是啥，反正是从星怒那里抄过来的
                //new Vector2(100f, 0f);
                Vector2 pointPosition = new Vector2(player.Center.X + (float)(offset * -player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);

                if (player.ItemAnimationJustStarted)
                {
                    float targetRot = (pointPosition - player.Center).ToRotation();
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<AstrosniperHeldProj>(), damage, knockback, player.whoAmI, targetRot);
                    Projectile.NewProjectile(source, player.Center, targetRot.ToRotationVector2() * 20f, ProjectileID.BulletHighVelocity, 5, 0, player.whoAmI);
                }
                else
                {
                    float num2 = (float)Main.mouseX + Main.screenPosition.X - pointPosition.X;
                    float num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPosition.Y;
                    if (player.gravDir == -1f)
                        num3 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - pointPosition.Y;

                    float num4 = MathF.Sqrt(num2 * num2 + num3 * num3);
                    if ((float.IsNaN(num2) && float.IsNaN(num3)) || (num2 == 0f && num3 == 0f))
                    {
                        num2 = player.direction;
                        num3 = 0f;
                        num4 = Item.shootSpeed;
                    }
                    else
                        num4 = Item.shootSpeed / num4;

                    num2 *= num4;
                    num3 *= num4;

                    Vector2 mouseWorld2 = Main.MouseWorld;
                    Vector2 vec = mouseWorld2;
                    Vector2 vector60 = (pointPosition - mouseWorld2).SafeNormalize(new Vector2(0f, -1f));
                    while (vec.Y > pointPosition.Y && WorldGen.SolidTile(vec.ToTileCoordinates()))
                        vec += vector60 * 16f;

                    Projectile.NewProjectile(source, pointPosition, new Vector2(num2, num3), ProjectileID.Starfury, damage, 0, player.whoAmI, 0f, vec.Y);
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Musket)
            .AddIngredient(ItemID.Glass, 20)
            .AddIngredient(ItemID.MeteoriteBar, 5)
            .AddTile(TileID.Anvils)
            .Register();
        }

    }
}