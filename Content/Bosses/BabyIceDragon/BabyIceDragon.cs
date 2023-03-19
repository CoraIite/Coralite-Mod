﻿using Coralite.Content.Items.IcicleItems;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
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

        public bool ExchangeState
        {
            get => NPC.ai[0] == 0f;
            set
            {
                if (value)
                    NPC.ai[0] = 0f;
                else
                    NPC.ai[0] = 1f;
            }
        }

        internal ref float State => ref NPC.ai[1];
        internal ref float Timer => ref NPC.ai[2];

        /// <summary>
        /// 普通攻击的计数，大于某一值之后归零并开始会产生破绽的动作
        /// </summary>
        internal ref float NormalMoveCount => ref NPC.ai[3];

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
            //动画
            onKillAnim = -5,
            onSpawnAnim = -4,
            roaringAnim=-3,

            //非攻击动作，也非动画
            dizzy = -2,
            rest = -1,
            //有可能会有破绽的攻击动作
            dive = 0,
            accumulate = 1,
            //普通攻击动作
            iceBreath = 2,
            horizontalDash = 3,
            //二阶段追加攻击动作
            smashDown = 4,
            iceThornsTrap = 5,
            //以下是大师模式专属，特殊攻击动作
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
                case (int)AIStates.roaringAnim:         //吼叫

                    break;
                case (int)AIStates.dizzy:      //原地眩晕


                    break;
                case (int)AIStates.rest:        //休息，原地悬停一会

                    break;
                default:
                case (int)AIStates.dive:      //俯冲攻击，先飞上去（如果飞不上去就取消攻击），在俯冲向玩家，期间如果撞墙则原地眩晕
                    Dive();

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

        public void ResetStates()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int phase;

            //大师模式更早进入2阶段
            if (Main.masterMode)
                phase = NPC.life < (int)(NPC.lifeMax * 0.75f) ? 2 : 1;
            else
                phase = NPC.life < (NPC.lifeMax / 2) ? 2 : 1;

            do
            {
                if (ExchangeState && phase == 2)    //进入2阶段时固定进入吼叫动画
                {
                    State = (int)AIStates.roaringAnim;
                    ExchangeState = false;
                    break;
                }

                if (CanVulnerableMove())
                {
                    VulnerableMove();
                    break;
                }

                if (CanSpecialMove(phase))
                {
                    SpecialMove();
                    break;
                }

                NormalMove(phase);

            } while (false);

            Timer = 0;
            NPC.TargetClosest();
            NPC.netUpdate = true;
        }

        public void Dizzy(int dizzyTime)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            State = (int)AIStates.dizzy;

            Timer = dizzyTime;
            NormalMoveCount = 0;        //归零这个计数（虽说感觉可能没什么用的样子）
            NPC.TargetClosest();
            NPC.netUpdate = true;
        }

        public void HaveARest(int restTime)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            State = (int)AIStates.rest;

            Timer = restTime;
            NPC.TargetClosest();
            NPC.netUpdate = true;
        }

        #region 特殊行动

        public bool CanSpecialMove(int phase)
        {
            if (phase !=2)
                return false;

            return Main.masterMode && Main.rand.NextBool(3);
        }

        /// <summary>
        /// 大师模式专属特殊行动
        /// </summary>
        public void SpecialMove()
        {
            State = Main.rand.Next(2) switch
            {
                0 => (int)AIStates.iceTornado,
                1 => (int)AIStates.iciclesFall,
                _ => (int)AIStates.iceTornado
            };
        }

        #endregion

        #region 普通行动

        /// <summary>
        /// 普通的动作
        /// </summary>
        public void NormalMove(int phase)
        {
            //这个switch表达式或许让它的可读性变得更加差了......
            State = phase switch
            {
                //2阶段
                2 => Main.rand.Next(4) switch
                {
                    0 => (int)AIStates.dive,
                    1 => (int)AIStates.accumulate,
                    2 => (int)AIStates.smashDown,
                    3 => (int)AIStates.iceThornsTrap,
                    _ => (int)AIStates.dive
                },
                //一阶段及其他
                _ => Main.rand.Next(2) switch
                {
                    0 => (int)AIStates.dive,
                    1 => (int)AIStates.accumulate,
                    _ => (int)AIStates.dive
                },
            };

            NormalMoveCount += 1f;
        }

        #endregion

        #region 有破绽的行动

        /// <summary>
        /// 有破绽的动作
        /// </summary>
        public void VulnerableMove()
        {
            State = Main.rand.Next(2) switch
            {
                0 => (int)AIStates.dive,
                1 => (int)AIStates.accumulate,
                _ => (int)AIStates.dive
            };
        }

        public bool CanVulnerableMove()
        {
            //大师模式行动每4次进行一次有破绽动作
            if (Main.masterMode && NormalMoveCount >= 4)
            {
                NormalMoveCount = 0;
                return true;
            }

            //其他模式每2次动作进行一次
            if (NormalMoveCount >= 2)
            {
                NormalMoveCount = 0;
                return true;
            }

            return false;
        }

        #endregion

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            int frameWidth = mainTex.Width;
            int frameHeight = mainTex.Height / Main.npcFrameCount[NPC.type];
            Rectangle frameBox = new Rectangle(0, NPC.frame.Y * frameHeight, frameWidth, frameHeight);

            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);

            if (NPC.spriteDirection != 1)
                effects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }

        #endregion

    }
}