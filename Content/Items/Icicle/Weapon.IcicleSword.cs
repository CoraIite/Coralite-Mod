using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Content.GlobalItems;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleSword : ModItem,IConsultableItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<IceDragon1Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<IciclePage1>();

        public byte useCount;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(28, 2.5f);

            Item.useTime = 17;
            Item.useAnimation = 17;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ProjectileType<IcicleSwordSplash>();
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 1));

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            CoraliteGlobalItem.SetColdDamage(Item);
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

            Projectile.NewProjectile(source, player.Center, rotate.ToRotationVector2() * 9, type, damage, knockback, player.whoAmI, player.direction, (int)(20 * (1f + factor)));
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IcicleSwordHeldProj>(), damage, knockback, player.whoAmI, useCount);
            SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, player.Center);

            useCount++;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IcicleCrystal>(2)
            .AddIngredient<IcicleScale>()
            .AddIngredient<IcicleBreath>(2)
            .AddTile(TileID.IceMachine)
            .Register();
        }
    }
}
