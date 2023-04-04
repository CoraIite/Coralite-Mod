using Coralite.Core;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeShrine : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        private bool rightClick;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("赤玉祭坛");

            Tooltip.SetDefault("召唤小赤玉灵为你作战\n"
                                         + $"[c/00bbff:消耗1召唤栏]");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 16;
            Item.useTime = 35;
            Item.useAnimation = 18;
            Item.knockBack = 4f;
            Item.maxStack = 1;
            Item.mana = 25;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<RedBink>();
            Item.UseSound = SoundID.Item5;
            Item.value = Item.sellPrice(0, 0, 40, 0);
            Item.rare = ItemRarityID.Green;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool AltFunctionUse(Player Player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                rightClick = true;
            else
                rightClick = false;

            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (rightClick)
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.type == Item.shoot && p.owner == player.whoAmI))
                {
                    RedBink pro = (RedBink)proj.ModProjectile;
                    pro.rightClick = true;
                }
                return false;
            }

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = damage;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(10)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }

    }
}
