using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;

namespace Coralite.Content.Items.FairyCatcher.CircleCoreAccessories
{
    public class CopperRing : BaseCircleCoreAccessory
    {
        public override int CircleCoreType => CoraliteContent.FairyCircleCoreType<CopperCircleCore>();
        public override int CircleRadiusBonus => 16;

        public override void SetDefaultsSafely()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.White0, Item.sellPrice(silver: 10));
        }
    }

    public class CopperCircleCore : FairyCircleCore
    { }
}
