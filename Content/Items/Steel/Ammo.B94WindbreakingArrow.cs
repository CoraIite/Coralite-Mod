using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B94WindbreakingArrowItem : ModItem
    {
        public override string Texture => AssetDirectory.SteelItems + "B94WindbreakingArrow";
        public override void SetDefaults()
        {
            Item.ammo = AmmoID.Arrow;
            Item.damage = 10;
            Item.knockBack = 3f;
            Item.maxStack = Item.CommonMaxStack;
            Item.shootSpeed = 8;
            Item.consumable = true;

            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 0, 5);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<B94WindbreakingArrow>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(350)
                .AddIngredient<B9Alloy>()
                .AddIngredient(ItemID.Bone, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
