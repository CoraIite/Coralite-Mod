using Coralite.Core.Systems.MagikeSystem.TileEntities;
using System.IO;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.CoraliteActorComponent
{
    //并非完全的ECS系统
    public abstract class MagikeComponent : IComponent<MagikeTP>
    {
        public MagikeComponent()
        {
            Initialize();
        }

        public MagikeTP Entity { get; set; }

        public abstract int ID { get; }

        public virtual void OnAdd(MagikeTP entity) { }
        public virtual void OnRemove(MagikeTP entity) { }
        public virtual void Initialize() { }

        public abstract void Update();

        public virtual void SendData(ModPacket data) { }
        public virtual void ReceiveData(BinaryReader reader, int whoAmI) { }
        public virtual void SaveData(string preName, TagCompound tag) { }
        public virtual void LoadData(string preName, TagCompound tag) { }
    }
}
