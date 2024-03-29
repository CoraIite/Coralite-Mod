using Coralite.Content.Biomes;
using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.Items.Magike;
using Terraria;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        #region 钓鱼系统

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;

            if (inWater && Player.ZoneBeach && !attempt.crate)
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
                            itemDrop = ItemType<BubblePearlNecklace>();
                        if (Main.rand.NextBool(6))
                            itemDrop = ItemType<Items.FlyingShields.HorseshoeCrab>();
                    }
                    if (Main.rand.NextBool(6))
                        itemDrop = ItemType<Items.FlyingShields.PearlRay>();
                }
                else if (attempt.legendary)
                {
                    if (NPC.downedFishron && Main.rand.NextBool(5))
                        itemDrop = ItemType<OceanheartNecklace>();
                }

            }

            if (attempt.crate)
            {
                if (inWater)
                {
                    if (Player.InModBiome<MagicCrystalCave>())
                    {
                        // 如果钓到了的是匣子，就替换为自己的匣子

                        // 为了不替换掉最高级的匣子，所以做点限制
                        // Their drop conditions are "veryrare" or "legendary"
                        // (After that come biome crates ("rare"), then iron/mythril ("uncommon"), then wood/pearl (none of the previous))
                        // Let's replace biome crates 50% of the time (player could be in multiple (modded) biomes, we should respect that)
                        //增加50%的概率替换掉其他匣子，因为玩家有时候可能在多个重叠的环境中
                        if (!attempt.veryrare && !attempt.legendary && attempt.rare && Main.rand.NextBool())
                        {
                            itemDrop = ItemType<CrystalCrate>();
                            return; // This is important so your code after this that rolls items will not run
                        }

                    }
                }
            }

        }

        #endregion

    }
}
