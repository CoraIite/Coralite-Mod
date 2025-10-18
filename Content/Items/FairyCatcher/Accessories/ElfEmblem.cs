using Coralite.Core.Systems.FairyCatcherSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Accessories
{
    public class ElfEmblem() : BaseFairyAccessory(ItemRarityID.LightRed, Item.sellPrice(0, 2))
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.SummonerEmblem;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fcp.fairyCatchPowerBonus += 0.15f;
        }
    }
}
