using Coralite.Core;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Coralite.Content.Items.ShadowItems
{
    public class ShadowEnergy : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("影子能量");

            Tooltip.SetDefault("它是无形的");
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0,0,5,50);
            Item.maxStack = 999;
            Item.height = Item.width= 40;
        }
    }
}
