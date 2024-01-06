using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class BubblePearlNecklace : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = RarityType<RareRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statLifeMax2 += 12;
            player.statDefense += 7;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.coreKeeperDodge += 0.05f;
            }
        }
    }
}
