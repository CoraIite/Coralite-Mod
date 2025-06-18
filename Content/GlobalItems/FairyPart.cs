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
        public override bool CanPickup(Item item, Player player)
        {
            //自身是仙灵物品并且身上有空的仙灵瓶
            if (item.TryGetGlobalItem(out CoraliteGlobalItem fgi) && CoraliteSets.Items.IsFairy[item.type])
            {
                if (player.TryGetModPlayer(out FairyCatcherPlayer fcp)
                    && fcp.FairyCatch_GetEmptyFairyBottle(out BaseFairyBottle bottle, out int emptySlot))
                {
                    //TODO：添加适配战斗仙灵
                    bottle.ContainFairies[emptySlot] = item.Clone();

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
                    wr.Add(modPrefix.Type, modPrefix.RollChance(item));
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
            if (item.DamageType == FairyDamage.Instance && item.ModItem is BaseFairyCatcher bfc)
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
