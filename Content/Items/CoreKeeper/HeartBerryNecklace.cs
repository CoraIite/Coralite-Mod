using Coralite.Core;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class HeartBerryNecklace : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = RarityType<EpicRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.lifeRegen += 1;
        }
    }
}
