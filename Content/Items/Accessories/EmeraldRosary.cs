using Coralite.Content.DamageClasses;
using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories
{
    public class EmeraldRosary:ModItem
    {
        public override string Texture => AssetDirectory.Accessories+Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("翡翠念珠");
            Tooltip.SetDefault("念力回复速度微量增加\n" +
                "念力伤害 +3");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0,30,0);
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CoralitePlayer>().nianliRegain += 0.15f;
            player.GetDamage<YujianDamage>().Flat += 3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Emerald, 3)
                .AddIngredient(ItemID.Bone, 10)
                .AddIngredient<MagliteDust>(10)
                .AddIngredient(ItemID.Vine, 3)
                .AddIngredient(ItemID.JungleSpores, 5)
                .Register();
        }
    }
}
