using Coralite.Content.Items.ShadowItems;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.VanillaNPC
{
    public class CoraliteGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.DarkCaster)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ShadowEnergy>(), 1, 0, 1));
            }
        }
    }
}
