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
        public virtual void OnStartAttack(BaseFairyProjectile fairyProj)
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

        /// <summary>
        /// 命中NPC后调用，主动效果，只有在使用该技能的时候才会触发
        /// </summary>
        /// <param name="fairyProj"></param>
        /// <param name="target"></param>
        /// <param name="hitModifier"></param>
        /// <param name="npcDamage"></param>
        public virtual void ModifyHitNPC_Active(BaseFairyProjectile fairyProj,NPC target,NPC.HitModifiers hitModifier,ref int npcDamage)
        {

        }

        /// <summary>
        /// 命中NPC后调用，被动效果，每次攻击到NPC都会触发
        /// </summary>
        /// <param name="fairyProj"></param>
        /// <param name="target"></param>
        /// <param name="hitModifier"></param>
        /// <param name="npcDamage"></param>
        public virtual void ModifyHitNPC_Inactive(BaseFairyProjectile fairyProj,NPC target,NPC.HitModifiers hitModifier, ref int npcDamage)
        {

        }

        /// <summary>
        /// 在与墙壁发生碰撞时调用
        /// <param name="fairyProj"></param>
        /// <param name="oldVelocity"></param>
        /// </summary>
        public virtual void OnTileCollide(BaseFairyProjectile fairyProj, Vector2 oldVelocity)
        {

        } 

        public virtual void PreDrawSpecial(ref Color lightColor)
        {

        }

        public virtual void PostDrawSpecial(Color lightColor)
        {

        }
    }
}
