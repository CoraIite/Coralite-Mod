using Coralite.Core.Loaders;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.KeySystem
{
    public class KnowledgeSeries : ModType, ILocalizedModType
    {
        public LocalizedText SeriesName { get; private set; }

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

        /// <summary>
        /// 初始化时设置里面的东西
        /// </summary>
        public virtual void SetUp()
        {

        }
    }
}
