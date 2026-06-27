using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem
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

        /// <summary>
        /// 是否允许在纯客户端上运行 <see cref="Update"/>。<br></br>
        /// 默认 <see langword="false"/>：机器逻辑服务端权威化，状态变更（产出/消耗/合成/发送魔能）只在服务端/单人运行。<br></br>
        /// 仅纯视觉/插值类组件（如鸟控制器、脉冲发送器特效）应重写为 <see langword="true"/>，
        /// 并自行用 <c>!VaultUtils.isClient</c> 包裹其中任何改变状态的逻辑。
        /// </summary>
        public virtual bool UpdateOnClient => false;

        public virtual void OnAdd(MagikeTP entity) { }
        public virtual void OnRemove(MagikeTP entity) { }
        public virtual void Initialize() { }

        public abstract void Update();

        public virtual void SendData(ModPacket data) { }
        public virtual void ReceiveData(BinaryReader reader, int whoAmI) { }
        public virtual void SaveData(string preName, TagCompound tag) { }
        public virtual void LoadData(string preName, TagCompound tag) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
