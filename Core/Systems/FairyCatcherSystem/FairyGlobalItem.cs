using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class FairyGlobalItem : GlobalItem
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
        public float baseScale = 1;
        /// <summary>
        /// 仙灵弹幕的基础伤害<br></br>
        /// 默认0
        /// </summary>
        public float baseDamage;

        /// <summary>
        /// 仙灵弹幕的默认防御，1防御能够抵挡1伤害<br></br>
        /// 默认0
        /// </summary>
        public int baseDefence;

        /// <summary>
        /// 仙灵弹幕的基础血量，默认10
        /// </summary>
        public int baseLifeMax = 10;

        #endregion


        #region HelperMethods

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
            if (item.TryGetGlobalItem(out FairyGlobalItem fi))
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
