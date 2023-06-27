
namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public interface IMagikeSender:IMagikeContainer
    {
         int HowManyPerSend { get; }

        void Send();
        bool CanSend();
        void OnSend(int howMany, IMagikeContainer receiver);
        void SendVisualEffect(IMagikeContainer container);
    }
}
