using Coralite.Core.Loaders;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵技能
    /// </summary>
    public abstract class FairySkill : ModType,ILocalizedModType
    {
        public int Type { get; internal set; }

        /// <summary>
        /// 仅在<see cref="FairyLoader"/>内存储的实例才能使用该属性，其余时候均为null
        /// </summary>
        public LocalizedText SkillName { get; private set; }

        public string LocalizationCategory => "Systems.FairySkill";

        protected override void Register()
        {
            ModTypeLookup<FairySkill>.Register(this);

            FairyLoader.skills ??= new List<FairySkill>();
            FairyLoader.skills.Add(this);

            Type = FairyLoader.ReserveFairyCoreID();

            SkillName = this.GetLocalization("SkillName");
        }

        public virtual FairySkill NewInstance()
        {
            var inst = (FairySkill)Activator.CreateInstance(GetType(), true);
            inst.Type = Type;
            return inst;
        }

        /// <summary>
        /// 开始攻击时调用，用于初始化
        /// </summary>
        public virtual void OnStartAttack()
        {

        }

        /// <summary>
        /// 更新仙灵
        /// </summary>
        public virtual void Update()
        {

        }
    }
}
