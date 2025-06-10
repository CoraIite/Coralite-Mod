using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;

namespace Coralite.Content.Items.FairyCatcher.CircleCoreAccessories
{
    public class PlatinumRing : BaseCircleCoreAccessory<PlatinumCircleCore>
    {
        public override int CircleRadiusBonus => 16 * 3;

        public override void SetDefaultsSafely()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(silver: 30));
        }

        public override void UpdateAccessorySafely(Player player, FairyCatcherPlayer fcp, bool hideVisual)
        {
            fcp.fairyCatchPowerBonus.Flat += 3;
        }
    }

    public class PlatinumCircleCore : FairyCircleCore
    { }
}
