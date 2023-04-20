using System.Collections.Generic;
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
    public class Hyacinth : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public float shootAngle;

        public override void SetDefaults()
        {
            Item.damage = 190;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.knockBack = 3;
            Item.shootSpeed = 11f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 50, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.shoot = 10;
            Item.useAmmo = AmmoID.Bullet;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                List<int> textureList = new List<int>();
                for (int i = 1; i < 23; i++)
                    textureList.Add(-i);

                for (int i = 0; i < 3; i++)     //生成环绕的幻影枪弹幕
                {
                    float angle = shootAngle + i * MathHelper.TwoPi / 3;
                    int textureType = Main.rand.NextFromList(textureList.ToArray());
                    Projectile.NewProjectile(source, player.Center + angle.ToRotationVector2() * 40, Vector2.Zero, ProjectileType<HyacinthPhantomGun>(), (int)(damage * 0.75f), knockback, player.whoAmI, textureType, i);
                    textureList.Remove(textureType);
                }

                shootAngle += MathHelper.TwoPi / 12;
                shootAngle %= MathHelper.TwoPi;

                //生成手持弹幕以及红色子弹
                Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One) * 16, ProjectileType<HyacinthRedBullet>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<HyacinthHeldProj>(), damage, knockback, player.whoAmI);

                SoundStyle style = CoraliteSoundID.Gun_Item11;
                //style.Pitch = -0.8f;
                SoundEngine.PlaySound(style, player.Center);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.PainterPaintballGun)
            .AddIngredient(ItemID.SuperStarCannon)
            .AddIngredient<Dunkleosteus>()
            .AddIngredient(ItemID.OnyxBlaster)
            .AddIngredient(ItemID.VenusMagnum)
            .AddIngredient(ItemID.ChainGun)
            .AddIngredient(ItemID.Xenopopper)
            .AddIngredient(ItemID.VortexBeater)
            .AddIngredient(ItemID.SDMG)
            .AddIngredient<StarsBreath>()
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}