using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class Pedestal<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public override int ExtendFilterCapacity => 1;

        public sealed override int TargetTileID => (ushort)ModContent.TileType<TModTile>();

        public override int MainComponentID => MagikeComponentID.ItemContainer;

        public override ApparatusInformation AddInformation() => new ApparatusInformation_NoPolar();

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartItemContainer());
        }

        public abstract ItemContainer GetStartItemContainer();
    }
}
