using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class BaseCostItemProducerTileEntity<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override ushort TileType => (ushort)ModContent.TileType<TModTile>();

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartContainer());
            AddComponent(GetStartSender());
            AddComponent(GetStartProducer());
            AddComponent(GetStartItemContainer());
        }

        public abstract MagikeContainer GetStartContainer();
        public abstract MagikeLinerSender GetStartSender();
        public abstract MagikeCostItemProducer GetStartProducer();
        public abstract ItemContainer GetStartItemContainer();
    }
}
