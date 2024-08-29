using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class BaseCraftAltar<TModTile>() : MagikeTileEntity()
        where TModTile : ModTile
    {
        public sealed override ushort TileType => (ushort)ModContent.TileType<TModTile>();

        public override void InitializeBeginningComponent()
        {
            AddComponentDirectly(GetStartContainer());
            AddComponentDirectly(GetStartSender());
            AddComponentDirectly(GetStartAltar());
            AddComponentDirectly(GetStartItemContainer());
            AddComponentDirectly(GetStartGetOnlyItemContainer());
        }

        public abstract MagikeContainer GetStartContainer();
        public abstract CheckOnlyLinerSender GetStartSender();
        public abstract CraftAltar GetStartAltar();
        public abstract ItemContainer GetStartItemContainer();
        public abstract GetOnlyItemContainer GetStartGetOnlyItemContainer();
    }
}
