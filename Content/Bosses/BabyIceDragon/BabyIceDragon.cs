using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Items.IcicleItems;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
    public partial class BabyIceDragon : ModNPC
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;

        Player Target => Main.player[NPC.target];

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

        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest(false);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                State = (int)AIStates.onSpawnAnim;
                NPC.noTileCollide = false;
                NPC.netUpdate = true;
            }
        }

        public enum AIStates : int
        {
            onKillAnim = -3,
            onSpawnAnim = -2,
            dizzy = -1,
            //以上是仅动画，以下是攻击动作
            dive = 0,
            accumulate = 1,
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
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)
                NPC.TargetClosest();

            if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)//没有玩家存活时离开
            {
                State = -1;
                NPC.velocity.X *= 1.02f;
                NPC.velocity.Y -= 0.04f;
                NPC.EncourageDespawn(10);
                return;
            }

            switch (State)
            {
                case (int)AIStates.onKillAnim:      //死亡时的动画
                    break;
                case (int)AIStates.onSpawnAnim:      //生成时的动画
                    {
                        if (Timer == 0)
                        {
                            //生成动画弹幕
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<BabyIceDragon_OnSpawnAnim>(), 0, 0);
                            NPC.dontTakeDamage = true;
                        }


                    }

                    break;
                case (int)AIStates.dizzy:      //原地眩晕


                    break;
                default:
                case (int)AIStates.dive:      //俯冲攻击，先飞上去（如果飞不上去就取消攻击），在俯冲向玩家，期间如果撞墙则原地眩晕
                    break;
                case (int)AIStates.accumulate:      //蓄力弹幕，如果蓄力途中收到一定伤害会失衡并使冰块砸到自己，眩晕一定时间
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
