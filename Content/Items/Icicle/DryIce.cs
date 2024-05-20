using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class DryIce : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 0, 20));

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 20;
            Item.UseSound = CoraliteSoundID.IceMagic_Item28;

            Item.consumable = true;
            Item.buffTime = 1;
            Item.buffType = BuffID.Sunflower;
        }

        public override bool CanUseItem(Player player)
        {
            if (Main.raining)
                return false;

            Main.StartRain();
            Main.SyncRain();
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return true;
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
