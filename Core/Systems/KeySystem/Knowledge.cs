using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    /// <summary>
    /// 用于显示在珊瑚笔记中的知识，唯一存在
    /// </summary>
    public abstract class Knowledge : ModTexturedType, ILocalizedModType
    {
        public override string Texture => AssetDirectory.KeyKnowledgeIcon + Name;

        public ATex Texture2D { get; private set; }

        public LocalizedText KnowledgeName { get; private set; }
        public LocalizedText LockTip { get; private set; }
        public LocalizedText Description { get; private set; }

        /// <summary>
        /// 用于在知识合集中排序的值
        /// </summary>
        public abstract float Priority { get; }

        public int InnerType { get; private set; }

        /// <summary>
        /// 该知识是否解锁
        /// </summary>
        public bool Unlock
        {
            get
            {
                if (Main.LocalPlayer.TryGetModPlayer(out KnowledgePlayer kp))
                    return kp.KnowledgeUnlocks[InnerType];

                return false;
            }
        }

        /// <summary>
        /// 玩家是否在珊瑚笔记中查看过这条知识，仅限客户端使用
        /// </summary>
        public bool Readed
        {
            get
            {
                if (Main.LocalPlayer.TryGetModPlayer(out KnowledgePlayer kp))
                    return kp.KnowledgeReaded[InnerType];

                return false;
            }
        }

        public string LocalizationCategory => "Systems.KnowledgeSystem";

        /// <summary>
        /// 获取页数用于在UI内跳转
        /// </summary>
        public abstract int FirstPageInCoraliteNote { get; }

        protected override void Register()
        {
            ModTypeLookup<Knowledge>.Register(this);

            InnerType = KeyKnowledgeLoader.ReserveKnowledgeID();

            KeyKnowledgeLoader.knowledges ??= [];
            KeyKnowledgeLoader.knowledges.Add(this);

            if (!Main.dedServ)
            {
                Texture2D = ModContent.Request<Texture2D>(Texture);
                KnowledgeName = this.GetLocalization("Name");
                LockTip = this.GetLocalization("LockTip");
                Description = this.GetLocalization("Description");
            }
        }

        /// <summary>
        /// 初始化时设置里面的东西
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
            //Unlock = true;
            if (!VaultUtils.isServer&&Main.LocalPlayer.TryGetModPlayer(out KnowledgePlayer kp))
            {
                kp.KnowledgeUnlocks[InnerType] = true;

                //TODO：添加弹窗
            }

            OnKnowldegeUnlock();
        }

        //public void SaveSelfData(TagCompound tag)
        //{
        //    if (Unlock)
        //        tag.Add(Name + nameof(Unlock), true);
        //    if (ReadKnowledge)
        //        tag.Add(Name + nameof(ReadKnowledge), true);

        //    SaveData(tag);
        //}

        //public void LoadSelfData(TagCompound tag)
        //{
            //Unlock = tag.ContainsKey(Name + nameof(Unlock));
            //ReadKnowledge = tag.ContainsKey(Name + nameof(ReadKnowledge));
            //LoadExtraData(tag);
        //}

        /// <summary>
        /// 存储内容，注意这个是存储在玩家里的
        /// </summary>
        /// <param name="tag"></param>
        public virtual void SaveData(TagCompound tag)
        {

        }

        /// <summary>
        /// 读取额外内容
        /// </summary>
        /// <param name="tag"></param>
        public virtual void LoadData(TagCompound tag)
        {

        }
    }
}
