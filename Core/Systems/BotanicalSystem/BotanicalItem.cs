using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Coralite.Helpers;

namespace Coralite.Core.Systems.BotanicalSystem
{
    public class BotanicalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public bool botanicalItem = false;
        public bool isIdentified = false;

        public int DominantGrowTime = 100;//显性植物生长速度基因
        public int RecessiveGrowTime = 100;//隐性植物生长速度基因

        public int DominantLevel;//显性强度基因
        public int RecessiveLevel;//隐性强度基因

        public Dictionary<int, CrossBreedData> CrossBreedDatas = null;

        public override GlobalItem Clone(Item from, Item to)
        {
            if (from == null || to == null)
                return base.Clone(from, to);
            if (from.IsAir || to.IsAir)
                return base.Clone(from, to);
            BotanicalItem BFrom = from.GetBotanicalItem();
            BotanicalItem BTo = to.GetBotanicalItem();

            if (!BFrom.botanicalItem)
                return base.Clone(from, to);

            BTo.DominantGrowTime = BFrom.DominantGrowTime;
            BTo.RecessiveGrowTime = BFrom.RecessiveGrowTime;
            BTo.DominantLevel = BFrom.DominantLevel;
            BTo.RecessiveLevel = BFrom.RecessiveLevel;
            return base.Clone(from, to);
        }

        public override bool CanStack(Item item1, Item item2)
        {
            if (item1 == null || item2 == null)
                return false;

            if (item1.IsAir || item2.IsAir)
                return false;

            BotanicalItem botanicalItem1 = item1.GetBotanicalItem();
            BotanicalItem botanicalItem2 = item2.GetBotanicalItem();

            if (!botanicalItem1.botanicalItem)
                return base.CanStack(item1, item2);
            if (!botanicalItem2.botanicalItem)
                return base.CanStack(item1, item2);

            if (botanicalItem1.isIdentified != botanicalItem2.isIdentified)
                return false;

            if (botanicalItem1.DominantGrowTime != botanicalItem2.DominantGrowTime)
                return false;
            if (botanicalItem1.RecessiveGrowTime != botanicalItem2.RecessiveGrowTime)
                return false;

            if (botanicalItem1.DominantLevel != botanicalItem2.DominantLevel)
                return false;
            if (botanicalItem1.RecessiveLevel != botanicalItem2.RecessiveLevel)
                return false;

            if (item1.stack == item1.maxStack || item2.stack == item2.maxStack)
                return false;

            return true;
        }

        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.Daybloom://太阳花
                    botanicalItem = true;

                    break;
                case ItemID.Blinkroot://闪耀根
                    botanicalItem = true;

                    break;
                case ItemID.Moonglow://月光草
                    botanicalItem = true;

                    break;
                case ItemID.Waterleaf://幌菊
                    botanicalItem = true;

                    break;
                case ItemID.Shiverthorn://寒颤棘
                    botanicalItem = true;

                    break;
                case ItemID.Deathweed://死亡草
                    botanicalItem = true;

                    break;
                case ItemID.Fireblossom://火焰花
                    botanicalItem = true;

                    break;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!botanicalItem)
                return;

            string Genotypes;
            if (isIdentified)
                Genotypes = $"生长时间: {DominantGrowTime}\n";
            else
                Genotypes = "该植物还未被鉴定";

            TooltipLine line = new TooltipLine(Mod, "", Genotypes);
            if (!isIdentified)
                line.OverrideColor = Color.Gray;

            tooltips.Add(line);
        }

    }
}
