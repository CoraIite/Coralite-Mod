using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;

namespace Coralite.Core.Systems.MagikeSystem.Spells
{
    /// <summary>
    /// 法术节点，一般来说只需要有一个线性连接器的功能
    /// </summary>
    /// <typeparam name="TModTile"></typeparam>
    public abstract class SpellNodeEntity<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override int TargetTileID => ModContent.TileType<TModTile>();

        public override ApparatusInformation AddInformation()
            => null;

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartConnector());
        }

        public abstract SpellConnector GetStartConnector();
    }
}
