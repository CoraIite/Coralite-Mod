using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Content.NPCs.GlobalNPC
{
    public class DownedGolemCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedGolemBoss;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => "击败石巨人后掉落";
    }
}