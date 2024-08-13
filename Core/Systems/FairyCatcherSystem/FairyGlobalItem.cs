using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class FairyGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        #region Fields

        /// <summary>
        /// 捕捉力
        /// </summary>
        public int CatchPower;

        /// <summary>
        /// 前缀的捕捉力加成
        /// </summary>
        public float CatchPowerMult = 1;

        /// <summary>
        /// 是否是一个仙灵，如果是的话能够放入仙灵瓶中
        /// </summary>
        public bool IsFairy;

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
        /// <param name="baseScale"></param>
        public static void FairyItemSets(Item item, int baseDamage, int baseDefence, int baseLifeMax, float baseScale = 1)
        {
            if (item.TryGetGlobalItem(out FairyGlobalItem fi))
            {
                fi.IsFairy = true;
                fi.baseDamage = baseDamage;
                fi.baseDefence = baseDefence;
                fi.baseLifeMax = baseLifeMax;
                fi.baseScale = baseScale;
            }
        }

        public void FairyItemSets(int baseDamage, int baseDefence, int baseLifeMax, float baseScale = 1)
        {
            IsFairy = true;
            this.baseDamage = baseDamage;
            this.baseDefence = baseDefence;
            this.baseLifeMax = baseLifeMax;
            this.baseScale = baseScale;
        }

        #endregion

        public override bool CanPickup(Item item, Player player)
        {
            //自身是仙灵物品并且身上有空的仙灵瓶
            if (item.TryGetGlobalItem(out FairyGlobalItem fgi) && fgi.IsFairy)
            {
                if (player.TryGetModPlayer(out FairyCatcherPlayer fcp)
                    && fcp.FairyCatch_GetEmptyFairyBottle(out IFairyBottle bottle, out int emptySlot))
                {
                    bottle.Fairies[emptySlot] = item.Clone();

                    PopupText.NewText(PopupTextContext.RegularItemPickup, item, item.stack, noStack: true, longText: false);

                    item.TurnToAir();
                    SoundEngine.PlaySound(CoraliteSoundID.Grab, player.Center);

                    //if (UILoader.GetUIState<FairyBottleUI>().visible)
                    //{
                    //UILoader.GetUIState<FairyBottleUI>().Recalculate();
                    //}

                    return false;
                }
            }

            return base.CanPickup(item, player);
        }

        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            if (item.DamageType != FairyDamage.Instance)
                return base.ChoosePrefix(item, rand);

            int prefix = 0;
            var wr = new WeightedRandom<int>(rand);

            void AddCategory()
            {
                IReadOnlyList<ModPrefix> list = PrefixLoader.GetPrefixesInCategory(PrefixCategory.Custom);
                foreach (ModPrefix modPrefix in list.Where(x => x.CanRoll(item)))
                {
                    wr.Add(modPrefix.Type, modPrefix.RollChance(item));
                }
            }

            AddCategory();

            for (int i = 0; i < 50; i++)
            {
                prefix = wr.Get();
            }

            return prefix;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.DamageType == FairyDamage.Instance
              )
            {
                int index = tooltips.FindIndex(line => line.Name == "Damage");
                if (index != -1)
                {
                    TooltipLine catchPowerLine =
                        new(Mod, "CatchPower",
                        FairySystem.CatchPowerMult.Format((Main.LocalPlayer.GetModPlayer<FairyCatcherPlayer>().GetBonusedCatchPower(CatchPower, CatchPowerMult)).ToString() + " "));

                    tooltips.Insert(index + 1, catchPowerLine);
                }
            }
        }

        #region IO

        public override void SaveData(Item item, TagCompound tag)
        {
            tag.Add("CatchPowerMult", CatchPowerMult);
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            CatchPowerMult = tag.GetFloat("CatchPowerMult");
        }

        #endregion
    }
}
