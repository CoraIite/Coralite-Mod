using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Content.Items.Placeable;
using Coralite.Content.Items.Shadow;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Coralite.Content.Items.Weapons_Shoot;
using Coralite.Content.Items.YujianHulu;

namespace Coralite.Content.NPCs.VanillaNPC
{
    public class CoraliteGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            switch (npc.type)
            {
                default: break;
                case NPCID.DemonEye:
                case NPCID.DemonEye2:
                    npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 50));
                    break;
                case NPCID.WanderingEye:
                    npcLoot.Add(ItemDropRule.Common(ItemType<EyeballSeed>(), 25));
                    break;

                case NPCID.DarkCaster:
                    npcLoot.Add(ItemDropRule.Common(ItemType<ShadowEnergy>(), 3, 1, 3));
                    break;

                case NPCID.EaterofSouls:
                case NPCID.CorruptGoldfish:
                case NPCID.DevourerHead:
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientDemoniteYujian>(), 100));
                    break;

                case NPCID.Crimera:
                case NPCID.FaceMonster:
                case NPCID.BloodCrawler:
                case NPCID.CrimsonGoldfish:
                    npcLoot.Add(ItemDropRule.Common(ItemType<AncientCrimtaneYujian>(), 100));
                    break;
                case NPCID.DukeFishron:
                    npcLoot.Add(ItemDropRule.ByCondition(new DownedGolemCondition(), ItemType<DukeFishronSkin>(), 1, 3, 5));
                    break;
            }

            if (Main.slimeRainNPC[npc.type])
                npcLoot.Add(ItemDropRule.Common(ItemType<SlimeSapling>(), 50));
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
