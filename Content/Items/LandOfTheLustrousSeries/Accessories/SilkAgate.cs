using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries.Accessories
{
    public class SilkAgate() : BaseAccessory(ItemRarityID.LightRed, Item.sellPrice(0, 0, 5))
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.GemWeaponAttSpeedBonus += 0.12f;
        }
    }
}
