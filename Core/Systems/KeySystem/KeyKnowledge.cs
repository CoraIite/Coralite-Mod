using Coralite.Core.Loaders;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    /// <summary>
    /// 关键信息
    /// </summary>
    public abstract class KeyKnowledge : ModTexturedType
    {
        public override string Texture => AssetDirectory.KeyKnowledgeIcon + Name;

        /// <summary>
        /// 类型
        /// </summary>
        public int Type {  get; set; }

        /// <summary>
        /// 该知识是否解锁
        /// </summary>
        public bool Unlock {  get; set; }

        /// <summary>
        /// 是否初次获得
        /// </summary>
        public bool FirstPickUp {  get; set; }

        protected override void Register()
        {
            ModTypeLookup<KeyKnowledge>.Register(this);

            KeyKnowledgeLoader.knowledges ??= new List<KeyKnowledge>();
            KeyKnowledgeLoader.knowledges.Add(this);
        }

        public void SaveSelfData(TagCompound tag)
        {
            if (Unlock)
                tag.Add(Name + nameof(Unlock), true);
            if (FirstPickUp)
                tag.Add(Name + nameof(FirstPickUp), true);
            SaveWorldData(tag);
        }

        public void LoadSelfData(TagCompound tag)
        {
            Unlock = tag.ContainsKey(Name + nameof(Unlock));
            FirstPickUp = tag.ContainsKey(Name + nameof(FirstPickUp));
            LoadWorldData(tag);
        }

        public virtual void SaveWorldData(TagCompound tag)
        {

        }

        public virtual void LoadWorldData(TagCompound tag)
        {

        }
    }
}
