using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeCharger<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override int TargetTileID => (ushort)ModContent.TileType<TModTile>();

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartContainer());
            AddComponent(GetStartCharger());
            AddComponent(GetStartItemContainer());
        }

        public abstract MagikeContainer GetStartContainer();
        public abstract Charger GetStartCharger();
        public abstract ItemContainer GetStartItemContainer();
    }
}
