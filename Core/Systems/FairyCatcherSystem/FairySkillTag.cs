using Coralite.Core.Loaders;
using System.Collections.Generic;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public abstract class FairySkillTag : ModType, ILocalizedModType
    {
        public string LocalizationCategory => "Systems.FairySkillTag";

        /// <summary>
        /// 技能标签名，用于技能描述
        /// </summary>
        public LocalizedText Text {  get; set; }

        public int Type { get; internal set; }

        protected override void Register()
        {
            ModTypeLookup<FairySkillTag>.Register(this);

            FairyLoader.skillTags ??= new List<FairySkillTag>();
            FairyLoader.skillTags.Add(this);

            Type = FairyLoader.ReserveFairySkillTagID();
            Text = this.GetLocalization(nameof(Text));
        }
    }
}
