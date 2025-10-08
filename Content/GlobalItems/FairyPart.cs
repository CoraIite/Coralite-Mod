using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Utilities;

namespace Coralite.Content.GlobalItems
{
    public partial class CoraliteGlobalItem
    {
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
