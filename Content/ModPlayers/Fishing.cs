using Coralite.Content.Biomes;
using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.Items.Magike;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Items.Misc;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;

            if (inWater)
            {
                //血月使用血蛭钓鱼必出血月敌怪
                if (Main.bloodMoon && attempt.playerFishingConditions.BaitItemType == ItemType<BloodWorm>())
                {
                    itemDrop = -1;
                    if (Main.hardMode)
                    {
                        WeightedRandom<int> wr = new WeightedRandom<int>();

                        wr.Add(NPCID.ZombieMerman, 1);//僵尸人鱼
                        wr.Add(NPCID.EyeballFlyingFish, 1);//眼球鱼

                        wr.Add(NPCID.GoblinShark, 2);//哥布林鲨鱼
                        wr.Add(NPCID.BloodEelHead, 2);//血鳗鱼
                        wr.Add(NPCID.BloodNautilus, 1);//恐惧鹦鹉螺

                        npcSpawn = wr.Get();
                    }
                    else
                    {
                        npcSpawn = Main.rand.NextFromList(NPCID.ZombieMerman, NPCID.EyeballFlyingFish);
                    }

                    return;
                }

                if (  Player.ZoneBeach && !attempt.crate)
                {
                    if (attempt.common)
                    {
                        //if (Main.rand.NextBool(15))
                        //    itemDrop = ItemType<NacliteSeedling>();
                    }
                    else if (attempt.uncommon)
                    {
                        if (Main.hardMode)
                        {
                            if (Main.rand.NextBool(10))
                                itemDrop = ItemType<Items.FlyingShields.HorseshoeCrab>();
                        }

                        if (Main.rand.NextBool(10))
                            itemDrop = ItemType<Items.FlyingShields.PearlRay>();
                    }
                    else if (attempt.rare)
                    {
                        if (Main.rand.NextBool(4))
                            itemDrop = Main.rand.NextFromList(
                                ItemType<BubblePearlNecklace>(),
                                ItemType<CoralRing>()
                                );
                    }
                    else if (attempt.legendary)
                    {
                        if (NPC.downedFishron && Main.rand.NextBool(5))
                            itemDrop = ItemType<OceanheartNecklace>();
                    }

                }

                if (Player.InModBiome<MagicCrystalCave>())
                {
                    if (attempt.crate)
                    {
                        // 如果钓到了的是匣子，就替换为自己的匣子

                        // 为了不替换掉最高级的匣子，所以做点限制
                        // Their drop conditions are "veryrare" or "legendary"
                        // (After that come biome crates ("rare"), then iron/mythril ("uncommon"), then wood/pearl (none of the previous))
                        // Let's replace biome crates 50% of the time (player could be in multiple (modded) biomes, we should respect that)
                        //增加50%的概率替换掉其他匣子，因为玩家有时候可能在多个重叠的环境中
                        if (!attempt.rare && !attempt.veryrare && !attempt.legendary && attempt.uncommon && Main.rand.NextBool())
                        {
                            itemDrop = ItemType<CrystalCrate>();
                            return; // This is important so your code after this that rolls items will not run
                        }
                    }
                    else
                    {
                        if (attempt.common && Main.rand.NextBool(5))//普通的晶鳍鱼
                        {
                            itemDrop = ItemType<CrystalFins>();
                            return;
                        }
                    }
                }

                if (Player.InModBiome<CrystallineSkyIsland>())
                    CrystallineSkyIslandFishing_Water(attempt, ref itemDrop);
            }
        }

        /// <summary>
        /// 蕴魔空岛水中钓鱼
        /// </summary>
        /// <param name="attempt"></param>
        /// <param name="itemDrop"></param>
        private static void CrystallineSkyIslandFishing_Water(FishingAttempt attempt, ref int itemDrop)
        {
            if (attempt.crate)
            {
                if (!attempt.rare && !attempt.veryrare && !attempt.legendary && attempt.uncommon && Main.rand.NextBool())
                {
                    itemDrop = ItemType<CrystallineCrate>();//蕴魔水晶匣子
                    return; 
                }
            }
            else
            {
                if (attempt.common && Main.rand.NextBool(5))//普通的玻璃猫
                {
                    itemDrop = ItemType<GlassCat>();
                    return;
                }

                if (attempt.uncommon && attempt.questFish == ItemType<RainbowPorpoise>())//任务鱼：虹豚
                {
                    itemDrop = ItemType<RainbowPorpoise>();
                    return;
                }
            }
        }
    }
}
