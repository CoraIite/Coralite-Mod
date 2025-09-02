using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
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

        /// <summary>
        /// 记录了技能的标签
        /// </summary>
        public static Dictionary<int, int[]> SkillWithTags { get; set; }


        public int Type { get; internal set; }

        /// <summary>
        /// 仅在<see cref="FairyLoader"/>内存储的实例才能使用该属性，其余时候均为null
        /// </summary>
        public LocalizedText SkillName { get; set; }

        public string LocalizationCategory => "Systems.FairySkill";

        public virtual Color SkillTextColor { get => Color.White; }

        /// <summary>
        /// 技能集合的Type，默认<see cref="null"/>
        /// </summary>
        public virtual int[] SkillTags => null;

        /// <summary>
        /// 技能计时器
        /// </summary>
        protected int SkillTimer { get; set; }
        /// <summary>
        /// 每隔多久寻找一次最近的敌人
        /// </summary>
        public virtual int TargetClosestTime { get => 45; }

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

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        public override void SetStaticDefaults()
        {
            SkillWithTags ??= new Dictionary<int, int[]>();
            int[] skillTags = SkillTags;
            if (skillTags != null)
                SkillWithTags.Add(Type, skillTags);
        }

        public virtual FairySkill NewInstance()
        {
            var inst = (FairySkill)Activator.CreateInstance(GetType(), true);
            inst.Type = Type;
            return inst;
        }

        /// <summary>
        /// 获取耐力消耗
        /// </summary>
        public virtual int GetStaminaCost(int skilLevel)
        {
            return 1;
        }

        /// <summary>
        /// 技能伤害
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public abstract int GetDamage(int baseDamage, int skillLevel);

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
        /// 是否能使用，被动技能返回<see cref="false"/>
        /// </summary>
        /// <param name="fairyProj"></param>
        /// <returns></returns>
        public virtual bool CanUseSkill(BaseFairyProjectile fairyProj)
        {
            return true;
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

        public virtual void PreDrawSpecial(BaseFairyProjectile fairyProj, ref Color lightColor)
        {

        }

        public virtual void PostDrawSpecial(BaseFairyProjectile fairyProj, Color lightColor)
        {

        }

        #region 帮助方法

        /// <summary>
        /// 设置仙灵的朝向
        /// </summary>
        /// <param name="fairyProj"></param>
        /// <param name="target"></param>
        public virtual void SetDirection(BaseFairyProjectile fairyProj, NPC target)
        {
            if (MathF.Abs(fairyProj.Projectile.Center.X - target.Center.X) > 8)
                fairyProj.Projectile.spriteDirection = target.Center.X > fairyProj.Projectile.Center.X ? 1 : -1;
        }

        #endregion


        #region 描述部分

        public struct TipSize
        {
            public Vector2 damageSize;
            public Vector2 staminaCostSize;

            public Vector2 totalSize;
            public Vector2 nameSize;
        }

        /// <summary>
        /// 获取技能描述的总大小
        /// </summary>
        /// <returns></returns>
        public TipSize GetSkillTipTotalSize(Player player, FairyIV iv)
        {
            Texture2D tex = FairyAsset.FairySkillAssets[Type].Value;

            float x = tex.Width + 10;

            //名称的长度
            string name = GetSkillNameTip(iv.SkillLevel);
            Vector2 nameSize = Helper.GetStringSize(name, Vector2.One * 1.1f);

            //伤害 / 耐力消耗的长度
            int skillLevel = Helper.GetBonusedSkillLevel(player, iv.SkillLevel, Type);

            string damage = FairySystem.SkillDamage.Value;
            int damage2 = GetDamage(iv.Damage, skillLevel);
            string damageValue = damage2 > 0 ? damage2.ToString() : " - ";

            Vector2 damageSize = Helper.GetStringSize(damage, Vector2.One * 0.9f);
            Vector2 damageValueSize = Helper.GetStringSize(damageValue, Vector2.One * 0.9f);


            string staminaCost = FairySystem.SkillStaminaCost.Value;
            string staminaCostValue = GetStaminaCost(skillLevel).ToString();

            Vector2 staminaCostSize = Helper.GetStringSize(staminaCost, Vector2.One * 0.9f);
            Vector2 staminaCostValueSize = Helper.GetStringSize(staminaCostValue, Vector2.One * 0.9f);

            damageSize = new Vector2(MathF.Max(damageSize.X, damageValueSize.X) + 20, damageSize.Y);
            staminaCostSize = new Vector2(MathF.Max(staminaCostSize.X, staminaCostValueSize.X) + 20, staminaCostSize.Y);


            //描述的长度
            string description = GetSkillTipsInner(player, iv);
            Vector2 describSize = Helper.GetStringSize(description, Vector2.One * 0.9f);

            //宽度是图片宽度+10+描述的最大宽度
            x += Math.Max(Math.Max(nameSize.X, describSize.X), damageSize.X + 8 + staminaCostSize.X) + 8;
            //高度是图片高度和描述高度中的最大值
            float y = describSize.Y + 8 + tex.Height;

            return new TipSize()
            {
                nameSize = nameSize,
                totalSize = new Vector2(x, y + damageSize.Y * 2),

                damageSize = damageSize,
                staminaCostSize = staminaCostSize,
            };
        }

        public Vector2 GetSkillTipSizeForUI()
        {
            Texture2D tex = FairyAsset.FairySkillAssets[Type].Value;

            float x = tex.Width + 10;

            string name = SkillName.Value;
            Vector2 nameSize = Helper.GetStringSize(name, Vector2.One * 1.1f);

            return new Vector2(x + nameSize.X, tex.Height);
        }

        public void DrawSkillTipInUI(Vector2 topLeft, Vector2 size)
        {
            Texture2D tex = FairyAsset.FairySkillAssets[Type].Value;
            tex.QuickCenteredDraw(Main.spriteBatch, topLeft + new Vector2(tex.Width / 2, size.Y / 2));

            topLeft.X += tex.Width + 10;
            topLeft.Y +=size.Y/2+4;

            Utils.DrawBorderString(Main.spriteBatch, SkillName.Value, topLeft
                , Color.White, 1.1f,0,0.5f);
        }

        /// <summary>
        /// 绘制技能描述
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="player"></param>
        /// <param name="iv"></param>
        /// <param name="size"></param>
        /// <param name="nameSize"></param>
        public void DrawSkillTip(Vector2 topLeft, Player player, FairyIV iv, TipSize sizes)
        {
            //topLeft.Y += 4;

            Texture2D tex = FairyAsset.FairySkillAssets[Type].Value;
            tex.QuickCenteredDraw(Main.spriteBatch, topLeft + tex.Size() / 2);

            //int level = iv.SkillLevel;
            //if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            //    level = fcp.FairySkillBonus[Type].ModifyLevel(level);

            topLeft.Y += tex.Height / 2;

            Utils.DrawBorderString(Main.spriteBatch, GetSkillNameTip(iv.SkillLevel), topLeft + new Vector2(tex.Width + 4, 4)
                , FairyIV.GetFairyIVColorAndText(iv.SkillLevelLV).Item1, 1.1f, 0, 0.5f);

            topLeft.Y += tex.Height / 2 + 8;

            //绘制伤害/耐力消耗文字
            Utils.DrawBorderString(Main.spriteBatch, FairySystem.SkillDamage.Value, topLeft + new Vector2(sizes.damageSize.X / 2, 0)
                , Color.White, 0.9f, 0.5f);

            Texture2D texture = TextureAssets.FishingLine.Value;
            Main.spriteBatch.Draw(texture, topLeft + new Vector2(sizes.damageSize.X + 4, sizes.damageSize.Y*0.8f), null
                , Color.White, 0, texture.Size() / 2, new Vector2(1, sizes.damageSize.Y * 1.6f / texture.Height), SpriteEffects.None, 0);
            //Utils.DrawBorderString(Main.spriteBatch, "|", topLeft + new Vector2(sizes.damageSize.X, 0)
            //    , Color.Coral, 0.9f, 0f);

            Utils.DrawBorderString(Main.spriteBatch, FairySystem.SkillStaminaCost.Value, topLeft + new Vector2(sizes.damageSize.X + 8 + sizes.staminaCostSize.X / 2, 0)
                , Color.White, 0.9f, 0.5f);


            topLeft.Y += sizes.damageSize.Y;
            int skillLevel = Helper.GetBonusedSkillLevel(player, iv.SkillLevel, Type);

            //绘制伤害/耐力消耗数字
            int damage2 = GetDamage(iv.Damage, skillLevel);
            string damageValue = damage2 > 0 ? damage2.ToString() : " - ";

            Utils.DrawBorderString(Main.spriteBatch, damageValue, topLeft + new Vector2(sizes.damageSize.X / 2, 0)
                , Color.White, 0.9f, 0.5f);

            //Utils.DrawBorderString(Main.spriteBatch, "|", topLeft + new Vector2(sizes.damageSize.X, 0)
            //    , Color.Coral, 0.9f, 0f);

            Utils.DrawBorderString(Main.spriteBatch, GetStaminaCost(skillLevel).ToString(), topLeft + new Vector2(sizes.damageSize.X + 8 + sizes.staminaCostSize.X / 2, 0)
                , Color.White, 0.9f, 0.5f);



            topLeft.Y += sizes.damageSize.Y + 4;

            Utils.DrawBorderString(Main.spriteBatch, GetSkillTipsInner(player, iv), topLeft
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
        private string GetSkillTipsInner(Player player, FairyIV iv)
        {
            string pre = "";
            if (SkillWithTags.TryGetValue(Type, out int[] tags) && tags != null)
            {
                pre = FairySystem.SkillTags.Value;
                foreach (int tagID in tags)
                {
                    FairySkillTag tag = FairyLoader.GetFairySkillTag(tagID);
                    pre = string.Concat(pre, " ", tag.Text.Value);
                }

                pre = string.Concat(pre, Environment.NewLine);
            }

            return string.Concat(pre, GetSkillTips(player, iv));
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
