using Coralite.Core.Loaders;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    public class KnowledgePlayer : ModPlayer
    {
        const string Unlock = "Unlock";
        const string Readed = "Readed";

        /// <summary>
        /// 存储着所有知识的解锁数据，仅在游戏中可正常使用
        /// </summary>
        public bool[] KnowledgeUnlocks { get; internal set; }
        /// <summary>
        /// 存储着所有知识是否已读
        /// </summary>
        public bool[] KnowledgeReaded { get; internal set; }

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < KeyKnowledgeLoader.KnowledgeCount; i++)
            {
                Knowledge knowledge = KeyKnowledgeLoader.GetKeyKnowledge(i);

                if (KnowledgeUnlocks[i])
                    tag.Add(knowledge.Name + Unlock, true);
                if (KnowledgeReaded[i])
                    tag.Add(knowledge.Name + Readed, true);

                knowledge.SaveData(tag);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            if (KnowledgeUnlocks == null || KnowledgeUnlocks.Length != KeyKnowledgeLoader.KnowledgeCount)
                KnowledgeUnlocks = new bool[KeyKnowledgeLoader.KnowledgeCount];
            if (KnowledgeReaded == null || KnowledgeReaded.Length != KeyKnowledgeLoader.KnowledgeCount)
                KnowledgeReaded = new bool[KeyKnowledgeLoader.KnowledgeCount];

            for (int i = 0; i < KnowledgeUnlocks.Length; i++)
            {
                Knowledge knowledge = KeyKnowledgeLoader.GetKeyKnowledge(i);

                KnowledgeUnlocks[i] = tag.ContainsKey(knowledge.Name + Unlock);
                KnowledgeReaded[i] = tag.ContainsKey(knowledge.Name + Readed);

                knowledge.LoadData(tag);
            }
        }
    }
}
