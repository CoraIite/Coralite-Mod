using Coralite.Core;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Mushroom
{
    public class MushroomStaff : ModItem
    {
        public override string Texture => AssetDirectory.MushroomItems + Name;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("蘑菇共生杖");

            /* Tooltip.SetDefault("召唤一只可爱的蘑菇幼龙\n"
                                            + $"[c/00bbff:消耗1召唤栏]"); */
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 6;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.reuseDelay = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 20;
            Item.knockBack = 3;

            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.White;
            Item.shoot = ProjectileType<MushroomDragon>();
            Item.shootSpeed = 0.1f;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.staff[Item.type] = true;
        }

        /// <summary>
        /// 使用这个可以很方便的实现按键控制召唤物弹幕
        /// </summary>
        /// <param name="player"></param>
        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && Main.mouseRight)
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.type == Item.shoot && p.owner == player.whoAmI))
                {
                    MushroomDragon pro = (MushroomDragon)proj.ModProjectile;
                    //代码写这里
                    pro.rightClick = true;
                }
                //Main.NewText("使用蓄力冲刺！", Color.Yellow);
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;
            return false;
        }

        /// <summary>
        /// 合成配方
        /// </summary>
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Mushroom, 10);
            recipe.AddTile(TileID.MushroomPlants);
            recipe.Register();
        }
    }
}
