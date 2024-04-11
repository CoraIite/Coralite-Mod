using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class FairyItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        #region Fields

        /// <summary>
        /// 是否是一个仙灵，如果是的话能够放入仙灵瓶中
        /// </summary>
        public bool IsFairy;

        /// <summary>
        /// 仙灵在射出时的弹幕类型<br></br>
        /// </summary>
        public int? fairyProjType;

        /// <summary>
        /// 仙灵弹幕的基础大小，默认1
        /// </summary>
        private float baseScale = 1;
        /// <summary>
        /// 仙灵弹幕的基础伤害<br></br>
        /// 默认0
        /// </summary>
        private float baseDamage;

        /// <summary>
        /// 仙灵弹幕的默认防御，1防御能够抵挡1伤害<br></br>
        /// 默认0
        /// </summary>
        private int baseDefence;

        /// <summary>
        /// 仙灵弹幕的基础血量，默认10
        /// </summary>
        private int baseLifeMax = 10;

        /// <summary>
        /// 仙灵的个体数据，用于存放各类增幅
        /// </summary>
        public FairyData fairyData;

        #endregion

        /// <summary>
        /// 受到个体值加成过的仙灵自身的伤害
        /// </summary>
        public float FairyDamage => fairyData.damageBonus.ApplyTo(baseDamage);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的大小
        /// </summary>
        public float FairyScale => fairyData.scaleBonus.ApplyTo(baseScale);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的防御
        /// </summary>
        public float FairyDefence => fairyData.defecceBonus.ApplyTo(baseDefence);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的生命值上限
        /// </summary>
        public float FairyLifeMax => fairyData.lifeBonus.ApplyTo(baseLifeMax);

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
        }

        #region HelperMethods

        /// <summary>
        /// 用于获取仙灵的实际伤害
        /// </summary>
        /// <returns></returns>
        public int GetFairyDamage(Player owner)
        {
            //由仙灵的基础伤害再用个体值增幅一下
            float bonusedDamage = FairyDamage;
            if (owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                bonusedDamage = fcp.FairyDamageBonus(bonusedDamage);
            return (int)bonusedDamage;
        }

        /// <summary>
        /// 将物品标记为仙灵物品，之后可以将仙灵放入仙灵瓶或者射出
        /// </summary>
        /// <param name="item">自身物品</param>
        /// <param name="fairyProjType"></param>
        /// <param name="baseDamage"></param>
        /// <param name="baseDefence"></param>
        /// <param name="baseLifeMax"></param>
        /// <param name="aseScale"></param>
        public static void FairyItemSets(Item item, int fairyProjType, int baseDamage, int baseDefence, int baseLifeMax, float baseScale)
        {
            if (item.TryGetGlobalItem(out FairyItem fi))
            {
                fi.IsFairy = true;
                fi.fairyProjType = fairyProjType;
                fi.baseDamage = baseDamage;
                fi.baseDefence = baseDefence;
                fi.baseLifeMax = baseLifeMax;
                fi.baseScale = baseScale;
            }
        }

        #endregion

        #region IO

        public override void SaveData(Item item, TagCompound tag)
        {
            base.SaveData(item, tag);
        }

        public override void LoadData(Item item, TagCompound tag)
        {
        }

        #endregion
    }
}
