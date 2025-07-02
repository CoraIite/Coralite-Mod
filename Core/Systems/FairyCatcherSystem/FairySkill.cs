using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using System;
using System.Collections.Generic;
using Terraria;
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

        public virtual Color SkillTextColor { get; set; }= Color.White;

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
        /// 生成仙灵技能名称文本
        /// </summary>
        /// <param name="pos"></param>
        public virtual void SpawnSkillText(Vector2 pos)
        {
            CombatText.NewText(Utils.CenteredRectangle(pos,Vector2.One), SkillTextColor,
                SkillName.Value);
        }

        /// <summary>
        /// 开始攻击时调用，用于初始化
        /// </summary>
        public virtual void OnStartAttack()
        {

        }

        /// <summary>
        /// 更新仙灵，返回 <see cref="true"/> 表示技能使用结束
        /// </summary>
        public virtual bool Update(BaseFairyProjectile fairyProj)
        {
            return true;
        }

        /// <summary>
        /// 在仙灵死亡时调用
        /// </summary>
        /// <param name="fairyProj"></param>
        public virtual void OnFairyKill(BaseFairyProjectile fairyProj)
        {

        }
    }
}
