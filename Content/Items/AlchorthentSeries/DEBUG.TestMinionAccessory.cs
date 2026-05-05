using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.AlchorthentSeries
{
#if DEBUG
    public class TestMinionAccessory : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxMinions += 1000;
        }
    }
#endif
}
