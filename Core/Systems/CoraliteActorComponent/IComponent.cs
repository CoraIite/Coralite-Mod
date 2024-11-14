using System.IO;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.CoraliteActorComponent
{
    public interface IComponent<TEntity>
    {
        public TEntity Entity { get; set; }

        public abstract int ID { get; }

        public virtual void OnAdd(TEntity entity) { }
        public virtual void OnRemove(TEntity entity) { }
        public virtual void Initialize() { }

        public abstract void Update();

        public virtual void SendData(ModPacket data) { }
        public virtual void ReceiveData(BinaryReader reader, int whoAmI) { }
        public virtual void SaveData(string preName, TagCompound tag) { }
        public virtual void LoadData(string preName, TagCompound tag) { }
    }
}
