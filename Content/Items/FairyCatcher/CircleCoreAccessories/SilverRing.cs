using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;

namespace Coralite.Content.Items.FairyCatcher.CircleCoreAccessories
{
    public class SilverRing : BaseCircleCoreAccessory<SilverCircleCore>
    {
        public override int CircleRadiusBonus => 16 * 2;

        public override void SetDefaultsSafely()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.White0, Item.sellPrice(silver: 20));
        }

        public override void UpdateAccessorySafely(Player player, FairyCatcherPlayer fcp, bool hideVisual)
        {
            fcp.fairyCatchPowerBonus.Flat += 2;
        }
    }

    public class SilverCircleCore : FairyCircleCore
    { }
}
