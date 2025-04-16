using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    /// <summary>
    /// 关键信息
    /// </summary>
    public abstract class KeyKnowledge : ModTexturedType, ILocalizedModType
    {
        public override string Texture => AssetDirectory.KeyKnowledgeIcon + Name;

        public ATex Texture2D { get; private set; }

        public LocalizedText KnowledgeName { get; private set; }
        public LocalizedText LockTip { get; private set; }
        public LocalizedText Description { get; private set; }

        public int InnerType { get; private set; }

        /// <summary>
        /// 类型
        /// </summary>
        public abstract int Type { get; }

        /// <summary>
        /// 该知识是否解锁
        /// </summary>
        public bool Unlock { get; set; }

        public string LocalizationCategory => "Systems.KnowledgeSystem";

        /// <summary>
        /// 获取页数用于在UI内跳转
        /// </summary>
        public abstract int FirstPageInCoraliteNote { get; }

        protected override void Register()
        {
            ModTypeLookup<KeyKnowledge>.Register(this);

            InnerType = KeyKnowledgeLoader.ReserveKnowledgeID();

            KeyKnowledgeLoader.knowledges ??= [];
            KeyKnowledgeLoader.knowledges.Add(InnerType, this);

            if (!Main.dedServ)
            {
                Texture2D = ModContent.Request<Texture2D>(Texture);
                KnowledgeName = this.GetLocalization("Name");
                LockTip = this.GetLocalization("LockTip");
                Description = this.GetLocalization("Description");
            }
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
