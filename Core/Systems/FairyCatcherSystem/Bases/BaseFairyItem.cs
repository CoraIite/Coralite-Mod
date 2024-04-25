using Coralite.Content.DamageClasses;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyItem : ModItem, IFairyItem
    {
        public override string Texture => AssetDirectory.FairyItems + Name;

        /// <summary>
        /// 仙灵的个体数据，用于存放各类增幅
        /// </summary>
        protected FairyData fairyData = new FairyData();

        /// <summary> 仙灵的实际血量 </summary>
        protected int life;
        /// <summary> 仙灵是否存活 </summary>
        protected bool dead;
        /// <summary>
        /// 复活时间
        /// </summary>
        protected int resurrectionTime;

        public abstract int FairyType { get; }
        public abstract FairyAttempt.Rarity Rarity { get; }
        public FairyData IV { get => fairyData; set => fairyData = value; }
        public bool IsDead => dead;
        public int Life { get => life; set => life = value; }
        public virtual int MaxResurrectionTime => 60 * 60 * 3;

        /// <summary>
        /// 受到个体值加成过的仙灵自身的伤害
        /// </summary>
        public float FairyDamage => fairyData.damageBonus.ApplyTo(Item.GetGlobalItem<FairyGlobalItem>().baseDamage);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的大小
        /// </summary>
        public float FairyScale
        {
            get
            {
                float scale = fairyData.scaleBonus * Item.GetGlobalItem<FairyGlobalItem>().baseScale;
                scale = Math.Clamp(scale, 0.5f, 2.5f);
                return scale;
            }
        }
        /// <summary>
        /// 受到个体值加成过的仙灵自身的防御
        /// </summary>
        public float FairyDefence => fairyData.defenceBonus.ApplyTo(Item.GetGlobalItem<FairyGlobalItem>().baseDefence);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的生命值上限
        /// </summary>
        public float FairyLifeMax => fairyData.lifeMaxBonus.ApplyTo(Item.GetGlobalItem<FairyGlobalItem>().baseLifeMax);

        public sealed override void SetDefaults()
        {
            SetOtherDefaults();
            if (Item.TryGetGlobalItem(out FairyGlobalItem fairyItem))
            {
                fairyItem.IsFairy = true;
                SetFairyDefault(fairyItem);
            }
        }

        public virtual void SetOtherDefaults() { }

        /// <summary>
        /// 在这里设置仙灵的<br></br>
        /// <see cref="FairyGlobalItem.baseDamage"/><br></br>
        /// <see cref="FairyGlobalItem.baseDefence"/><br></br>
        /// <see cref="FairyGlobalItem.baseLifeMax"/><br></br>
        /// <see cref="FairyGlobalItem.baseLifeMax"/><br></br>
        /// 等字段
        /// </summary>
        /// <param name="fairyItem"></param>
        public virtual void SetFairyDefault(FairyGlobalItem fairyItem) { }

        public override ModItem Clone(Item newEntity)
        {
            ModItem modItem = base.Clone(newEntity);
            if (modItem != null)
            {
                (modItem as IFairyItem).IV = IV;
            }

            return modItem;
        }

        public virtual bool Hurt(Player owner, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int damage = target.damage;

            damage -= (int)FairyDefence;

            if (damage < 1)
                damage = 1;

            life -= damage;
            if (life <= 0)
                Dead(owner, target);
            LimitLife();

            return dead;
        }

        public void Dead(Player owner, NPC target)
        {
            dead = true;

            int time = MaxResurrectionTime;
            if (owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {

            }

            resurrectionTime = time;

            OnDead(owner, target);
        }

        public virtual void OnDead(Player owner, NPC target) { }

        /// <summary>
        /// 将生命值限制在0-最大值之间
        /// </summary>
        public void LimitLife()
        {
            life = Math.Clamp(life, 0, (int)FairyLifeMax);
        }

        /// <summary>
        /// 将仙灵发射出去
        /// </summary>
        /// <returns></returns>
        public virtual bool ShootFairy(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int catcherDamage, float knockBack)
        {
            if (dead)
                return false;

            catcherDamage += (int)player.GetTotalDamage<FairyDamage>().ApplyTo(Item.damage);

            //生成仙灵弹幕
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, Item.shoot, catcherDamage, knockBack, player.whoAmI);
            //将弹幕的item赋值为自身
            if (proj.ModProjectile is IFairyProjectile fairyProjectile)

                fairyProjectile.FairyItem = this;
            return true;
        }

        #region 描述相关

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //稀有度
            tooltips.Add(RarityDescription());
            //当前血量
            tooltips.Add(SurvivalStatus());

            //各种增幅数值
            tooltips.Add(LifeMaxeBonusDescription());
            tooltips.Add(DamageBonusDescription());
            tooltips.Add(DefenceBonusDescription());
            tooltips.Add(ScaleBonusDescription());
        }

        public TooltipLine RarityDescription()
        {
            string description;
            Color color;

            switch (Rarity)
            {
                case FairyAttempt.Rarity.C:
                    color = FairySystem.RarityC_LiteYellow;
                    description = FairySystem.Rarity_C.Value;
                    break;
                case FairyAttempt.Rarity.U:
                    color = FairySystem.RarityU_LawnGreen;
                    description = FairySystem.Rarity_U.Value;
                    break;
                case FairyAttempt.Rarity.R:
                    color = FairySystem.RarityR_SkyBlue;
                    description = FairySystem.Rarity_R.Value;
                    break;
                case FairyAttempt.Rarity.RR:
                    color = FairySystem.RarityRR_Blue;
                    description = FairySystem.Rarity_RR.Value;
                    break;
                case FairyAttempt.Rarity.SR:
                    color = FairySystem.RaritySR_Red;
                    description = FairySystem.Rarity_SR.Value;
                    break;
                case FairyAttempt.Rarity.UR:
                    color = FairySystem.RarityUR_Orange;
                    description = FairySystem.Rarity_UR.Value;
                    break;
                case FairyAttempt.Rarity.RRR:
                    color = FairySystem.RarityRRR_BluePurple;
                    description = FairySystem.Rarity_RRR.Value;
                    break;
                case FairyAttempt.Rarity.HR:
                    color = FairySystem.RarityHR_Pink;
                    description = FairySystem.Rarity_HR.Value;
                    break;
                case FairyAttempt.Rarity.AR:
                    color = FairySystem.RarityAR_Orchid;
                    description = FairySystem.Rarity_AR.Value;
                    break;
                case FairyAttempt.Rarity.MR:
                    color = FairySystem.RarityMR_Gold;
                    description = FairySystem.Rarity_MR.Value;
                    break;
                default:
                    {
                        //特殊
                        color = Color.MediumVioletRed;
                        description = FairySystem.Rarity_SP.Value;
                    }
                    break;
            }

            TooltipLine line = new TooltipLine(Mod, "FairyRarity", description);
            line.OverrideColor = color;

            return line;
        }

        public TooltipLine SurvivalStatus()
        {
            string status;
            Color newColor;
            if (dead)
            {
                newColor = Color.OrangeRed;
                status = FairySystem.ResurrectionTime.Format($"{resurrectionTime / (60 * 60)}:{resurrectionTime / 60}");
            }
            else
            {
                newColor = Color.LightGreen;
                status = FairySystem.CurrentLife.Format(life, (int)FairyLifeMax);
            }

            TooltipLine line = new TooltipLine(Mod, "SurvivalStatus", status);
            line.OverrideColor = newColor;

            return line;
        }

        public virtual TooltipLine LifeMaxeBonusDescription()
        {
            float @base = Item.GetGlobalItem<FairyGlobalItem>().baseLifeMax;
            float bonused = FairyLifeMax;
            (Color, LocalizedText) group = FairyIVAppraise.FairyLifeMaxAppraise.GetAppraiseResult(@base, bonused);

            TooltipLine line = new TooltipLine(Mod, "LifeMaxBonus"
                , FairySystem.FormatIVDescription(FairySystem.FairyLifeMax, group.Item2, @base, (int)bonused));
            line.OverrideColor = group.Item1;
            return line;
        }

        public virtual TooltipLine DamageBonusDescription()
        {
            float @base = Item.GetGlobalItem<FairyGlobalItem>().baseDamage;
            float bonused = FairyDamage;
            (Color, LocalizedText) group = FairyIVAppraise.FairyDamageAppraise.GetAppraiseResult(@base, bonused);

            TooltipLine line = new TooltipLine(Mod, "DamageBonus"
                , FairySystem.FormatIVDescription(FairySystem.FairyDamage, group.Item2, @base, (int)bonused));
            line.OverrideColor = group.Item1;
            return line;
        }

        public virtual TooltipLine DefenceBonusDescription()
        {
            float @base = Item.GetGlobalItem<FairyGlobalItem>().baseDefence;
            float bonused = FairyDefence;
            (Color, LocalizedText) group = FairyIVAppraise.FairyDefenceAppraise.GetAppraiseResult(@base, bonused);

            TooltipLine line = new TooltipLine(Mod, "DefenceBonus"
                , FairySystem.FormatIVDescription(FairySystem.FairyDefence, group.Item2, @base, (int)bonused));
            line.OverrideColor = group.Item1;
            return line;
        }

        public virtual TooltipLine ScaleBonusDescription()
        {
            float @base = Item.GetGlobalItem<FairyGlobalItem>().baseScale;
            float bonused = MathF.Round(FairyScale, 2);

            TooltipLine line = new TooltipLine(Mod, "ScaleBonus"
                , FairySystem.FairyScale.Value + $"{bonused} ({@base})");
            return line;
        }

        #endregion

        public override void SaveData(TagCompound tag)
        {
            fairyData.SaveData(tag);
            tag.Add("Life", life);
            if (dead)
                tag.Add("ResurrectionTime", resurrectionTime);
        }

        public override void LoadData(TagCompound tag)
        {
            fairyData = new FairyData();
            fairyData.LoadData(tag);
            life = tag.GetInt("Life");
            if (tag.TryGet("ResurrectionTime", out int time))
            {
                dead = true;
                resurrectionTime = time;
            }
        }
    }

    public interface IFairyItem
    {
        public bool IsDead { get; }
        public FairyData IV { get; set; }
        public int Life { get; set; }
        public float FairyLifeMax { get; }
        public int FairyType { get; }
        public FairyAttempt.Rarity Rarity { get; }

        /// <summary>
        /// 返回值是仙灵是否死亡
        /// </summary>
        /// <param name="target"></param>
        /// <param name="hit"></param>
        /// <param name="damageDon"></param>
        /// <returns></returns>
        public bool Hurt(Player owner, NPC target, NPC.HitInfo hit, int damageDon);
        public bool ShootFairy(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int catcherDamage, float knockBack);
    }
}
