using System;
using System.IO;
using Coralite.Content.Items.IcicleItems;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    /*小冰龙宝宝
    * 小小的也很可爱
    *
    */

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

        internal bool canDrawDizzyStars;

        private int movePhase;

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
            NPC.width = 64;
            NPC.height = 40;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.lifeMax = 2500;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 6, 0, 0);

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
            NPC.frame.Y = 2;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                State = (int)AIStates.onSpawnAnim;
                NPC.ai[0] = 0f;
                Timer = 0;
                NormalMoveCount = 0;
                NPC.noTileCollide = false;
                NPC.netUpdate = true;
            }
        }

        public enum AIStates : int
        {
            //动画
            onKillAnim = -5,
            onSpawnAnim = -4,
            roaringAnim = -3,

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
                        do
                        {
                            if (Timer == 0)
                            {
                                //生成动画弹幕
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<BabyIceDragon_OnSpawnAnim>(), 0, 0);
                                NPC.dontTakeDamage = true;
                                NPC.velocity.Y = -0.2f;
                                break;
                            }

                            if (Timer == 100)
                            {
                                NPC.frame.Y = 0;
                                NPC.velocity *= 0;
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                            }

                            if (Timer < 110)
                            {
                                ChangeFrameNormally();
                                break;
                            }

                            if (Timer == 110)
                            {
                                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                                PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(0.3f, 0.3f), 5f, 6f, 60, 1000f, "BabyIceDragon");
                                Main.instance.CameraModifiers.Add(modifier);
                            }

                            if (Timer > 110 && Timer < 170)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                if (Timer % 10 == 0)
                                    Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                                if (Timer % 20 == 0)
                                    Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);

                                break;
                            }

                            FlyUp();

                            if (Timer >= 260)
                            {
                                NPC.dontTakeDamage = false;
                                ResetStates();
                                break;
                            }
                        } while (false);

                        Timer++;
                    }
                    break;
                case (int)AIStates.roaringAnim:         //吼叫

                    break;
                case (int)AIStates.dizzy:      //原地眩晕
                    if (Timer < 1)
                        ResetStates();

                    Timer--;

                    break;
                case (int)AIStates.rest:        //休息，原地悬停一会
                    {
                        NPC.velocity *= 0.98f;

                        if (Timer < 0)
                        {
                            ResetStates();
                            break;
                        }

                        Timer--;
                        ChangeFrameNormally();
                    }
                    break;
                default:
                case (int)AIStates.dive:      //俯冲攻击，先飞上去（如果飞不上去就取消攻击），在俯冲向玩家，期间如果撞墙则原地眩晕
                    Dive();
                    break;
                case (int)AIStates.accumulate:      //蓄力弹幕，如果蓄力途中收到一定伤害会失衡并使冰块砸到自己，眩晕一定时间
                    {
                        ChangeFrameNormally();
                        do
                        {
                            if (Timer < 60)
                            {
                                SetDirection();
                                NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                                float yLength = Math.Abs(Target.Center.Y - 40 - NPC.Center.Y);
                                if (yLength > 50)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.14f, 0.1f, 0.96f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3f, 0.08f, 0.08f, 0.96f);
                                break;
                            }

                            if (Timer < 80)
                            {
                                NPC.velocity *= 0.92f;
                                break;
                            }

                            if (Timer == 80)
                            {
                                //生成冰块弹幕
                                NPC.velocity *= 0;
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + NPC.direction * 64, (int)NPC.Center.Y, NPCType<IceCube>());
                            }

                            if (Timer % 20 == 0)
                            {
                                //控制冰块弹幕，让它变大
                                int index = -1;
                                int iceCubeType = NPCType<IceCube>();
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC npc = Main.npc[i];
                                    if (npc.active && npc.type == iceCubeType)
                                    {
                                        index = i;
                                        break;
                                    }
                                }

                                if (index == -1)
                                    ResetStates();

                                Main.npc[index].ai[0] = 1;
                            }

                            if (Timer > 1000)
                                ResetStates();
                        } while (false);
                        Timer++;
                    }
                    break;
                case (int)AIStates.iceBreath:      //冰吐息
                    {
                        do
                        {
                            if (Timer < 100)
                            {
                                SetDirection();
                                NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                                float yLength = Math.Abs(Target.Center.Y - 40 - NPC.Center.Y);
                                if (yLength > 50)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.14f, 0.1f, 0.96f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3f, 0.08f, 0.08f, 0.96f);
                                ChangeFrameNormally();
                                break;
                            }

                            if (Timer < 120)
                            {
                                NPC.velocity *= 0.99f;
                                SetDirection();
                                break;
                            }

                            if (Timer < 200)
                            {
                                //生成冰吐息弹幕
                                if (Timer % 10 == 0)
                                {
                                    GetMouseCenter(out Vector2 targetDir, out Vector2 mouseCenter);
                                    for (int i = -1; i < 2; i++)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), mouseCenter, targetDir.RotatedBy(i * 0.3f) * 12f, ProjectileType<IceBreath>(), 30, 5f);
                                }
                                break;
                            }

                            int restTime = Main.masterMode ? 80 : 140;
                            HaveARest(restTime);
                            return;

                        } while (false);

                        Timer++;
                    }
                    break;
                case (int)AIStates.horizontalDash:      //龙车，或者就叫做水平方向冲刺，专家模式以上会在冲刺期间生成冰锥
                    {
                        do
                        {
                            if (Timer < 100)
                            {
                                SetDirection();
                                NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                                float yLength = Math.Abs(Target.Center.Y - 40 - NPC.Center.Y);
                                if (yLength > 50)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.14f, 0.1f, 0.96f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3f, 0.08f, 0.08f, 0.96f);

                                ChangeFrameNormally();
                                break;
                            }

                            //冲向玩家方向
                            if (Timer == 100)
                            {
                                NPC.velocity.Y = 0f;
                                NPC.velocity.X = NPC.direction * 15f;
                                float factor = (NPC.Center.Y - Target.Center.Y) / 100;
                                factor = Math.Clamp(factor, -1, 1);
                                NPC.velocity.RotatedBy(factor * 0.6f);
                            }

                            if (Timer > 180)
                            {
                                NPC.velocity *= 0.99f;
                                break;
                            }

                            if (Timer == 240)
                                ResetStates();

                        } while (false);

                        Timer++;
                    }
                    break;
                case (int)AIStates.smashDown:      //飞得高点然后下砸，并在周围生成冰刺弹幕
                    {
                        do
                        {
                            if (Timer < 100)
                            {
                                FlyUp();
                                break;
                            }

                            if (Timer == 100)
                            {
                                NPC.velocity.X *= 0;
                                NPC.velocity.Y = -3;
                                NPC.noGravity = false;
                                NPC.noTileCollide = true;
                                SetDirection();
                            }
                            //TODO:控制旋转


                            //向玩家加速
                            if (Timer < 140)
                            {
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3, 0.08f, 0.08f, 0.96f);
                                break;
                            }

                            //TODO:生成下落特效的粒子
                            //下落
                            NPC.velocity.X *= 0.96f;
                            NPC.velocity.Y += 0.02f;
                            if (NPC.velocity.Y > 16)
                                NPC.velocity.Y = 16;

                            if (Timer < 240)
                                break;

                            if (Timer == 240)
                                NPC.noTileCollide = false;

                            if (Timer > 240)
                            {
                                //检测下方物块
                                Point position = NPC.BottomLeft.ToPoint();
                                position.Y += 1;
                                for (int i = 0; i < 3; i++)
                                {
                                    if (WorldGen.ActiveAndWalkableTile(position.X, position.Y))    //砸地，生成冰刺弹幕
                                    {
                                        SpawnIceThorns();
                                        int restTime = Main.masterMode ? 80 : 140;
                                        HaveARest(restTime);
                                    }
                                    position.X += 1;
                                }
                            }

                            if (Timer > 480)
                            {
                                int restTime2 = Main.masterMode ? 80 : 140;
                                HaveARest(restTime2);
                            }
                        } while (false);

                        Timer++;
                    }
                    break;
                case (int)AIStates.iceTornado:      //简单准备后冲向玩家并在轨迹上留下冰龙卷风一样的东西
                    IceTornado();
                    break;
                case (int)AIStates.iciclesFall:      //冰雹弹幕攻击，先由下至上吐出一群冰锥，再在玩家头顶随机位置落下冰锥
                                                     //暂时先不写
                    ResetStates();
                    break;
            }
        }

        /// <summary>
        /// 飞上去，如果头顶被方块阻挡则无法飞上去
        /// </summary>
        public void FlyUp()
        {
            //根据帧图来改变速度，大概效果是扇一下翅膀向上飞一小段
            NPC.velocity.X *= 0.99f;

            //只有扇翅膀的时候才会有向上加速度，否则减速
            switch (NPC.frame.Y)
            {
                default:
                case 0:
                case 3:
                case 4:
                    NPC.velocity.Y *= 0.99f;
                    break;
                case 1:
                case 2:
                    NPC.velocity.Y -= 0.15f;
                    break;
            }

            if (NPC.velocity.Y < -10)
                NPC.velocity.Y = -10;

            ChangeFrameNormally();

            if (State == (int)AIStates.onSpawnAnim)
                return;

            if (NPC.velocity.Y < -10)
                NPC.velocity.Y = -10;

            Point position = NPC.position.ToPoint();
            position.X -= 1;
            position.Y -= 1;
            //如果头顶有方块阻挡直接结束此环节
            for (int i = -1; i < 5; i++)
            {
                //检测NPC头顶的一排物块
                Tile tile = Framing.GetTileSafely(position);
                if (tile.HasSolidTile())
                    ResetStates();

                position.X += 1;
            }

        }

        public void GetMouseCenter(out Vector2 targetDir,out Vector2 mouseCenter)
        {
            targetDir = (NPC.rotation+ (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2();
            mouseCenter = NPC.Center + targetDir * 20 ;
        }

        public void SpawnIceThorns()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Point sourceTileCoords = NPC.Bottom.ToTileCoordinates();
            sourceTileCoords.X += 3;

            PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(0f, 1f), 20f, 6f, 30, 1000f, "BabyIceDragon");
            Main.instance.CameraModifiers.Add(modifier);

            for (int i = 0; i < 4; i++)
                TryMakingSpike(ref sourceTileCoords, 1, 20, i, 1);
            sourceTileCoords.X -= 6;
            for (int i = 0; i < 4; i++)
                TryMakingSpike(ref sourceTileCoords, -1, 20, i, 1);
        }

        private void TryMakingSpike(ref Point sourceTileCoords, int dir, int howMany, int whichOne, int xOffset)
        {
            int num = 13;
            int position_X = sourceTileCoords.X + xOffset * dir;
            int position_Y = TryMakingSpike_FindBestY(ref sourceTileCoords, position_X);
            if (WorldGen.ActiveAndWalkableTile(position_X, position_Y))
            {
                Vector2 position = new Vector2(position_X * 16 + 8, position_Y * 16 - 8);
                Vector2 velocity = new Vector2(0f, -1f).RotatedBy(whichOne * dir * 0.7f * ((float)Math.PI / 4f / howMany));
                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ProjectileID.DeerclopsIceSpike, num, 0f, Main.myPlayer, 0f, 0.1f + Main.rand.NextFloat() * 0.1f + xOffset * 1.1f / howMany);
            }
        }

        private int TryMakingSpike_FindBestY(ref Point sourceTileCoords, int x)
        {
            int position_Y = sourceTileCoords.Y;
            NPCAimedTarget targetData = NPC.GetTargetData();
            if (!targetData.Invalid)
            {
                Rectangle hitbox = targetData.Hitbox;
                Vector2 vector = new Vector2(hitbox.Center.X, hitbox.Bottom);
                int num2 = (int)(vector.Y / 16f);
                int num3 = Math.Sign(num2 - position_Y);
                int num4 = num2 + num3 * 15;
                int? num5 = null;
                float num6 = float.PositiveInfinity;
                for (int i = position_Y; i != num4; i += num3)
                {
                    if (WorldGen.ActiveAndWalkableTile(x, i))
                    {
                        float num7 = new Point(x, i).ToWorldCoordinates().Distance(vector);
                        if (!num5.HasValue || !(num7 >= num6))
                        {
                            num5 = i;
                            num6 = num7;
                        }
                    }
                }

                if (num5.HasValue)
                    position_Y = num5.Value;
            }

            for (int j = 0; j < 20; j++)
            {
                if (position_Y < 10)
                    break;

                if (!WorldGen.SolidTile(x, position_Y))
                    break;

                position_Y--;
            }

            for (int k = 0; k < 20; k++)
            {
                if (position_Y > Main.maxTilesY - 10)
                    break;

                if (WorldGen.ActiveAndWalkableTile(x, position_Y))
                    break;

                position_Y++;
            }

            return position_Y;
        }

        public void ResetStates()
        {
            canDrawDizzyStars = false;
            movePhase = 0;
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

                //有破绽的行动
                if (CanVulnerableMove())
                {
                    VulnerableMove();
                    break;
                }

                //特殊行动，大师模式专属
                if (CanSpecialMove(phase))
                {
                    SpecialMove();
                    break;
                }

                //普通行动
                NormalMove(phase);

            } while (false);

            Timer = 0;
            NPC.TargetClosest();
            NPC.netUpdate = true;
        }

        public void Dizzy(int dizzyTime)
        {
            canDrawDizzyStars = true;

            if (NPC.target != Main.myPlayer)
                return;

            State = (int)AIStates.dizzy;

            NPC.frameCounter = 0;
            NPC.frame.Y = 2;

            Timer = dizzyTime;
            NormalMoveCount = 0;        //归零这个计数（虽说感觉可能没什么用的样子）
            NPC.noGravity = false;
            NPC.noTileCollide = false;
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
            SetDirection();
            NPC.netUpdate = true;
        }

        public void SetDirection()
        {
            if (Math.Abs(NPC.Center.X - Target.Center.X) < 16)
                return;

            NPC.spriteDirection = NPC.direction = NPC.Center.X > Target.Center.X ? 1 : -1;
        }

        #region 特殊行动

        public bool CanSpecialMove(int phase)
        {
            if (phase != 2)
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

        #region Frames

        /// <summary>
        /// 简易的控制帧图
        /// </summary>
        public void ChangeFrameNormally()
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += 1;
                if (NPC.frame.Y > 4)
                    NPC.frame.Y = 0;
            }
        }

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

        #region NewWork
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(movePhase);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            movePhase = reader.ReadInt32();
        }

        #endregion
    }
}
