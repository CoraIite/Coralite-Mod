using Coralite.Content.Items.Thunder;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    [AutoloadBossHead]
    public partial class ThunderveinDragon : ModNPC
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + Name;

        private Player Target => Main.player[NPC.target];

        internal ref float Phase => ref NPC.ai[0];
        internal ref float State => ref NPC.ai[1];
        internal ref float SonState => ref NPC.ai[2];
        internal ref float Timer => ref NPC.ai[3];

        internal ref float Recorder => ref NPC.localAI[0];
        internal ref float Recorder2 => ref NPC.localAI[1];
        internal ref float StateRecorder => ref NPC.localAI[2];
        internal ref float UseMoveCount => ref NPC.localAI[3];

        public readonly int trailCacheLength = 12;
        public Point[] oldFrame;
        public int[] oldDirection;

        public float selfAlpha = 1f;
        public float anmiAlpha;

        public static Color ThunderveinYellowAlpha = new Color(255, 202, 101, 0);
        public static Color ThunderveinPurpleAlpha = new Color(135, 94, 255, 0);
        public static Color ThunderveinOrangeAlpha = new Color(219, 114, 22, 0);

        /// <summary>
        /// 是否绘制残影
        /// </summary>
        public bool canDrawShadows;
        /// <summary>
        /// 是否绘制冲刺是的特殊贴图
        /// </summary>
        public bool isDashing;

        /// <summary>
        /// 残影的透明度
        /// </summary>
        public float shadowAlpha = 1f;
        /// <summary>
        /// 残影的大小
        /// </summary>
        public float shadowScale = 1f;

        public int oldSpriteDirection;

        /// <summary>
        /// 身上有电流环绕，会减伤并生成闪电粒子
        /// </summary>
        public bool currentSurrounding;

        public bool Initialize = true;

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 100;
            NPC.damage = 60;
            NPC.defense = 50;
            NPC.lifeMax = 45500;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 12, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.BossBar = ModContent.GetInstance<ThunderveinDragonBossBar>();
            ModContent.GetInstance<ThunderveinDragonBossBar>().Reset(NPC);

            //BGM：暂无
            //if (!Main.dedServ)
            //    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/IcyColdStream");
            Music = MusicID.OtherworldlyTowers;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((39000 + numPlayers * 15500) / journeyScale);
                    NPC.damage = 66;
                    NPC.defense = 50;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((45500 + numPlayers * 19550) / journeyScale);
                    NPC.damage = 72;
                    NPC.defense = 50;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                    NPC.defense = 50;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 2.4f;
                }

                return;
            }

            NPC.lifeMax = 39000 + numPlayers * 15500;
            NPC.damage = 66;
            NPC.defense = 50;

            if (Main.masterMode)
            {
                NPC.lifeMax = 45500 + numPlayers * 19550;
                NPC.damage = 72;
                NPC.defense = 50;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 52000 + numPlayers * 25850;
                NPC.damage = 80;
                NPC.defense = 50;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 2.4f;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ThunderveinDragonRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<ThunderveinSoulStone>(), 4));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ThunderveinDragonBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThunderveinDragonTrophy>(), 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThunderveinDragonMask>(), 7));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ZapCrystal>(), 1, 6, 8));
            npcLoot.Add(notExpertRule);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool? CanCollideWithPlayerMeleeAttack(Player player, Item item, Rectangle meleeAttackHitbox)
        {
            return base.CanCollideWithPlayerMeleeAttack(player, item, meleeAttackHitbox);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (currentSurrounding)
                modifiers.SourceDamage -= 0.4f;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Colliding(projectile.getRect(), HeadHitBox()))
                modifiers.SourceDamage += 0.15f;

            if (currentSurrounding)
                modifiers.SourceDamage -= 0.4f;

            if (projectile.hostile)
                modifiers.SourceDamage -= 0.5f;
        }

        public Rectangle HeadHitBox()
        {
            Vector2 pos = GetMousePos();
            return new Rectangle((int)(pos.X - 22), (int)(pos.Y - 22), 44, 44);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
            {
                Particle.NewParticle(NPC.Center.MoveTowards(projectile.Center, 50), Vector2.Zero,
                    CoraliteContent.ParticleType<LightningParticle>(), Scale: Main.rand.NextFloat(1f, 1.5f));
            }
        }

        public override void OnKill()
        {
            DownedBossSystem.DownThunderveinDragon();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.StopRain();
                Main.SyncRain();
            }
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override bool CheckDead()
        {
            if ((int)State != (int)AIStates.onKillAnim)
            {
                State = (int)AIStates.onKillAnim;
                SonState = 0;
                Timer = 0;
                NPC.dontTakeDamage = true;
                currentSurrounding = true;
                canDrawShadows = false;
                isDashing = false;
                NPC.life = 1;
                return false;
            }

            return true;
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            int width = (int)(95 * NPC.scale);
            int height = (int)(70 * NPC.scale);
            npcHitbox = new Rectangle((int)(NPC.Center.X - width / 2), (int)(NPC.Center.Y - height / 2), width, height);
            return true;
        }

        #endregion

        #region AI

        public enum AIStates
        {
            onSpawnAnmi = 1,
            onKillAnim,

            /// <summary> 短冲，用于调整身位 </summary>
            SmallDash,
            /// <summary> 闪电突袭，先3段短冲后进行一次长冲，二阶段的长冲会在路径上留下电球 </summary>
            LightningRaid,
            /// <summary> 放电，在身体周围生成电流环绕 </summary>
            Discharging,
            /// <summary> 闪电吐息，原地转一圈后使用吐息，二阶段改为使用电磁炮，会根据玩家位置持续调整方向 </summary>
            LightningBreath,
            /// <summary> 电球，吐出一个电球，二阶段时吐出多个 </summary>
            LightningBall,
            /// <summary> 电球，吐出一个电球，飞行一段时间后向四周爆开 </summary>
            CrossLightingBall,
            /// <summary> 落雷，先吼叫一声后飞向空中并隐身，之后选择落点，再下落，二阶段会连续使用，最多3次 </summary>
            FallingThunder,

            /// <summary> 一二阶段的切换动画 </summary>
            ExchangeP1_P2,

            /// <summary> 先冲刺，再放电 </summary>
            DashDischarging,
            /// <summary> 引力雷球 </summary>
            GravitationThunder,
            /// <summary> 电磁炮 </summary>
            ElectromagneticCannon,
            /// <summary> 冥雷，旋转飞，之后进入背景，并生成一些幻影，在天被照亮时才能看到，击破一定数量幻影后打断招式并使用落雷<br></br>
            /// 否则就释放超大范围放电
            /// </summary>
            StygianThunder
        }

        public override void OnSpawn(IEntitySource source)
        {
            ResetAllOldCaches();
            Phase = 1;
            State = (int)AIStates.onSpawnAnmi;
            if (!SkyManager.Instance["ThunderveinSky"].IsActive())//如果这个天空没激活
                SkyManager.Instance.Activate("ThunderveinSky");
        }

        public override void AI()
        {
            ThunderveinPurpleAlpha = new Color(135, 94, 255, 0);
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000 || !Target.ZoneSnow)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 4500)//没有玩家存活时离开
                {
                    NPC.dontTakeDamage = false;
                    canDrawShadows = false;
                    isDashing = false;
                    State = -1;
                    NPC.spriteDirection = 1;
                    NPC.rotation = NPC.rotation.AngleTowards(0f, 0.14f);
                    NPC.velocity.X *= 0.98f;
                    FlyingUp(0.3f, 20, 0.9f);
                    NPC.EncourageDespawn(30);
                    return;
                }
            }

            UpdateSky();

            if (Initialize)
            {
                if (Main.zenithWorld)
                {
                    ThunderveinYellowAlpha = new Color(255, 171, 248, 0);
                    ThunderveinPurpleAlpha = new Color(6, 184, 217, 0);
                    ThunderveinOrangeAlpha = new Color(255, 157, 175, 0);
                }
                else
                {
                    ThunderveinYellowAlpha = new Color(255, 202, 101, 0);
                    ThunderveinPurpleAlpha = new Color(135, 94, 255, 0);
                    ThunderveinOrangeAlpha = new Color(219, 114, 22, 0);
                }
                Initialize = false;
            }
            switch (State)
            {
                default:
                    ResetStates();
                    break;
                case (int)AIStates.onSpawnAnmi:
                    OnSpawnAnmi();
                    break;
                case (int)AIStates.onKillAnim:
                    OnKillAnmi();
                    break;
                case (int)AIStates.SmallDash://短距离冲刺
                    {
                        if (Phase == 1)
                            SmallDashP1();
                        else
                            SmallDashP2();
                    }
                    break;
                case (int)AIStates.LightningRaid://闪电突袭
                    {
                        if (Phase == 1)
                            LightningRaidP1();
                        else
                            LightningRaidP2();
                    }
                    break;
                case (int)AIStates.Discharging://闪电突袭
                    Discharging();
                    break;
                case (int)AIStates.LightningBreath://闪电吐息
                    {
                        if (Phase == 1)
                            LightingBreathP1();
                        else
                            LightingBreathP2();
                    }
                    break;
                case (int)AIStates.LightningBall://闪电吐息
                    LightingBall();
                    break;
                case (int)AIStates.CrossLightingBall://闪电吐息
                    CrossLightingBall();
                    break;
                case (int)AIStates.FallingThunder://闪电吐息
                    {
                        if (Phase == 1)
                            FallingThunderP1();
                        else
                            FallingThunderP2();
                    }
                    break;
                case (int)AIStates.ExchangeP1_P2://切换动画
                    ExchangeP1_P2();
                    break;
                case (int)AIStates.DashDischarging://冲刺放电
                    DashDischarging();
                    break;
                case (int)AIStates.GravitationThunder://引力电球
                    GravitationThunder();
                    break;
                case (int)AIStates.ElectromagneticCannon://引力电球
                    ElectromagneticCannon();
                    break;
                case (int)AIStates.StygianThunder://引力电球
                    StygianThunder();
                    break;
            }
        }

        public override void PostAI()
        {
            oldSpriteDirection = NPC.spriteDirection;

            if (currentSurrounding && Main.rand.NextBool(3))
            {
                Vector2 offset = Main.rand.NextVector2Circular(100 * NPC.scale, 70 * NPC.scale);
                ElectricParticle_Follow.Spawn(NPC.Center, offset, () => NPC.Center, Main.rand.NextFloat(0.75f, 1f));
            }
        }

        public void OnSpawnAnmi()
        {
            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0:
                    {
                        NPC.TargetClosest();
                        NPC.dontTakeDamage = true;
                        TurnToNoRot(1);
                        Recorder = -1;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile proj = Main.projectile[i];
                            if (proj.active && proj.type == ModContent.ProjectileType<ThunderSpawn>())
                            {
                                Recorder = i;
                                break;
                            }
                        }

                        if (Recorder == -1)
                        {
                            NPC.Center = Target.Center + new Vector2(0, -400);
                        }
                        else
                        {
                            NPC.Center = Main.projectile[(int)Recorder].Center + new Vector2(0, -400);
                        }

                        SonState++;
                        selfAlpha = 0;
                    }
                    break;
                case 1:
                    {
                        FlyingFrame();
                        selfAlpha += 1 / 80f;
                        Timer++;
                        if (Timer > 80)
                        {
                            SonState++;
                            Timer = 0;
                            //生成名称
                            NPC.NewProjectileDirectInAI<ThunderveinDragon_OnSpawnAnim>(NPC.Center, Vector2.Zero, 1, 0, NPC.target);
                        }
                    }
                    break;
                case 2://出现
                    {
                        FlyingFrame();
                        Timer++;
                        if (Timer > 30)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 3://吼叫
                    {
                        UpdateAllOldCaches();

                        NPC.QuickSetDirection();
                        TurnToNoRot();
                        NPC.velocity *= 0.9f;
                        if (Timer == 0 && NPC.frame.Y != 4)
                        {
                            FlyingFrame();
                            break;
                        }

                        if (Timer == 15)
                        {
                            NPC.frame.Y = 0;
                            NPC.frame.X = 1;
                            NPC.velocity *= 0;
                            SoundStyle st = CoraliteSoundID.LightningOrb_Item121;
                            st.Pitch = 0.4f;
                            SoundEngine.PlaySound(st, NPC.Center);
                            SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                        }
                        else if (Timer > 15 && Timer < 130)
                        {
                            Vector2 pos = NPC.Center + (NPC.rotation).ToRotationVector2() * 60 * NPC.Center;
                            if ((int)Timer % 10 == 0)
                            {
                                var modifyer = new PunchCameraModifier(NPC.Center, Helper.NextVec2Dir(), 8, 12, 20, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                                Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Coralite.Instance.ThunderveinYellow, 0.2f);
                            }
                            if ((int)Timer % 20 == 0)
                                Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.2f);
                        }

                        Timer++;
                        if (Timer > 150)
                            ResetStates();
                    }
                    break;

            }
        }

        public void OnKillAnmi()
        {
            if (Timer < 60)
            {
                anmiAlpha += 1 / 60f;
                NPC.velocity = -Vector2.UnitY;
                NPC.frame.X = 1;
                NPC.frame.Y = 0;
            }
            else
            {
                for (int i = 0; i < 30; i++)
                {
                    float factor = i / 30f;
                    float length = Helper.Lerp(80, 400, factor);

                    for (int j = 0; j < 5; j++)
                    {
                        Particle.NewParticle(NPC.Center + Main.rand.NextVector2CircularEdge(length, length),
                            Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(), Scale: Main.rand.NextFloat(0.9f, 1.3f));
                    }
                }

                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, NPC.Center);

                NPC.Kill();
            }

            Timer++;
        }

        #endregion

        #region States

        public void ResetStates()
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            shadowAlpha = 1;
            shadowScale = 1;
            canDrawShadows = false;
            isDashing = false;
            currentSurrounding = false;
            NPC.dontTakeDamage = false;

            Timer = 0;
            SonState = 0;
            Recorder = 0;
            Recorder2 = 0;

            List<int> moves = new List<int>();
            int oldState = (int)State;
            float distance = Vector2.Distance(NPC.Center, Target.Center);
            int dir = Target.Center.X > NPC.Center.X ? 1 : -1;

            moves.Add((int)AIStates.LightningRaid);
            moves.Add((int)AIStates.FallingThunder);
            moves.Add((int)AIStates.LightningBall);
            moves.Add((int)AIStates.LightningBreath);

            if (Main.masterMode)
                moves.Add((int)AIStates.CrossLightingBall);

            if (dir != NPC.spriteDirection)//玩家在背后时，大概率使用闪电突袭
            {
                for (int i = 0; i < 5; i++)
                    moves.Add((int)AIStates.LightningRaid);
            }

            if (oldState != (int)AIStates.SmallDash)//如果上次招式不是小冲刺那就小冲一下
            {
                for (int i = 0; i < 7; i++)
                    moves.Add((int)AIStates.SmallDash);
            }

            if (distance < 420)//距离较近是大概率使用放电
            {
                for (int i = 0; i < 4; i++)
                    moves.Add((int)AIStates.Discharging);
            }

            switch (Phase)
            {
                default:
                    SetPhase();
                    break;
                case 1://一阶段
                    {
                        if (SetPhase())
                            return;

                        if (distance > 800)//距离较大，使用闪电突袭，距离再大就直接落雷
                        {
                            if (distance > 1400)
                                for (int i = 0; i < 7; i++)
                                    moves.Add((int)AIStates.FallingThunder);
                            else
                                for (int i = 0; i < 7; i++)
                                    moves.Add((int)AIStates.LightningRaid);
                        }

                        //当上次使用的是短距离冲刺的话，额外移除上上次所使用的招式
                        if (oldState == (int)AIStates.SmallDash)
                            moves.RemoveAll(i => i == (int)StateRecorder);
                        //移除上次使用的招式
                        moves.RemoveAll(i => i == oldState);

                        //随机一个招式出来
                        State = Main.rand.NextFromList(moves.ToArray());
                        //State = (int)AIStates.LightningRaid;
                    }
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    {
                        if (UseSpecialMove())
                            break;

                        moves.Add((int)AIStates.DashDischarging);

                        if (distance > 800)//距离较大，使用闪电突袭，距离再大就直接落雷
                        {
                            if (distance > 1400)
                                for (int i = 0; i < 7; i++)
                                    moves.Add((int)AIStates.FallingThunder);
                            else
                                for (int i = 0; i < 7; i++)
                                    moves.Add((int)AIStates.DashDischarging);
                        }

                        if (UseMoveCount > 7)
                        {
                            for (int i = 0; i < (int)UseMoveCount; i++)
                                moves.Add((int)AIStates.GravitationThunder);
                        }

                        //当上次使用的是短距离冲刺的话，额外移除上上次所使用的招式
                        if (oldState == (int)AIStates.SmallDash)
                            moves.RemoveAll(i => i == (int)StateRecorder);
                        //移除上次使用的招式
                        moves.RemoveAll(i => i == oldState);

                        //随机一个招式出来
                        State = Main.rand.NextFromList(moves.ToArray());
                        //State = (int)AIStates.LightningBreath;

                        UseMoveCount++;
                        //如果使用了引力雷球那么重置计时
                        if (State == (int)AIStates.GravitationThunder)
                            UseMoveCount = 0;
                    }
                    break;
            }

            //如果本次使用的是短距离冲刺那么旧记录上一招
            if ((int)State == (int)AIStates.SmallDash)
                StateRecorder = oldState;
        }

        public bool SetPhase()
        {
            int oldPhase = (int)Phase;
            if (Main.masterMode || Main.getGoodWorld)
            {
                if (NPC.life < NPC.lifeMax * 3 / 4)
                    Phase = 2;
                else
                    Phase = 1;
            }
            else
            {
                if (NPC.life < NPC.lifeMax / 2)
                    Phase = 2;
                else
                    Phase = 1;
            }

            if (oldPhase == 1 && Phase == 2)
            {
                State = (int)AIStates.ExchangeP1_P2;
                return true;
            }

            return false;
        }

        public bool UseSpecialMove()
        {
            if (Main.masterMode || Main.getGoodWorld)
            {
                if (Phase == 2 && NPC.life < NPC.lifeMax / 2)//血量1/2时必定使用冥雷
                {
                    Phase = 3;
                    State = (int)AIStates.StygianThunder;
                    return true;
                }

                if (Phase == 3 && NPC.life < NPC.lifeMax / 4)//血量1/4时必定使用冥雷
                {
                    Phase = 4;
                    State = (int)AIStates.StygianThunder;
                    return true;
                }

                if (Phase == 5 && NPC.life < NPC.lifeMax / 8)//血量1/8时必定使用冥雷
                {
                    Phase = 5;
                    State = (int)AIStates.StygianThunder;
                    return true;
                }

                return false;
            }

            if (Phase == 2 && NPC.life < NPC.lifeMax / 4)
            {
                Phase = 3;
                State = (int)AIStates.StygianThunder;
                return true;
            }

            if (Phase == 3 && NPC.life < NPC.lifeMax / 8)
            {
                Phase = 4;
                State = (int)AIStates.StygianThunder;
                return true;
            }

            return false;
        }

        public void ResetToSelectedState(AIStates state)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            shadowAlpha = 1;
            shadowScale = 1;
            canDrawShadows = false;
            isDashing = false;
            currentSurrounding = false;
            NPC.dontTakeDamage = false;

            Timer = 0;
            SonState = 0;
            Recorder = 0;
            Recorder2 = 0;

            State = (int)state;
        }

        #endregion

        #region HelperMethods

        public void GetLengthToTargetPos(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = NPC.Center.X - targetPos.X;
            yLength = NPC.Center.Y - targetPos.Y;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }

        /// <summary>
        /// 向上飞，会改变速度
        /// </summary>
        /// <param name="acc">加速度</param>
        /// <param name="velMax">速度最大值</param>
        /// <param name="slowDownPercent">减速率</param>
        public void FlyingUp(float acc, float velMax, float slowDownPercent)
        {
            FlyingFrame();

            if (NPC.frame.Y <= 4)
            {
                NPC.velocity.Y -= acc;
                if (NPC.velocity.Y > velMax)
                    NPC.velocity.Y = velMax;
            }
            else
                NPC.velocity.Y *= slowDownPercent;
        }

        public void FlyingFrame(bool openMouse = false)
        {
            NPC.frame.X = openMouse ? 1 : 0;

            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }
        }

        public void DashFrame()
        {
            NPC.frame.X = 2;
            NPC.frame.Y = 0;
        }

        public Vector2 GetMousePos()
        {
            return NPC.Center + (NPC.rotation - NPC.direction * 0.1f).ToRotationVector2() * 60 * NPC.scale;
        }

        /// <summary>
        /// 根据Y方向速度设置旋转
        /// </summary>
        public void SetRotationNormally(float rate = 0.08f)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;
            float targetRot = NPC.velocity.Y * 0.05f * NPC.spriteDirection + (NPC.spriteDirection > 0 ? 0 : MathHelper.Pi);
            NPC.rotation = NPC.rotation.AngleLerp(targetRot, rate);
        }

        /// <summary>
        /// 将身体回正
        /// </summary>
        /// <param name="rate"></param>
        public void TurnToNoRot(float rate = 0.2f)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            NPC.rotation = NPC.rotation.AngleLerp(NPC.spriteDirection > 0 ? 0 : MathHelper.Pi, rate);
        }

        public static void SetBackgroundLight(float light, int fadeTime, int exchangeTime = 5)
        {
            ThunderveinSky sky = ((ThunderveinSky)SkyManager.Instance["ThunderveinSky"]);
            sky.ExchangeTime = sky.MaxExchangeTime = exchangeTime;
            sky.targetLight = light;
            sky.oldLight = sky.light;
            sky.LightTime = fadeTime;
        }

        public static void UpdateSky()
        {
            ThunderveinSky sky = ((ThunderveinSky)SkyManager.Instance["ThunderveinSky"]);
            if (sky.Timeleft < 100)
                sky.Timeleft += 2;
            if (sky.Timeleft > 100)
                sky.Timeleft = 100;
        }

        public void InitOldFrame()
        {
            oldFrame ??= new Point[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldFrame[i] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void InitOldDirection()
        {
            oldDirection ??= new int[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldDirection[i] = NPC.spriteDirection;
        }

        public void UpdateOldFrame()
        {
            for (int i = 0; i < oldFrame.Length - 1; i++)
                oldFrame[i] = oldFrame[i + 1];
            oldFrame[^1] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void UpdateOldDirection()
        {
            for (int i = 0; i < oldDirection.Length - 1; i++)
                oldDirection[i] = oldDirection[i + 1];
            oldDirection[^1] = NPC.spriteDirection;
        }

        public void ResetAllOldCaches()
        {
            NPC.InitOldPosCache(trailCacheLength);
            NPC.InitOldRotCache(trailCacheLength);
            InitOldFrame();
            InitOldDirection();
        }

        public void UpdateAllOldCaches()
        {
            NPC.UpdateOldPosCache();
            NPC.UpdateOldRotCache();
            UpdateOldFrame();
            UpdateOldDirection();
        }

        #endregion

        #region 绘制部分

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();

            var frameBox = mainTex.Frame(3, 8, NPC.frame.X, NPC.frame.Y);
            var pos = NPC.Center - screenPos;
            var origin = frameBox.Size() / 2;
            float rot = NPC.rotation;

            SpriteEffects effects = SpriteEffects.None;

            if (NPC.spriteDirection < 0)
            {
                effects = SpriteEffects.FlipVertically;
            }

            //绘制残影
            if (canDrawShadows)
            {
                Color shadowColor = ThunderveinYellowAlpha;
                shadowColor.A = 50;
                shadowColor *= shadowAlpha;
                for (int i = 0; i < trailCacheLength; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i] - screenPos;
                    float oldrot = NPC.oldRot[i];
                    var frameOld = mainTex.Frame(3, 8, oldFrame[i].X, oldFrame[i].Y);
                    float factor = (float)i / trailCacheLength;
                    if (Phase == 2)
                    {
                        Color c1 = ThunderveinYellowAlpha;
                        c1.A = 50;
                        Color c2 = ThunderveinPurpleAlpha;
                        c2.A = 50;
                        shadowColor = Color.Lerp(c2, c1, factor);
                        shadowColor *= shadowAlpha;
                    }

                    SpriteEffects oldEffect = oldDirection[i] > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
                    spriteBatch.Draw(mainTex, oldPos, frameOld, shadowColor * factor, oldrot, origin
                        , NPC.scale * shadowScale * (1 - (1 - factor) * 0.3f), oldEffect, 0);
                }
            }

            //绘制自己
            if (Main.zenithWorld)
                drawColor *= 0.2f;
            spriteBatch.Draw(mainTex, pos, frameBox, drawColor * selfAlpha, rot, origin, NPC.scale, effects, 0);
            //绘制glow
            spriteBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "ThunderveinDragon_Glow").Value
                , pos, frameBox, Color.White * 0.75f * selfAlpha, rot, origin, NPC.scale, effects, 0);

            //绘制冲刺时的特效
            if (isDashing)
            {
                Texture2D exTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "StrikeTrail").Value;

                Vector2 exOrigin = new Vector2(exTex.Width * 6 / 10, exTex.Height / 2);

                Vector2 scale = new Vector2(1.3f, 1.5f) * NPC.scale;
                spriteBatch.Draw(exTex, pos, null, ThunderveinYellowAlpha, rot
                    , exOrigin, scale, effects, 0);
                scale.Y *= 1.2f;
                spriteBatch.Draw(exTex, pos - NPC.rotation.ToRotationVector2() * 50, null, ThunderveinYellowAlpha * 0.5f, rot
                    , exOrigin, scale, effects, 0);
            }

            if (State == (int)AIStates.onKillAnim)
            {
                Texture2D whiteTex = ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "ThunderveinDragon_Highlight").Value;

                spriteBatch.Draw(whiteTex, pos, frameBox, Color.White * anmiAlpha, rot, origin, NPC.scale, effects, 0);
            }

            return false;
        }

        #endregion
    }
}
