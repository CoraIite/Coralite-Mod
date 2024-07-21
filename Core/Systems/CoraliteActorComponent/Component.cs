using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.CoraliteActorComponent
{
    //并非完全的ECS系统
    public abstract class Component
    {
        public Component()
        {
            Initialize();
        }

        public IEntity Entity { get; set; }

        public abstract int ID { get; }

        public virtual void OnAdd(IEntity entity) { }
        public virtual void OnRemove(IEntity entity) { }
        public virtual void Initialize() { }

        public abstract void Update(IEntity entity);

        public virtual void SaveData(string preName, TagCompound tag) { }
        public virtual void LoadData(string preName, TagCompound tag) { }
    }
}
