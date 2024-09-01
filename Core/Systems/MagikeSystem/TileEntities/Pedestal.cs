using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class Pedestal<TModTile>() : MagikeTileEntity()
        where TModTile : ModTile
    {
        public sealed override ushort TileType => (ushort)ModContent.TileType<TModTile>();

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartItemContainer());
        }

        public abstract ItemContainer GetStartItemContainer();
    }
}
