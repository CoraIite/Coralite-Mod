using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class BaseSenderTileEntity<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override ushort TileType => (ushort)ModContent.TileType<TModTile>();

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartContainer());
            AddComponent(GetStartSender());
        }

        public abstract MagikeContainer GetStartContainer();
        public abstract MagikeLinerSender GetStartSender();
    }
}
