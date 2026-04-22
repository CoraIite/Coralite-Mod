using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Content.GlobalItems;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleStaff : ModItem,IConsultableItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<IceDragon1Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<IciclePage1>();

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ProjectileType<IcicleStaffHeldProj>(), 14, 0, true);
            Item.SetWeaponValues(29, 3f);
            Item.mana = 16;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 1));

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            Helpers.Helper.PlayPitched("Icicle/IcicleStaff", 0.2f, 0f, player.Center);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IcicleCrystal>()
            .AddIngredient<IcicleBreath>(3)
            .AddTile(TileID.IceMachine)
            .Register();
        }
    }
}
