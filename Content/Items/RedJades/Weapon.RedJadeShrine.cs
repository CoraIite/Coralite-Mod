using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.RedJade;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeShrine : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<RedJadeKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<RedJadeItemPage>();

        private bool rightClick;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(20, 4f);
            Item.useTime = 35;
            Item.useAnimation = 18;
            Item.maxStack = 1;
            Item.DamageType = DamageClass.Summon;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<RedBink>();
            Item.UseSound = CoraliteSoundID.Bow_Item5;

            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 0, 40, 0));
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (rightClick)
                {
                    foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.type == Item.shoot && p.owner == player.whoAmI))
                    {
                        RedBink pro = (RedBink)proj.ModProjectile;
                        pro.rightClick = true;
                        proj.netUpdate = true;//同步右键标志，使远端的状态判定与特殊冲刺视觉一致
                    }
                    return false;
                }

                var projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, Main.myPlayer);
                projectile.originalDamage = Item.damage;
            }
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
