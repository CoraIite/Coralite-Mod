using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class BaseCraftAltar<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override int TargetTileID => (ushort)ModContent.TileType<TModTile>();

        public override int MainComponentID => MagikeComponentID.MagikeFactory;

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartContainer());
            AddComponent(GetStartSender());
            AddComponent(GetStartAltar());
            AddComponent(GetStartItemContainer());
            AddComponent(GetStartGetOnlyItemContainer());
        }

        public abstract MagikeContainer GetStartContainer();
        public abstract CheckOnlyLinerSender GetStartSender();
        public abstract CraftAltar GetStartAltar();
        public abstract ItemContainer GetStartItemContainer();
        public abstract GetOnlyItemContainer GetStartGetOnlyItemContainer();
    }
}
