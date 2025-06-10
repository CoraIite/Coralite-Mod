using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;

namespace Coralite.Content.Items.FairyCatcher.CircleCoreAccessories
{
    public class IronRing : BaseCircleCoreAccessory<IronCircleCore>
    {
        public override int CircleRadiusBonus => 16;

        public override void SetDefaultsSafely()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.White0, Item.sellPrice(silver: 15));
        }

        public override void UpdateAccessorySafely(Player player, FairyCatcherPlayer fcp, bool hideVisual)
        {
            fcp.fairyCatchPowerBonus.Flat += 1;
        }
    }

    public class IronCircleCore : FairyCircleCore
    { }
}
