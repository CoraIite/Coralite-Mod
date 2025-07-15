using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 仙灵技能<br></br>
    /// 基类自带自动加载本地化词条<see cref="LocalizedText"/>
    /// </summary>
    public abstract class FairySkill : ModTexturedType, ILocalizedModType
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public int Type { get; internal set; }

        /// <summary>
        /// 仅在<see cref="FairyLoader"/>内存储的实例才能使用该属性，其余时候均为null
        /// </summary>
        public LocalizedText SkillName { get; set; }

        public string LocalizationCategory => "Systems.FairySkill";

        public virtual Color SkillTextColor { get => Color.White; }

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

            PropertyInfo[] infos = GetType().GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
            if (infos != null && infos.Length > 0)
                foreach (var propinfo in infos)
                {
                    if (propinfo.PropertyType == typeof(LocalizedText))
                        propinfo.SetValue(this, this.GetLocalization(propinfo.Name));
                }
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
                FairyLoader.GetFairySkill(Type).SkillName.Value, true, true);
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

        public virtual void SendExtra(BinaryWriter writer) { }

        public virtual void ReceiveExtra(BinaryReader reader) { }

        public virtual void PreDrawSpecial(ref Color lightColor)
        {

        }

        public virtual void PostDrawSpecial(Color lightColor)
        {

        }

        #region 描述部分

        /// <summary>
        /// 获取技能描述的总大小
        /// </summary>
        /// <returns></returns>
        public Vector2 GetSkillTipTotalSize(Player player, FairyIV iv, out Vector2 nameSize)
        {
            Texture2D tex = FairySystem.FairySkillAssets[Type].Value;

            float x = tex.Width + 10;

            string name = GetSkillNameTip(iv.SkillLevel);
            nameSize = Helper.GetStringSize(name, Vector2.One * 1.1f);

            string description = GetSkillTips(player, iv);
            Vector2 describSize = Helper.GetStringSize(description, Vector2.One * 0.9f);

            //宽度是图片宽度+10+描述的最大宽度
            x += Math.Max(nameSize.X, describSize.X) + 8;
            //高度是图片高度和描述高度中的最大值
            float y = Math.Max(nameSize.Y + describSize.Y + 8, tex.Height);

            return new Vector2(x, y);
        }

        /// <summary>
        /// 绘制技能描述
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="player"></param>
        /// <param name="iv"></param>
        /// <param name="size"></param>
        /// <param name="nameSize"></param>
        public void DrawSkillTip(Vector2 topLeft, Player player, FairyIV iv, Vector2 size, Vector2 nameSize)
        {
            Texture2D tex = FairySystem.FairySkillAssets[Type].Value;
            tex.QuickCenteredDraw(Main.spriteBatch, topLeft + new Vector2(tex.Width / 2, size.Y / 2));

            topLeft.X += tex.Width + 10;
            topLeft.Y +=  4;
            //int level = iv.SkillLevel;
            //if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            //    level = fcp.FairySkillBonus[Type].ModifyLevel(level);

            Utils.DrawBorderString(Main.spriteBatch, GetSkillNameTip(iv.SkillLevel), topLeft
                , FairyIV.GetFairyIVColorAndText(iv.SkillLevelLevel).Item1, 1.1f);

            topLeft.Y += nameSize.Y+4;

            Utils.DrawBorderString(Main.spriteBatch, GetSkillTips(player, iv), topLeft
                , Color.White, 0.9f);
        }

        /// <summary>
        /// 获取技能名称的信息
        /// </summary>
        public string GetSkillNameTip(int skillLevel)
        {
            return FairySystem.SkillLVTips.Format(SkillName.Value, skillLevel);
        }

        /// <summary>
        /// 获得技能描述文本
        /// </summary>
        public virtual string GetSkillTips(Player player, FairyIV iv)
        {
            return "";
        }

        #endregion
    }
}
