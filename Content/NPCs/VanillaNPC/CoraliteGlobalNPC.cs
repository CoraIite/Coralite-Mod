using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Content.Items.Placeable;
using Coralite.Content.Items.Shadow;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Coralite.Content.Items.Weapons_Shoot;

namespace Coralite.Content.NPCs.VanillaNPC
{
    public class CoraliteGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.DarkCaster)
                npcLoot.Add(ItemDropRule.Common(ItemType<ShadowEnergy>(), 3, 1, 3));

            if (npc.type == NPCID.DemonEye || npc.type == NPCID.DemonEye2)
                npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 50));
            if (npc.type == NPCID.WanderingEye)
                npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 25));

            if (Main.slimeRainNPC[npc.type])
            {
                npcLoot.Add(ItemDropRule.Common(ItemType<SlimeSapling>(), 50));
            }
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            switch (type)
            {
                case NPCID.ArmsDealer:
                    {
                        if (NPC.downedPlantBoss)    //花后售卖远古核心
                        {
                            shop.item[nextSlot].SetDefaults(ModContent.ItemType<AncientCore>());
                            nextSlot++;
                        }
                        break;
                    }
                default: break;
            }
        }
    }
}
