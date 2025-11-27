using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.Spells;
using System.Collections.Generic;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Spells
{
    public class SpellCircuitTile() : SpellTile(3, 3, Coralite.CrystallinePurple, DustID.Water)
    {
        public override int DropItemType => ModContent.ItemType<SpellCircuitCore>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                BrilliantLevel.ID,
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

    public class SpellCircuitContainer : UpgradeableContainer<SpellCircuitTile>
    {
    }
}
