using Coralite.Content.Biomes;
using Coralite.Content.ModPlayers;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.WorldValueSystem;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.GlobalNPCs
{
    public class DownedGolemCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedGolemBoss;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => CoraliteConditions.DownedGolemCondition.Value;
    }

    public class InCoralCatWorld : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => CoraliteWorld.CoralCatWorld;
        public bool CanShowItemDropInUI() => CoraliteWorld.CoralCatWorld;
        public string GetConditionDescription() => CoraliteConditions.CoralCat.Description.Value;
    }

    public class DropSoulOfDeveloperCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info)
            => !info.npc.friendly && !info.npc.SpawnedFromStatue && info.npc.lifeMax > 1 && info.npc.value > 1 && !info.npc.boss && !NPCID.Sets.CannotDropSouls[info.npc.type]
            && CoraliteWorld.HasPermission
            && info.player.InModBiome<CrystallineSkyIsland>();

        public bool CanShowItemDropInUI()
            => CoraliteWorld.HasPermission && Main.LocalPlayer.InModBiome<CrystallineSkyIsland>();

        public string GetConditionDescription() => CoraliteConditions.InCrystallineSkyIsland.Description.Value;
    }
}