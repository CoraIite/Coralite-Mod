using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.CoreKeeper
{
    public class ClearGemstone : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 999;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ModContent.RarityType<EpicRarity>();
        }
    }
}
