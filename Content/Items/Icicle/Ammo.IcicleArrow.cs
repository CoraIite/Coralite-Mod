using Coralite.Content.GlobalItems;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleArrowItem : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleArrow";

        public override void SetDefaults()
        {
            Item.ammo = AmmoID.Arrow;
            Item.damage = 9;
            Item.knockBack = 3f;
            Item.maxStack = Item.CommonMaxStack;
            Item.shootSpeed = 4;
            Item.consumable = true;

            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 0, 0, 5);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<IcicleArrow>();
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe(350)
                .AddIngredient<IcicleCrystal>()
                .AddIngredient(ItemID.WoodenArrow, 100)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }
}
