using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;

namespace Coralite.Core.Systems.MagikeSystem.Spells
{
    /// <summary>
    /// 法术回路核心，需要有魔能容器以及法术工作器以及物品容器
    /// </summary>
    /// <typeparam name="TModTile"></typeparam>
    public abstract class SpellCoreEntity<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override int TargetTileID => ModContent.TileType<TModTile>();

        public override int MainComponentID => MagikeComponentID.MagikeFactory;

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartContainer());
            AddComponent(GetStartFactory());
            AddComponent(GetStartGetOnlyContainer());
        }

        public abstract MagikeContainer GetStartContainer();
        public abstract SpellFactory GetStartFactory();
        public abstract GetOnlyItemContainer GetStartGetOnlyContainer();

        public override ApparatusInformation AddInformation() => null;
    }
}
