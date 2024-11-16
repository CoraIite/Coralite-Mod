using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    /// <summary>
    /// 关键信息
    /// </summary>
    public abstract class KeyKnowledge : ModTexturedType
    {
        public override string Texture => AssetDirectory.KeyKnowledgeIcon + Name;

        public Asset<Texture2D> Texture2D {  get;private set; }

        /// <summary>
        /// 类型
        /// </summary>
        public abstract int Type { get; }

        /// <summary>
        /// 该知识是否解锁
        /// </summary>
        public bool Unlock { get; set; }

        protected override void Register()
        {
            ModTypeLookup<KeyKnowledge>.Register(this);

            KeyKnowledgeLoader.knowledges ??= [];
            KeyKnowledgeLoader.knowledges.Add(Type, this);

            if (!Main.dedServ)
                Texture2D = ModContent.Request<Texture2D>(Texture);
        }

        /// <summary>
        /// 设置里面的东西
        /// </summary>
        public virtual void SetUp()
        {

        }

        /// <summary>
        /// 在该知识解锁时调用，可以用于跳字等行为
        /// </summary>
        public virtual void OnKnowldegeUnlock()
        {

        }

        public void UnlockKnowledge()
        {
            Unlock = true;

            OnKnowldegeUnlock();
        }


        public void SaveSelfData(TagCompound tag)
        {
            if (Unlock)
                tag.Add(Name + nameof(Unlock), true);
            SaveWorldData(tag);
        }

        public void LoadSelfData(TagCompound tag)
        {
            Unlock = tag.ContainsKey(Name + nameof(Unlock));
            LoadWorldData(tag);
        }

        /// <summary>
        /// 存储内容
        /// </summary>
        /// <param name="tag"></param>
        public virtual void SaveWorldData(TagCompound tag)
        {

        }

        public virtual void LoadWorldData(TagCompound tag)
        {

        }
    }
}
