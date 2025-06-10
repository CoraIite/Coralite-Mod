using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;

namespace Coralite.Content.Items.FairyCatcher.CircleCoreAccessories
{
    public class TinRing : BaseCircleCoreAccessory<TinCircleCore>
    {
        public override int CircleRadiusBonus => 16;

        public override void SetDefaultsSafely()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.White0, Item.sellPrice(silver: 10));
        }
    }

    public class TinCircleCore : FairyCircleCore
    { }
}
