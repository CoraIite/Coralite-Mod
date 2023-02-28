using Coralite.Content.Items.IcicleItems;
using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    //  小冰龙宝宝
    //  小小的也很可爱
    //  就好这口！
    //
    //
    //
    //
    //

    [AutoloadBossHead]
    public class BabyIceDragon : ModNPC
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;

        internal ref float State => ref NPC.ai[1];
        internal ref float Timer => ref NPC.ai[2];

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("冰龙宝宝");

            Main.npcFrameCount[Type] = 5;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 85;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.lifeMax = 2500;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.lifeMax = (int)(1700 * bossLifeScale) + numPlayers * 350;
            //NPC.damage = 30;
            //NPC.defense = 10;

            //if (Main.masterMode)
            //{
            //    NPC.lifeMax = (int)(2200 * bossLifeScale) + numPlayers * 550;
            //    NPC.damage = 45;
            //    NPC.defense = 12;
            //}
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<RediancieRelic>()));
            //npcLoot.Add(ItemDropRule.BossBag(ItemType<RediancieBossBag>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleCrystal>(), 1, 3, 5));
            npcLoot.Add(notExpertRule);
        }

        public override void Load()
        {
            //for (int i = 0; i < 5; i++)
            //    GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.BossGores + "Rediancie_Gore" + i);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);
        }

        #endregion

        #region AI

        public enum AIStates : int
        {
            onKillAnmi = -3,
            onSpawnAnmi = -2,
            dizzy = -1,
            //以上是仅动画，以下是攻击动作
            dive = 0,
            Accumulate = 1,
            iceBreath = 2,
            horizontalDash = 3,
            smashDown = 4,
            iceThornsTrap = 5,
            //以下是大师模式专属
            iceTornado = 6,
            iciclesFall = 7
        }

        public override void AI()
        {
            switch (State)
            {
                case (int)AIStates.onKillAnmi:      //死亡时的动画
                    break;
                case (int)AIStates.onSpawnAnmi:      //生成时的动画
                    break;
                case (int)AIStates.dizzy:      //原地眩晕
                    break;
                default:
                case (int)AIStates.dive:      //俯冲攻击，先飞上去（如果飞不上去就取消攻击），在俯冲向玩家，期间如果撞墙则原地眩晕
                    break;
                case (int)AIStates.Accumulate:      //蓄力弹幕，如果蓄力途中收到一定伤害会失衡并使冰块砸到自己，眩晕一定时间
                    break;
                case (int)AIStates.iceBreath:      //冰吐息
                    break;
                case (int)AIStates.horizontalDash:      //龙车，或者就叫做水平方向冲刺，专家模式以上会在冲刺期间生成冰锥
                    break;
                case (int)AIStates.smashDown:      //飞得高点然后下砸，并在周围生成冰刺弹幕
                    break;
                case (int)AIStates.iceTornado:      //简单准备后冲向玩家并在轨迹上留下冰龙卷风一样的东西
                    break;
                case (int)AIStates.iciclesFall:      //冰雹弹幕攻击，先由下至上吐出一群冰锥，再在玩家头顶随机位置落下冰锥
                    break;
            }
        }

        #endregion
    }
}
