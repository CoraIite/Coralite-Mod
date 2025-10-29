using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.Plush;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.GlobalNPCs
{
    public partial class CoraliteGlobalNPC : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            switch (shop.NpcType)
            {
                case NPCID.Painter:
                    {
                        shop.Add<StartSkyIsland>(CoraliteConditions.InCrystallineSkyIsland);
                    }
                    break;
                case NPCID.BestiaryGirl:
                    {
                        shop.Add<BlackPlush>();
                    }
                    break;
                //case NPCID.ArmsDealer:
                //    {
                //        shop.Add<AncientCore>(Condition.DownedPlantera);//远古核心
                //        break;
                //    }
                //case NPCID.TravellingMerchant://游商
                //    {
                //        shop.Add<TravelJournaling>();//手记
                //        shop.Add(ItemID.GlowTulip, Condition.Hardmode);//发光郁金香
                //        shop.Add<MineShield>(Condition.Hardmode);//我的盾牌
                //        shop.Add<RuneParchment>(Condition.DownedPlantera);//花后获取符文羊皮纸
                //    }
                //    break;
                case NPCID.WitchDoctor:
                    {
                        shop.Add<CrystallineFountainItem>(CoraliteConditions.InCrystallineSkyIsland);
                    }
                    break;
                default: break;
            }
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (npc.type == NPCID.TravellingMerchant)//游商
            {
                int i = 0;
                for (; i < items.Length - 1; i++)
                {
                    if (items[i] == null || items[i].IsAir)
                        break;
                }

                items[i] = new Item(ItemType<TravelJournaling>());
                i++;
                if (i >= items.Length)
                    return;

                if (Main.hardMode)
                {
                    items[i] = new Item(ItemID.GlowTulip);
                    i++;
                    if (i >= items.Length)
                        return;

                    items[i] = new Item(ItemType<MineShield>());
                    i++;
                    if (i >= items.Length)
                        return;
                }

                if (NPC.downedPlantBoss)//花后获取符文羊皮纸，象形文字羊皮纸
                {
                    items[i] = new Item(ItemType<RuneParchment>());
                    i++;
                    if (i >= items.Length)
                        return;

                    items[i] = new Item(ItemType<GlyphParchment>());
                    i++;
                    if (i >= items.Length)
                        return;
                }

                if (NPC.downedMoonlord)//月后卖海盾
                {
                    items[i] = new Item(ItemType<HylianShield>());
                    i++;
                    if (i >= items.Length)
                        return;

                }
            }
        }

    }
}
