using System.Linq;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleThornStaff : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        private bool rightClick;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 20;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 4f;
            Item.maxStack = 1;
            Item.mana = 25;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<IcicleThorn>();
            Item.UseSound = CoraliteSoundID.SummonStaff_Item44;
            Item.value = Item.sellPrice(0, 0, 40, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (rightClick)
                {
                    foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.type == Item.shoot && p.owner == player.whoAmI))
                    {
                        IcicleThorn pro = (IcicleThorn)proj.ModProjectile;
                        pro.rightClick = true;
                        if (pro.State == 1||pro.State==0)
                            pro.ResetStates();
                    }
                    return false;
                }

                var projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, Main.myPlayer);
                projectile.originalDamage = damage;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IcicleCrystal>(2)
            .AddTile(TileID.IceMachine)
            .Register();
        }
    }
}