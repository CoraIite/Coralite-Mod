using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class BaseMabirdNest<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override int TargetTileID => (ushort)ModContent.TileType<TModTile>();

        public override int MainComponentID => MagikeComponentID.ItemContainer;

        public override ApparatusInformation AddInformation() => new ApparatusInformation_NoPolar();

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartMabirdController());
        }

        public abstract MabirdController GetStartMabirdController();
    }
}
