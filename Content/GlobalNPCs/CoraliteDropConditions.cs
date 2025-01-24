using Coralite.Core;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Content.GlobalNPCs
{
    public class DownedGolemCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedGolemBoss;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => CoraliteConditions.DownedGolemCondition.Value;
    }
}