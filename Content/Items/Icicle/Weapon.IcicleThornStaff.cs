using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Content.GlobalItems;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleThornStaff : ModItem,IConsultableItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<IceDragon1Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<IciclePage1>();

        private bool rightClick;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(22, 4f);

            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.maxStack = 1;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<IcicleThorn>();
            Item.UseSound = CoraliteSoundID.SummonStaff_Item44;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 1));

            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;

            CoraliteGlobalItem.SetColdDamage(Item);
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
            if (rightClick)
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.type == Item.shoot && p.owner == player.whoAmI))
                {
                    IcicleThorn pro = (IcicleThorn)proj.ModProjectile;
                    pro.rightClick = true;
                    if (pro.State == 1 || pro.State == 0)
                        pro.ResetStates();
                }
                return false;
            }

            var projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IcicleCrystal>()
            .AddIngredient<IcicleBreath>(2)
            .AddTile(TileID.IceMachine)
            .Register();
        }
    }
}