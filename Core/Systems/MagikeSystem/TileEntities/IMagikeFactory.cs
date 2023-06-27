
namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public interface IMagikeFactory : IMagikeContainer
    {
        bool StartWork();
        bool CanWork();
        void Work();
        void DuringWork();
        void WorkFinish();
    }
}
