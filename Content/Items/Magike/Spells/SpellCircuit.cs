using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Spells;
using Coralite.Helpers;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Spells
{
    public class SpellCircuitTile() : SpellTile(3, 3, Coralite.CrystallinePurple, DustID.Water)
    {
        public override int DropItemType => ModContent.ItemType<SpellCircuitCore>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.CrystallineMagike,
            ];
        }
    }

    public class SpellCircuitEntity : SpellCoreEntity<SpellCircuitTile>
    {
        public override MagikeContainer GetStartContainer()
            => new SpellCircuitContainer();

        public override SpellFactory GetStartFactory()
            => new SpellFactory();

        public override GetOnlyItemContainer GetStartGetOnlyContainer()
            => new GetOnlyItemContainer()
            {
                CapacityBase = 20
            };
    }

    public class SpellCircuitContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.None
                or MALevel.CrystallineMagike => MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 8, 60 * 2),
                _ => 0,
            };

            LimitMagikeAmount();

            //AntiMagikeMaxBase = MagikeMaxBase * 2;
            //LimitAntiMagikeAmount();
        }
    }
}
