using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.CoreKeeper
{
    public class CraftLock : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateInventory(Player player)
        {
            Item.TurnToAir();
        }

        public override bool OnPickup(Player player)
        {
            return false;
        }
    }
}
