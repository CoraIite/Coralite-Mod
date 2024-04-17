using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public abstract class BaseFairyItem : ModItem
    {
        /// <summary>
        /// 仙灵的个体数据，用于存放各类增幅
        /// </summary>
        public FairyData fairyData;

        /// <summary>
        /// 仙灵的实际血量
        /// </summary>
        public int life;
        /// <summary>
        /// 仙灵是否存活
        /// </summary>
        public bool dead;

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

        ///// <summary>
        ///// 用于获取仙灵的实际伤害
        ///// </summary>
        ///// <returns></returns>
        //public int GetFairyDamage(Player owner)
        //{
        //    //由仙灵的基础伤害再用个体值增幅一下
        //    float bonusedDamage = FairyDamage;
        //    if (owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
        //        bonusedDamage = fcp.FairyDamageBonus(bonusedDamage);
        //    return (int)bonusedDamage;
        //}

        public virtual void Hurt()
        {

        }

        /// <summary>
        /// 将生命值限制在0-最大值之间
        /// </summary>
        public void LimitLife()
        {
            life = Math.Clamp(life, 0, (int)FairyLifeMax);
        }

        public abstract Fairy GetFairy();

        /// <summary>
        /// 将仙灵发射出去
        /// </summary>
        /// <returns></returns>
        public virtual bool ShootFairy(Vector2 position,Vector2 velocity)
        {
            if (dead)
                return false;   

            //生成仙灵弹幕

            //将弹幕的item赋值为自身

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }
    }
}
