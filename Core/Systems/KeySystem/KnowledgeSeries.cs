using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.KeySystem
{
    public abstract class KnowledgeSeries : ModTexturedType, ILocalizedModType
    {
        public override string Texture => AssetDirectory.NoteReadfragment + Name;

        public LocalizedText SeriesName { get; private set; }
        public LocalizedText SeriesDescription { get; private set; }
        public ATex Texture2D { get; private set; }

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

            InnerType = KnowledgeLoader.ReserveKnowledgeSeriesID();

            KnowledgeLoader.knowledgeSerieses ??= [];
            KnowledgeLoader.knowledgeSerieses.Add(this);

            if (!Main.dedServ)
            {
                Texture2D = ModContent.Request<Texture2D>(Texture);

                SeriesName = this.GetLocalization("Name");
                SeriesDescription = this.GetLocalization("Description");
            }
        }

        public void SetUpKnowledges()
        {
            ContainedKnowledges = [];

            AddKnowledges();

            //排序
            //ContainedKnowledges.Sort((s1, s2) => s1.Priority.CompareTo(s2.Priority));
        }

        public abstract void AddKnowledges();

        /// <summary>
        /// 帮助方法，用于添加知识
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddKnowledge<T>() where T : Knowledge
           => ContainedKnowledges.Add(CoraliteContent.GetKnowledge<T>());

        /// <summary>
        /// 初始化时设置里面的东西
        /// </summary>
        public virtual void SetUp()
        {

        }
    }
}
