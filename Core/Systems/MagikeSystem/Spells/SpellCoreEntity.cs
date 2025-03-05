using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;

namespace Coralite.Core.Systems.MagikeSystem.Spells
{
    public abstract class SpellCoreEntity<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override int TargetTileID => ModContent.TileType<TModTile>();

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartContainer());
            AddComponent(GetStartFactory());
        }

        public abstract MagikeContainer GetStartContainer();
        public abstract SpellFactory GetStartFactory();
    }
}
