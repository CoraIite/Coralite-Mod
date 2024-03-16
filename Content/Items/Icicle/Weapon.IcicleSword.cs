using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleSword : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public byte useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 24;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<IcicleSwordSplash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (useCount == 3)
            {
                Item.useTime = 16;
                Item.useAnimation = 16;
            }
            else
            {
                Item.useTime = 20;
                Item.useAnimation = 20;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 dir = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.One);
                float factor = Math.Clamp(1 - (player.itemTimeMax * 2 / 54f), 0f, 1f);
                float rotate = dir.ToRotation();

                if (useCount == 3)
                {
                    Projectile.NewProjectile(source, player.Center, dir * 14, ProjectileType<IcicleSpurt>(), damage, knockback, player.whoAmI, player.direction, (int)(22 * (1f + factor)));
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IcicleSpurtHeldProj>(), damage, knockback, player.whoAmI, 0, rotate);

                    Helper.PlayPitched("Icicle/IcicleSword", 0.4f, 0f, player.Center);
                    useCount = 0;
                    return false;
                }

                //生成普通斩击和剑气
                //将角度限制在一定范围
                //            \     X        X     /         OK
                //                 \           /        OK
                //         OK         O           <---这个角是90°
                //                /             \       OK
                //            /    X       X       \        OK
                if (rotate < 0)
                    rotate += 6.282f;           //全是Magic Number。。。。
                if (rotate > 3.926f && rotate < 5.497f)
                    rotate = player.direction > 0 ? 5.497f : 3.926f;
                if (rotate > 0.785f && rotate < 2.355f)
                    rotate = player.direction > 0 ? 0.785f : 2.355f;

                Projectile.NewProjectile(source, player.Center, rotate.ToRotationVector2() * 8, type, damage, knockback, player.whoAmI, player.direction, (int)(18 * (1f + factor)));
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IcicleSwordHeldProj>(), damage, knockback, player.whoAmI, useCount);
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, player.Center);
            }

            useCount++;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IcicleCrystal>(2)
            .AddTile(TileID.IceMachine)
            .Register();

            Recipe recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.IceMachine);
            recipe.AddIngredient<IcicleCrystal>();
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.IceRod);
            recipe.AddIngredient<IcicleCrystal>();
            recipe.AddTile(TileID.IceMachine);
            recipe.Register();

        }
    }
}
