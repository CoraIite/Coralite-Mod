using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class ShieldShelf:ModItem
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateInventory(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.ShieldAbility_GuardShelf = true;
            }
        }
    }
}
