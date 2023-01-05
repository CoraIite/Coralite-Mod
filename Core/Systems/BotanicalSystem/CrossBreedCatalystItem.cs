using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.BotanicalSystem
{
    public class CrossBreedCatalystItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public bool isCatalyst;
        public int mutantPower = 0;
        public int levelPower = 0;
        public int growTimePower = 0;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            //TODO:加入对催化剂的描述
        }
    }
}
