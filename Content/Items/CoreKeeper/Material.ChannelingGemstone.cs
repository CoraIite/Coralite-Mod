using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.CoreKeeper
{
    public class ChannelingGemstone : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = Item.CommonMaxStack;

            Item.value = Item.sellPrice(0, 1);
            Item.rare = ModContent.RarityType<EpicRarity>();
        }
    }
}
