using Coralite.Core;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Steel
{
    public class B94WindbreakingCompoundBow : ModItem
    {
        public override string Texture => AssetDirectory.SteelItems + Name;
        public override void SetDefaults()
        {
            Item.SetWeaponValues(45, 2f, 4);
            Item.DefaultToRangedWeapon(ProjectileType<B94WindbreakingArrow>(), AmmoID.Arrow, 18, 14.5f, true);

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 2, 50);
            Item.rare = ItemRarityID.Pink;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
            Item.useTurn = false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ProjectileType<B94WindbreakingArrow>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 20)
                .AddIngredient(ItemID.BlackLens, 1)
                .AddIngredient<B9Alloy>(8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
    }
}