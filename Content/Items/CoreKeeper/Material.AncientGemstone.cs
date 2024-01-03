using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.CoreKeeper
{
    public class AncientGemstone : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 999;

            Item.value = Item.sellPrice(0, 0, 12, 0);
            Item.rare = ModContent.RarityType<RareRarity>();
        }
    }
}
