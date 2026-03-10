using Coralite.Core.Systems.KeySystem;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    public class KnowledgeLoader
    {
        internal static List<KnowledgeSeries> knowledgeSerieses = [];
        internal static List<Knowledge> knowledges = [];

        /// <summary>
        /// 整理后的知识合集
        /// </summary>
        internal static List<KnowledgeSeries> SortedKnowledgeSerieses { get; private set; }

        internal static int KnowledgeCount { get; private set; } = 0;
        internal static int KnowledgeSeriesCount { get; private set; } = 0;

        /// <summary>
        /// 设置知识ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveKnowledgeID() => KnowledgeCount++;
        /// <summary>
        /// 设置知识合集ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveKnowledgeSeriesID() => KnowledgeCount++;

        /// <summary>
        /// 根据ID获取知识
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Knowledge GetKeyKnowledge(int type)
            => knowledges[type];

        /// <summary>
        /// 根据ID获取知识合集
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static KnowledgeSeries GetKnowledgeSeries(int type)
            => knowledgeSerieses[type];

        internal static void SetUp()
        {
            foreach (var knowledge in knowledges)
                knowledge.SetUp();
            foreach (var knowledgeSeries in knowledgeSerieses)
            {
                knowledgeSeries.SetUpKnowledges();
                knowledgeSeries.SetUp();
            }

            SortedKnowledgeSerieses = [];
            foreach (var item in knowledgeSerieses)
                SortedKnowledgeSerieses.Add(item);

            SortedKnowledgeSerieses.Sort((s1, s2) => s1.Priority.CompareTo(s2.Priority));
        }

        internal static void Unload()
        {
            foreach (var item in knowledgeSerieses)
                item.Unload();

            knowledgeSerieses = null;
            KnowledgeSeriesCount = 0;

            foreach (var item in knowledges)
                item.Unload();


            knowledges = null;
            KnowledgeCount = 0;
        }
    }
}
