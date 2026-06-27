using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Network;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
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

        public static Asset<Texture2D> GlowTex;
        private Player Target => Main.player[NPC.target];

        /// <summary>是否仍待进入二阶段（一次性）。迁移后仅服务端决策使用，无需占用同步 ai 槽。</summary>
        internal bool ExchangeState = true;

        /// <summary>顶层 FSM 状态 ID，占用 <c>ai[0]</c> 并由基座 <see cref="CoraliteBossStateMachine{TContext}"/> 自动同步。</summary>
        internal ref float State => ref NPC.ai[0];
        internal ref float Timer => ref NPC.ai[2];

        /// <summary>
        /// 普通攻击的计数，大于某一值之后归零并开始会产生破绽的动作。<br/>
        /// 迁移后仅服务端在 <see cref="ResetStates"/> 中决策使用（结果以状态 ID 经 ai[0] 同步），无需占用同步 ai 槽。
        /// </summary>
        internal float NormalMoveCount;

        internal BabyIceDragonContext AiContext;
        internal CoraliteBossStateMachine<BabyIceDragonContext> StateMachine;

        /// <summary>当前顶层状态 ID；状态机未建立时回退到出生动画。</summary>
        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)AIStates.onSpawnAnim;

        internal ref float GlowAlpha => ref NPC.localAI[0];
        internal ref float DoubleDashAngle => ref NPC.localAI[1];
        internal ref float DoubleDashLength => ref NPC.localAI[2];
        internal ref float DropScaleCount => ref NPC.localAI[3];

        /// <summary>
        /// 脱战计时器
        /// </summary>
        public int FlyAwayTimer;

        public int movePhase;
        public bool canDrawShadows;
        internal List<int> Moves;
        private bool spwan;

        public const int FlyingFrame = 4;
        public const int DizyFrame = 4;

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 106;
            NPC.height = 68;
            NPC.damage = 40;
            NPC.defense = 6;
            NPC.lifeMax = 4500;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 6, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.BossBar = GetInstance<BabyIceDragonBossBar>();
            GetInstance<BabyIceDragonBossBar>().Reset(NPC);

            //BGM：冰结寒流
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/IcyColdStream");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((3820 + (numPlayers * 1750)) / journeyScale);
                    NPC.damage = 55;
                    NPC.defense = 15;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((4720 + (numPlayers * 2100)) / journeyScale);
                    NPC.damage = 60;
                    NPC.defense = 18;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 70;
                    NPC.defense = 20;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 0.4f;
                }

                return;
            }

            NPC.lifeMax = 3820 + (numPlayers * 1750);
            NPC.damage = 55;
            NPC.defense = 15;

            if (Main.masterMode)
            {
                NPC.lifeMax = 4720 + (numPlayers * 2100);
                NPC.damage = 60;
                NPC.defense = 18;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 5320 + (numPlayers * 2200);
                NPC.damage = 70;
                NPC.defense = 20;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 0.4f;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<IcicleSoulStone>(), 4));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<BabyIceDragonRelic>()));
            npcLoot.Add(ItemDropRule.BossBag(ItemType<BabyIceDragonBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<BabyIceDragonTrophy>(), 10));
            npcLoot.Add(ItemDropRule.Common(ItemType<BabyIceDragonMask>(), 7));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleCrystal>(), 1, 3, 5));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleScale>(), 1, 2, 4));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleBreath>(), 1, 4, 7));
            npcLoot.Add(notExpertRule);
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GlowTex = Request<Texture2D>(AssetDirectory.BabyIceDragon + Name + "_Glow");
            //for (int i = 0; i < 5; i++)
            //    GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.BossGores + "Rediancie_Gore" + i);
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            GlowTex = null;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(CoraliteSoundID.DigIce, NPC.Center);
            if (NPC.life < NPC.lifeMax / 4 && hit.Crit && DropScaleCount < 8 && !VaultUtils.isClient)
            {
                DropScaleCount++;
                Item.NewItem(NPC.GetSource_DropAsItem(), NPC.getRect(), ItemType<IcicleScale>());
            }
        }

        public override void OnKill()
        {
            DownedBossSystem.DownBabyIceDragon();
            if (!VaultUtils.isClient)
            {
                IceEggSpawner.BabyIceDragonSlain();
                Main.StopRain();
                Main.SyncRain();
            }
        }

        public override bool? CanFallThroughPlatforms()
        {
            return State != (int)AIStates.smashDown && State != (int)AIStates.dizzy;
        }

        public override bool CheckDead()
        {
            if (StateMachine == null)
                return true;

            if (VaultUtils.isClient)
                return CurrentStateId == (int)AIStates.onKillAnim;

            if (CurrentStateId != (int)AIStates.onKillAnim)
            {
                Timer = 0;
                NPC.dontTakeDamage = true;
                NPC.life = 1;
                StateMachine.ChangeState((int)AIStates.onKillAnim);
                return false;
            }

            return true;
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            int width = (int)(86 * NPC.scale);
            int height = (int)(58 * NPC.scale);
            npcHitbox = new Rectangle((int)(NPC.Center.X - (width / 2)), (int)(NPC.Center.Y - (height / 2)), width, height);
            return true;
        }

        #endregion

        #region AI

        // 迁移说明：状态 ID 现走 ai[0] 并由 AiSlotNetSync 同步，要求<b>非负</b>，因此整体重新编号（仅数值变化，语义不变）。
        public enum AIStates : int
        {
            //动画
            onSpawnAnim = 0,
            roaringAnim = 1,
            onKillAnim = 2,

            //非攻击动作，也非动画
            dizzy = 3,
            rest = 4,

            //有可能会有破绽的攻击动作
            dive = 5,
            accumulate = 6,

            //普通攻击动作
            iceBreath = 7,
            horizontalDash = 8,
            smashDown = 9,

            //二阶段追加攻击动作
            iceThornsTrap = 10,
            iceCloud = 11,
            doubleDash = 12,

            //大师模式专属，特殊攻击动作
            iceTornado = 13,
            iciclesFall = 14
        }

        public override void AI()
        {
            if (!spwan)
            {
                NPC.oldPos = new Vector2[8];
                Moves = new List<int>();
                ResetMovePool(1);
                NPC.TargetClosest(false);
                NPC.frame.Y = 3;
                ExchangeState = true;
                Timer = 0;
                NormalMoveCount = 0;
                NPC.noTileCollide = false;
                NPC.netUpdate = true;

                spwan = true;
            }

            EnsureAiMachine();

            if (Target.ZoneSnow)
                FlyAwayTimer = 0;
            else
                FlyAwayTimer++;

            if ((CurrentStateId != (int)AIStates.onKillAnim && CurrentStateId != (int)AIStates.onSpawnAnim)
                && (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000 || FlyAwayTimer > 60 * 6))
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000 || !Target.ZoneSnow)//没有玩家存活时离开
                {
                    NPC.dontTakeDamage = false;
                    NPC.rotation = NPC.rotation.AngleTowards(0f, 0.14f);
                    NPC.velocity.X *= 0.98f;
                    canDrawShadows = false;
                    FlyUp();
                    NPC.EncourageDespawn(30);
                    return;
                }
            }

            // 顶层 FSM 驱动：状态 ID 经 ai[0] 同步，客户端跟随服务端权威；招式 body 在两端运行（移动/视觉）。
            StateMachine.Update();

            if (canDrawShadows)
            {
                for (int i = 0; i < 7; i++)
                    NPC.oldPos[i] = NPC.oldPos[i + 1];

                NPC.oldPos[7] = NPC.Center;
            }
        }

        private void EnsureAiMachine()
        {
            if (StateMachine != null)
                return;

            AiContext = new BabyIceDragonContext(this);
            StateMachine = new CoraliteBossStateMachine<BabyIceDragonContext>(AiContext);
            StateMachine.SetInitialState(VaultStateRegistry<BabyIceDragonContext>.Create((int)AIStates.onSpawnAnim));
        }

        internal void OnKillAnimBody()
        {
            {
                {
                        if (Timer < 60)
                        {
                            GlowAlpha += 1 / 60f;
                            NPC.velocity = -Vector2.UnitY;
                            NPC.frame.X = 1;
                            NPC.frame.Y = 0;

                            if (!VaultUtils.isServer)
                                Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(100, 100), DustID.ApprenticeStorm, Vector2.UnitY);
                        }
                        else
                        {
                            if (!VaultUtils.isServer)
                            {
                                for (int i = 0; i < 12; i++)
                                {
                                    Vector2 center = NPC.Center + Main.rand.NextVector2CircularEdge(2000, 2000);
                                    IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100),
                                        Main.rand.NextVector2CircularEdge(4, 4), 1f, () => center, 16);
                                }
                                for (int j = 0; j < 20; j++)
                                {
                                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(32, 32), ModContent.DustType<CrushedIceDust>(),
                                        -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f)) * Main.rand.Next(2, 7), Scale: Main.rand.NextFloat(1f, 1.4f));
                                }

                                for (int i = 0; i < 3; i++)
                                    PRTLoader.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo>(), Color.White, 0.15f);
                            }

                            Helper.PlayPitched("Icicle/Broken", 0.4f, 0f, NPC.Center);

                            NPC.Kill();
                        }
                        Timer++;
                    }
            }
        }

        internal void OnSpawnAnimBody()
        {
            {
                        do
                        {
                            if ((int)Timer == 0)
                            {
                                //生成动画弹幕
                                GlowAlpha = 1f;
                                if (!VaultUtils.isClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center
                                        , Vector2.Zero, ProjectileType<BabyIceDragon_OnSpawnAnim>(), 0, 0);
                                NPC.dontTakeDamage = true;
                                NPC.velocity.Y = -0.2f;
                                NPC.netUpdate = true;
                                break;
                            }

                            if (Timer < 30 && !VaultUtils.isServer)
                            {
                                GlowAlpha -= 1 / 30f;
                                Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(72, 72), DustID.ApprenticeStorm, Vector2.UnitY * Main.rand.NextFloat(2f, 4f),
                                      Scale: Main.rand.NextFloat(1f, 1.5f));
                                dust.noGravity = true;
                            }

                            GlowAlpha = 0;

                            if (Timer < 100)
                            {
                                NormallyFlyingFrame();
                                break;
                            }

                            if ((int)Timer == 100)
                            {
                                NPC.frame.Y = 3;
                                NPC.velocity *= 0;
                            }

                            if (Timer < 130)
                                break;

                            if ((int)Timer == 130)
                            {
                                NPC.frame.X = 1;
                                NPC.frame.Y = 1;
                                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                                GetMouseCenter(out _, out Vector2 mouseCenter);

                                if (!VaultUtils.isServer)
                                {
                                    PRTLoader.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                                    PunchCameraModifier modifier = new(NPC.Center, new Vector2(0.8f, 0.8f), 5f, 20f, 40, 1000f, "BabyIceDragon");
                                    Main.instance.CameraModifiers.Add(modifier);
                                }
                            }

                            if (Timer < 170)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                if (!VaultUtils.isServer)
                                {
                                    if ((int)Timer % 10 == 0)
                                        PRTLoader.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                                    if ((int)Timer % 20 == 0)
                                        PRTLoader.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                                }
                                break;
                            }

                            NPC.velocity.X *= 0.99f;
                            switch (NPC.frame.Y)
                            {
                                default:
                                    NPC.velocity.Y *= 0.96f;
                                    break;
                                case 2:
                                case 3:
                                    NPC.velocity.Y -= 0.3f;
                                    break;
                            }

                            if (NPC.velocity.Y < -12)
                                NPC.velocity.Y = -12;

                            NormallyFlyingFrame(changeRot: false);

                            if (Timer >= 260)
                            {
                                ResetStates();
                                break;
                            }
                        } while (false);

                        Timer++;
                    }
        }

        internal void RoaringAnimBody()
        {
            {
                        do
                        {
                            NPC.velocity *= 0.98f;

                            if ((int)Timer == 20)
                            {
                                NPC.frame.Y = 3;
                                NPC.velocity *= 0;
                            }

                            if (Timer < 40)
                                break;

                            if ((int)Timer == 40)
                            {
                                NPC.frame.X = 1;
                                NPC.frame.Y = 1;
                                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                                GetMouseCenter(out _, out Vector2 mouseCenter);

                                if (!VaultUtils.isServer)
                                {
                                    PRTLoader.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                                    PunchCameraModifier modifier = new(NPC.Center, new Vector2(0.8f, 0.8f), 5f, 20f, 40, 1000f, "BabyIceDragon");
                                    Main.instance.CameraModifiers.Add(modifier);
                                }
                            }

                            if (Timer < 80)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                if (!VaultUtils.isServer)
                                {
                                    if (Timer % 10 == 0)
                                        PRTLoader.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                                    if (Timer % 20 == 0)
                                        PRTLoader.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                                }

                                break;
                            }

                            ResetStates();

                        } while (false);

                        Timer++;
                    }
        }

        internal void DizzyBody()
        {
            {
                        if (Timer < 1)
                            HaveARest(30);

                        NPC.frame.X = 2;
                        if (NPC.velocity.Y < 0.1f && Framing.GetTileSafely(NPC.Bottom).HasTile)
                        {
                            NPC.frameCounter++;
                            if (NPC.frameCounter > 8)
                            {
                                NPC.frameCounter = 0;
                                NPC.frame.Y++;
                                if (NPC.frame.Y > 3)
                                    NPC.frame.Y = 0;
                            }
                        }
                        else
                            NPC.frame.Y = 4;

                        NPC.velocity.X *= 0.96f;
                        Timer--;
                    }
        }

        internal void RestBody()
        {
            {
                        NPC.velocity.X *= 0.97f;
                        NPC.rotation = NPC.rotation.AngleTowards(0f, 0.08f);
                        NPC.directionY = (Target.Center.Y - 150) > NPC.Center.Y ? 1 : -1;
                        float yLength = Math.Abs(Target.Center.Y - 100 - NPC.Center.Y);
                        if (yLength > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.14f, 0.1f, 0.96f);
                        else
                            NPC.velocity.Y *= 0.96f;

                        if (Timer < 0)
                        {
                            ResetStates();
                            return;
                        }

                        Timer--;
                        NormallyFlyingFrame();
                    }
        }

        internal void AccumulateBody()
        {
            {
                        do
                        {
                            if (Timer < 50)
                            {
                                NormallyFlyingFrame(changeRot: false);

                                NPC.noGravity = true;
                                SetDirection();
                                NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                                float yLength = Math.Abs(Target.Center.Y - NPC.Center.Y);
                                if (yLength > 50)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.14f, 0.1f, 0.96f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3f, 0.08f, 0.08f, 0.96f);
                                NPC.rotation = NPC.rotation.AngleTowards(0f, 0.06f);
                                break;
                            }

                            if ((int)Timer == 62)
                            {
                                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                                if (!VaultUtils.isServer)
                                {
                                    GetMouseCenter(out _, out Vector2 mouseCenter2);
                                    PRTLoader.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 0.8f);
                                    PRTLoader.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1.2f);
                                    for (int i = 0; i < 4; i++)
                                        IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100), Main.rand.NextVector2CircularEdge(3, 3), 1f, () =>
                                        {
                                            return NPC.Center + ((NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * 30);
                                        }, 16);
                                }
                            }

                            if (Timer < 80)
                            {
                                NormallyFlyingFrame(changeRot: false);
                                NPC.velocity *= 0.92f;
                                break;
                            }

                            //生成冰块NPC
                            if ((int)Timer == 80 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                movePhase = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.direction * 170), (int)NPC.Center.Y + 20, NPCType<IceCube>());
                                NPC.netUpdate = true;
                                NPC.noTileCollide = true;
                                NPC.noGravity = true;
                                NPC.netUpdate = true;
                            }

                            NormallyFlyingFrame(1, false);
                            Vector2 targetDir = (Main.npc[movePhase].Center - NPC.Center).SafeNormalize(Vector2.One);
                            NPC.velocity = targetDir.RotatedBy(-1.57f) * 4f * NPC.direction;
                            NPC.rotation = NPC.rotation.AngleTowards(NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0 : 3.14f), 0.8f);
                            GetMouseCenter(out _, out Vector2 mouseCenter);
                            PRTLoader.NewParticle(mouseCenter, targetDir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 8f,
                                    CoraliteContent.ParticleType<Fog>(), Color.AliceBlue, Main.rand.NextFloat(0.6f, 0.8f));

                            if ((int)Timer % 20 == 0)
                            {
                                //如果莫得冰块弹幕那么就重置AI
                                int npcIndex = Helper.GetNPCByType(NPCType<IceCube>());
                                if (npcIndex == -1)
                                {
                                    ResetStates();
                                    break;
                                }
                            }

                            if (Timer > 900)
                                ResetStates();
                        } while (false);

                        Timer++;
                    }
        }

        /// <summary>
        /// 飞上去，如果头顶被方块阻挡则无法飞上去
        /// </summary>
        public void FlyUp()
        {
            //根据帧图来改变速度，大概效果是扇一下翅膀向上飞一小段
            //只有扇翅膀的时候才会有向上加速度，否则减速
            switch (NPC.frame.Y)
            {
                default:
                    NPC.velocity.Y *= 0.94f;
                    break;
                case 2:
                case 3:
                    NPC.velocity.Y -= 0.7f;
                    break;
            }

            if (NPC.velocity.Y < -8)
                NPC.velocity.Y = -8;

            NormallyFlyingFrame(changeRot: false);
        }

        public void GetMouseCenter(out Vector2 targetDir, out Vector2 mouseCenter)
        {
            targetDir = (NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2();
            mouseCenter = NPC.Center + (targetDir * 40 * NPC.scale);
        }

        public void SetDirection()
        {
            if (Math.Abs(NPC.Center.X - Target.Center.X) < 16)
                return;

            NPC.direction = NPC.Center.X > Target.Center.X ? -1 : 1;
            NPC.spriteDirection = NPC.direction;
        }

        public Vector2 GetDizzyStarCenter()
        {
            if (NPC.frame.Y == 4)
                return NPC.Center + new Vector2(NPC.direction * 30, -24);

            return NPC.Center + new Vector2(NPC.direction * 30, -30);
        }

        public void InitCaches()
        {
            for (int i = 0; i < 8; i++)
                NPC.oldPos[i] = NPC.Center;
        }

        #region 冰刺

        public void SpawnIceThorns()
        {
            if (!VaultUtils.isServer)
            {
                PunchCameraModifier modifier = new(NPC.Center, new Vector2(0f, 1f), 20f, 6f, 30, 1000f, "BabyIceDragon");
                Main.instance.CameraModifiers.Add(modifier);
            }
            if (!VaultUtils.isClient)
            {
                Point sourceTileCoords = NPC.Bottom.ToTileCoordinates();
                //sourceTileCoords.X += 1;
                for (int i = 0; i < 4; i++)
                {
                    TryMakingSpike(ref sourceTileCoords, 1, 20, i * 6, 1, i * 0.2f);
                    sourceTileCoords.X += 2;
                }

                sourceTileCoords = NPC.Bottom.ToTileCoordinates();
                //sourceTileCoords.X -= 1;
                for (int i = 0; i < 4; i++)
                {
                    TryMakingSpike(ref sourceTileCoords, -1, 20, i * 6, 1, i * 0.2f);
                    sourceTileCoords.X -= 2;
                }
            }
        }

        private void TryMakingSpike(ref Point sourceTileCoords, int dir, int howMany, int whichOne, int xOffset, float scaleOffset)
        {
            int position_X = sourceTileCoords.X + (xOffset * dir);
            int position_Y = TryMakingSpike_FindBestY(ref sourceTileCoords, position_X);
            if (WorldGen.ActiveAndWalkableTile(position_X, position_Y))
            {
                Vector2 position = new((position_X * 16) + 8, (position_Y * 16) - 8);
                Vector2 velocity = new Vector2(0f, -1f).RotatedBy(whichOne * dir * 0.7f * ((float)Math.PI / 4f / howMany));
                int damage = Helper.GetProjDamage(40, 45, 60);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ProjectileID.DeerclopsIceSpike, damage, 0f, NPC.target, 0f, 0.4f + scaleOffset + (xOffset * 1.1f / howMany));
            }
        }

        private int TryMakingSpike_FindBestY(ref Point sourceTileCoords, int x)
        {
            int position_Y = sourceTileCoords.Y;
            NPCAimedTarget targetData = NPC.GetTargetData();
            if (!targetData.Invalid)
            {
                Rectangle hitbox = targetData.Hitbox;
                Vector2 vector = new(hitbox.Center.X, hitbox.Bottom);
                int num2 = (int)(vector.Y / 16f);
                int num3 = Math.Sign(num2 - position_Y);
                int num4 = num2 + (num3 * 15);
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

            for (int j = 0; j < 8; j++)
            {
                if (position_Y < 10)
                    break;

                if (!WorldGen.SolidTile(x, position_Y))
                    break;

                position_Y--;
            }

            for (int k = 0; k < 8; k++)
            {
                if (position_Y > Main.maxTilesY - 10)
                    break;

                if (WorldGen.ActiveAndWalkableTile(x, position_Y))
                    break;

                position_Y++;
            }

            return position_Y;
        }

        #endregion

        #region 状态切换相关

        public void ResetStates()
        {
            if (VaultUtils.isClient)
                return;

            movePhase = 0;
            canDrawShadows = false;
            NPC.dontTakeDamage = false;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            Timer = 0;
            GlowAlpha = 0;
            NPC.TargetClosest(false);

            NPC.netUpdate = true;

            int phase;

            //大师模式更早进入2阶段
            if (Main.masterMode)
                phase = NPC.life < (int)(NPC.lifeMax * 0.75f) ? 2 : 1;
            else
                phase = NPC.life < (NPC.lifeMax / 2) ? 2 : 1;

            if (ExchangeState && phase == 2)    //进入2阶段时固定进入吼叫动画
            {
                ExchangeState = false;
                Main.StartRain();
                Main.SyncRain();
                ResetEffects();
                ResetMovePool(2);
                StateMachine.ChangeState((int)AIStates.roaringAnim);
                return;
            }

            //有破绽的行动
            if (CanVulnerableMove())
            {
                NormalMoveCount = 0;
                ResetMovePool(phase);

                int vulnerable = Main.rand.Next(2) switch
                {
                    0 => (int)AIStates.dive,
                    _ => (int)AIStates.accumulate
                };
                StateMachine.ChangeState(vulnerable);
                return;
            }

            // 招池仍是“用过即移除”的可枯竭列表，但选取改用基座 WeightedRandomPicker（等概率，确定性纯函数化）。
            int next;
            if (Moves.Count > 0)
            {
                WeightedRandomPicker<int> picker = new(Moves.Select(m => (m, 1f)));
                (int move, int index) = picker.Pick(Main.rand.Next());
                Moves.RemoveAt(index);
                next = move;
            }
            else
            {
                next = (int)AIStates.iceBreath;
            }

            NormalMoveCount += 1f;
            StateMachine.ChangeState(next);
        }

        public void Dizzy(int dizzyTime)
        {
            movePhase = 0;
            canDrawShadows = false;
            Helper.PlayPitched("Icicle/Broken", 0.4f, 0f, NPC.Center);
            SetDirection();
            DizzyStar.Spawn(NPC.Center, -1.57f, dizzyTime, 10, GetDizzyStarCenter);
            DizzyStar.Spawn(NPC.Center, 1.57f, dizzyTime, 10, GetDizzyStarCenter);
            GetMouseCenter(out _, out Vector2 mouseCenter);
            for (int j = 0; j < 8; j++)
            {
                Dust.NewDustPerfect(mouseCenter, DustType<CrushedIceDust>(), -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(2f, 5f),
                    Scale: Main.rand.NextFloat(1f, 1.4f));
            }

            NPC.velocity = new Vector2(-NPC.direction * 4f, -4f);
            NPC.rotation = 0f;
            NPC.frameCounter = 0;
            NPC.frame.X = 2;

            Timer = dizzyTime;
            NormalMoveCount = 0;        //归零这个计数（虽说感觉可能没什么用的样子）
            NPC.noGravity = false;
            NPC.noTileCollide = false;

            if (VaultUtils.isClient)
                return;

            NPC.netUpdate = true;
            // 仅服务端推进顶层状态，客户端经 ai[0] 同步跟随（Timer 等已在上方双端写入）。
            StateMachine?.ChangeState((int)AIStates.dizzy);
        }

        public void HaveARest(int restTime)
        {
            movePhase = 0;
            canDrawShadows = false;

            Timer = restTime;
            NPC.TargetClosest();
            SetDirection();
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            if (VaultUtils.isClient)
                return;

            NPC.netUpdate = true;
            StateMachine?.ChangeState((int)AIStates.rest);
        }

        public void ResetMovePool(int phase)
        {
            Moves.Clear();

            switch (phase)
            {
                default:
                case 1:
                    for (int i = 0; i < 4; i++)
                        Moves.Add((int)AIStates.iceBreath);
                    for (int i = 0; i < 2; i++)
                        Moves.Add((int)AIStates.horizontalDash);
                    for (int i = 0; i < 4; i++)
                        Moves.Add((int)AIStates.smashDown);
                    break;
                case 2:
                    if (Main.masterMode)
                    {
                        Moves.Add((int)AIStates.iceBreath);
                        Moves.Add((int)AIStates.horizontalDash);
                        Moves.Add((int)AIStates.smashDown);
                        for (int i = 0; i < 2; i++)
                            Moves.Add((int)AIStates.iceThornsTrap);
                        for (int i = 0; i < 2; i++)
                            Moves.Add((int)AIStates.doubleDash);
                        Moves.Add((int)AIStates.iceCloud);
                        Moves.Add((int)AIStates.iciclesFall);
                        Moves.Add((int)AIStates.iceTornado);
                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                            Moves.Add((int)AIStates.iceBreath);
                        Moves.Add((int)AIStates.horizontalDash);
                        for (int i = 0; i < 2; i++)
                            Moves.Add((int)AIStates.smashDown);
                        for (int i = 0; i < 2; i++)
                            Moves.Add((int)AIStates.iceThornsTrap);
                        for (int i = 0; i < 2; i++)
                            Moves.Add((int)AIStates.doubleDash);
                        Moves.Add((int)AIStates.iceCloud);
                    }
                    break;
            }
        }

        public bool CanVulnerableMove()
        {
            //大师模式行动每7次进行一次有破绽动作
            if (Main.masterMode)
            {
                if (NormalMoveCount > 6)
                    return true;
                return false;
            }

            //其他模式每5次动作进行一次
            if (NormalMoveCount > 4)
                return true;

            return false;
        }

        #endregion

        #endregion

        #region Frames

        /// <summary>
        /// 简易的控制帧图
        /// </summary>
        public void NormallyFlyingFrame(int xFrame = 0, bool changeRot = true)
        {
            NPC.frame.X = xFrame;
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += 1;
                if (NPC.frame.Y > 3)
                    NPC.frame.Y = 0;
            }

            if (changeRot)
                NPC.rotation = NPC.direction * NPC.velocity.Y * 0.05f;
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            Rectangle frameBox = mainTex.Frame(3, 5, NPC.frame.X, NPC.frame.Y);//new Rectangle(NPC.frame.X * FrameWidth, NPC.frame.Y * FrameHeight, FrameWidth, FrameHeight);
            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = frameBox.Size() / 2;

            if (NPC.spriteDirection != 1)
                effects = SpriteEffects.FlipHorizontally;

            if (canDrawShadows)
            {
                Color color = new Color(43, 255, 198, 255) * 0.5f;
                float scale = NPC.scale;
                for (int i = 7; i > -1; i -= 2)
                {
                    spriteBatch.Draw(mainTex, NPC.oldPos[i] - screenPos, frameBox, color, NPC.rotation, origin, scale, effects, 0f);
                    color *= 0.75f;
                    scale *= 0.98f;
                }
            }

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            if (GlowAlpha > 0.1f)
                spriteBatch.Draw(GlowTex.Value, NPC.Center - screenPos, frameBox, Color.White * GlowAlpha, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }

        #endregion

        #region NetWork
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(movePhase);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            movePhase = reader.ReadInt32();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        #endregion
    }
}
