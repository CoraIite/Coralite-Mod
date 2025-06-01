using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Utilities;

namespace Coralite.Content.GlobalItems
{
    public partial class CoraliteGlobalItem
    {
        #region Fields

        /// <summary>
        /// 仙灵弹幕的基础大小，默认1
        /// </summary>
        public float baseScale = 1;

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
        public static void FairyItemSets(Item item, int baseDefence, int baseLifeMax, float baseScale = 1)
        {
            if (item.TryGetGlobalItem(out CoraliteGlobalItem fi))
            {
                fi.baseDefence = baseDefence;
                fi.baseLifeMax = baseLifeMax;
                fi.baseScale = baseScale;
            }
        }

        public void FairyItemSets(int baseDefence, int baseLifeMax, float baseScale = 1)
        {
            this.baseDefence = baseDefence;
            this.baseLifeMax = baseLifeMax;
            this.baseScale = baseScale;
        }

        #endregion

        public override bool CanPickup(Item item, Player player)
        {
            //自身是仙灵物品并且身上有空的仙灵瓶
            if (item.TryGetGlobalItem(out CoraliteGlobalItem fgi) && CoraliteSets.Items.IsFairy[item.type])
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

        public void ModifyFairyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.DamageType == FairyDamage.Instance&& item.ModItem is BaseFairyCatcher bfc)
            {
                int index = tooltips.FindIndex(line => line.Name == "Damage");
                if (index != -1)
                {
                    TooltipLine catchPowerLine =
                        new(Mod, "CatchPower",
                        FairySystem.CatchPowerMult.Format(Main.LocalPlayer.GetModPlayer<FairyCatcherPlayer>()
                        .GetBonusedCatchPower(bfc.CatchPower, bfc.CatchPowerMult).ToString() + " "));

                    tooltips.Insert(index + 1, catchPowerLine);
                }
            }
        }
    }
}
