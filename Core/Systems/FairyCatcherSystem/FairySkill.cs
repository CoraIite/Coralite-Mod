using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵技能
    /// </summary>
    public abstract class FairySkill : ModTexturedType, ILocalizedModType
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public int Type { get; internal set; }

        /// <summary>
        /// 仅在<see cref="FairyLoader"/>内存储的实例才能使用该属性，其余时候均为null
        /// </summary>
        public LocalizedText SkillName { get; private set; }

        public string LocalizationCategory => "Systems.FairySkill";

        public virtual Color SkillTextColor { get=> Color.White; } 

        /// <summary>
        /// 技能计时器
        /// </summary>
        protected int SkillTimer { get; set; }

        protected override void Register()
        {
            ModTypeLookup<FairySkill>.Register(this);

            FairyLoader.skills ??= new List<FairySkill>();
            FairyLoader.skills.Add(this);

            Type = FairyLoader.ReserveFairySkillID();

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
            CombatText.NewText(Utils.CenteredRectangle(pos, Vector2.One), SkillTextColor,
                FairyLoader.GetFairySkill(Type).SkillName.Value,true,true);
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

        public virtual void ModifyHitByProj(BaseFairyProjectile fairyProj, Projectile proj, ref int damage)
        {

        }

        /// <summary>
        /// 命中NPC后调用，主动效果，只有在使用该技能的时候才会触发
        /// </summary>
        /// <param name="fairyProj"></param>
        /// <param name="target"></param>
        /// <param name="hitModifier"></param>
        /// <param name="npcDamage"></param>
        public virtual void ModifyHitNPC_Active(BaseFairyProjectile fairyProj, NPC target, ref NPC.HitModifiers hitModifier, ref int npcDamage)
        {

        }

        /// <summary>
        /// 命中NPC后调用，被动效果，每次攻击到NPC都会触发
        /// </summary>
        /// <param name="fairyProj"></param>
        /// <param name="target"></param>
        /// <param name="hitModifier"></param>
        /// <param name="npcDamage"></param>
        public virtual void ModifyHitNPC_Inactive(BaseFairyProjectile fairyProj, NPC target, ref NPC.HitModifiers hitModifier, ref int npcDamage)
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

        /// <summary>
        /// 获取技能描述
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public virtual string GetSkillDescription(int skillLevel) => "";

        /// <summary>
        /// 获取仙灵技能描述的尺寸
        /// </summary>
        /// <returns></returns>
        public Vector2 GetSkillTipSize()
        {
            Texture2D tex = FairySystem.FairySkillAssets[Type].Value;
            return tex.Size();
        }

        /// <summary>
        /// 绘制仙灵描述
        /// </summary>
        /// <param name="topLeft"></param>
        public void DrawSkillTip(Vector2 topLeft)
        {

        }

        public virtual void SendExtra(BinaryWriter writer) { }

        public virtual void ReceiveExtra(BinaryReader reader) { }

        public virtual void PreDrawSpecial(ref Color lightColor)
        {

        }

        public virtual void PostDrawSpecial(Color lightColor)
        {

        }
    }
}
