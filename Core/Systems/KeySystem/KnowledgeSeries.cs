using Coralite.Core.Loaders;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.KeySystem
{
    public abstract class KnowledgeSeries : ModType, ILocalizedModType
    {
        public LocalizedText SeriesName { get; private set; }

        /// <summary>
        /// 用于排序的值
        /// </summary>
        public abstract float Priority { get; }

        /// <summary>
        /// 存储的知识，需要手动加入，加入后自动排序
        /// </summary>
        public List<Knowledge> ContainedKnowledges { get; private set; }

        public int InnerType { get; private set; }

        public string LocalizationCategory => "Systems.KnowledgeSystem.Series";

        protected override void Register()
        {
            ModTypeLookup<KnowledgeSeries>.Register(this);

            InnerType = KeyKnowledgeLoader.ReserveKnowledgeSeriesID();

            KeyKnowledgeLoader.knowledgeSerieses ??= [];
            KeyKnowledgeLoader.knowledgeSerieses.Add(this);

            if (!Main.dedServ)
            {
                SeriesName = this.GetLocalization("Name");
            }
        }

        public void SetUpKnowledges()
        {
            ContainedKnowledges = [];

            AddKnowledges();

            //排序
            ContainedKnowledges.Sort((s1, s2) => s1.Priority.CompareTo(s2.Priority));
        }

        public abstract void AddKnowledges();

        /// <summary>
        /// 初始化时设置里面的东西
        /// </summary>
        public virtual void SetUp()
        {

        }
    }
}
