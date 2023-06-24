using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem
{
    public class MagikeItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => true;
        public override bool InstancePerEntity => true;

        public int magiteAmount = -1;

        public int magikeRemodelRequired = -1;
        public int stackRemodelRequired;
        public IMagikeRemodelCondition condition = null;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (magiteAmount>0)
            {
                string magikeAmount = $"魔能含量: {magiteAmount}";
                TooltipLine line = new TooltipLine(Mod, "magiteAmount", magikeAmount);
                if (magiteAmount < 300)
                    line.OverrideColor = Coralite.Instance.MagicCrystalPink;
                else if (magiteAmount < 1000)
                    line.OverrideColor = Coralite.Instance.CrystallineMagikePurple;
                tooltips.Add(line);
            }

            if (magikeRemodelRequired > 0)
            {
                string stackAmount = $"物品需求量： {stackRemodelRequired}\n";
                string magikeAmount = $"消耗魔能： {magikeRemodelRequired}";
                string conditionNeed = condition == null ? "" : ("\n" + condition.Description);
                TooltipLine line = new TooltipLine(Mod, "remodelConition", stackAmount+magikeAmount+conditionNeed);
                if (magikeRemodelRequired < 300)
                    line.OverrideColor = Coralite.Instance.MagicCrystalPink;
                else if (magikeRemodelRequired < 1000)
                    line.OverrideColor = Coralite.Instance.CrystallineMagikePurple;
                //else if (true)
                //{

                //}
                tooltips.Add(line);
            }
        }
    }
}
