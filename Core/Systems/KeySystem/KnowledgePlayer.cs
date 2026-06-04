using Coralite.Core.Loaders;
using System.Collections.Generic;
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

        public Dictionary<string, object> Datas { get; private set; }



        public override void Initialize()
        {
            KnowledgeUnlocks = new bool[KnowledgeLoader.KnowledgeCount];
            KnowledgeReaded = new bool[KnowledgeLoader.KnowledgeCount];

            Datas ??= [];

            for (int i = 0; i < KnowledgeLoader.KnowledgeCount; i++)
            {
                Knowledge knowledge = KnowledgeLoader.GetKnowledge(i);
                knowledge.OnPlayerInitialize(this);
            }
        }

        public void AddData(string name, object data)
        {
            Datas.Add(name, data);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetData<T>(string name) => (T)Datas[name];
        /// <summary>
        /// 设置数据，一般只有值类型才需要使用这个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetData<T>(string name, T value) => Datas[name] = value;

        public override void OnEnterWorld()
        {
            //if (KnowledgeUnlocks == null || KnowledgeUnlocks.Length != KnowledgeLoader.KnowledgeCount)
            //    KnowledgeUnlocks = new bool[KnowledgeLoader.KnowledgeCount];
            //if (KnowledgeReaded == null || KnowledgeReaded.Length != KnowledgeLoader.KnowledgeCount)
            //    KnowledgeReaded = new bool[KnowledgeLoader.KnowledgeCount];

            for (int i = 0; i < KnowledgeLoader.KnowledgeCount; i++)
            {
                Knowledge knowledge = KnowledgeLoader.GetKnowledge(i);
                knowledge.OnEnterWorld();
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (KnowledgeUnlocks != null && KnowledgeReaded != null)
                for (int i = 0; i < KnowledgeLoader.KnowledgeCount; i++)
                {
                    Knowledge knowledge = KnowledgeLoader.GetKnowledge(i);

                    if (KnowledgeUnlocks[i])
                        tag.Add(knowledge.Name + Unlock, true);
                    if (KnowledgeReaded[i])
                        tag.Add(knowledge.Name + Readed, true);

                    knowledge.SaveData(this, tag);
                }
        }

        public override void LoadData(TagCompound tag)
        {
            //if (KnowledgeUnlocks == null || KnowledgeUnlocks.Length != KnowledgeLoader.KnowledgeCount)
            //    KnowledgeUnlocks = new bool[KnowledgeLoader.KnowledgeCount];
            //if (KnowledgeReaded == null || KnowledgeReaded.Length != KnowledgeLoader.KnowledgeCount)
            //    KnowledgeReaded = new bool[KnowledgeLoader.KnowledgeCount];

            for (int i = 0; i < KnowledgeLoader.KnowledgeCount; i++)
            {
                Knowledge knowledge = KnowledgeLoader.GetKnowledge(i);

                KnowledgeUnlocks[i] = tag.ContainsKey(knowledge.Name + Unlock);
                KnowledgeReaded[i] = tag.ContainsKey(knowledge.Name + Readed);

                knowledge.LoadData(this, tag);
            }
        }
    }
}
