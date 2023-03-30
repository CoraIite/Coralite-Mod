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
            SoundEngine.PlaySound(CoraliteSoundID.DigIce, NPC.Center);
        }

        public override bool? CanFallThroughPlatforms()
        {
            return State != (int)AIStates.smashDown;
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
                                PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(0.8f, 0.8f), 5f, 6f, 60, 1000f, "BabyIceDragon");
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

                            NPC.velocity.X *= 0.99f;
                            switch (NPC.frame.Y)
                            {
                                default:
                                case 0:
                                case 3:
                                case 4:
                                    NPC.velocity.Y *= 0.96f;
                                    break;
                                case 1:
                                case 2:
                                    NPC.velocity.Y -= 0.3f;
                                    break;
                            }

                            if (NPC.velocity.Y < -12)
                                NPC.velocity.Y = -12;

                            ChangeFrameNormally();

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

                    NPC.velocity *= 0.96f;
                    Timer--;
                    
                    break;
                case (int)AIStates.rest:        //休息，原地悬停一会
                    {
                        NPC.velocity.X *= 0.96f;
                        NPC.rotation = NPC.rotation.AngleTowards(0f, 0.08f);
                        NPC.directionY =( Target.Center.Y - 200 )> NPC.Center.Y ? 1 : -1;
                        float yLength = Math.Abs(Target.Center.Y-200 - NPC.Center.Y);
                        if (yLength > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.14f, 0.1f, 0.96f);
                        else
                            NPC.velocity.Y *= 0.96f;


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
                case (int)AIStates.dive:      //俯冲攻击，先飞上去再俯冲向玩家，俯冲时如果撞墙会眩晕
                    Dive();
                    break;
                case (int)AIStates.accumulate:      //生成冰块并围绕它飞行，如果冰块被打掉会眩晕
                    {
                        ChangeFrameNormally();
                        do
                        {
                            if (Timer < 80)
                            {
                                NPC.noGravity = true;
                                SetDirection();
                                NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                                float yLength = Math.Abs(Target.Center.Y  - NPC.Center.Y);
                                if (yLength > 50)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.14f, 0.1f, 0.96f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3f, 0.08f, 0.08f, 0.96f);
                                NPC.rotation = NPC.rotation.AngleTowards(0f, 0.08f);
                                break;
                            }

                            if (Timer == 92)
                            {
                                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                                GetMouseCenter(out _, out Vector2 mouseCenter2);
                                Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 0.8f);
                                Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1.2f);
                                for (int i = 0; i < 4; i++)
                                    IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100), Main.rand.NextVector2CircularEdge(3, 3), 1f, () =>
                                    {
                                        return NPC.Center + (NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * 30;
                                    });
                            }

                            if (Timer < 110)
                            {
                                NPC.velocity *= 0.92f;
                                break;
                            }

                            //生成冰块弹幕
                            if (Timer == 110)
                            {
                                movePhase = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + NPC.direction * 120, (int)NPC.Center.Y + 20, NPCType<IceCube>());
                                NPC.netUpdate = true;
                                NPC.noTileCollide = true;
                                NPC.noGravity = true;
                            }

                            Vector2 targetDir = (Main.npc[movePhase].Center - NPC.Center).SafeNormalize(Vector2.One);
                            NPC.velocity = targetDir.RotatedBy(-1.57f) * 4f * NPC.direction;
                            NPC.rotation = NPC.rotation.AngleTowards(NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0 : 3.14f), 0.14f);
                            GetMouseCenter(out _, out Vector2 mouseCenter);
                            Particle.NewParticle(mouseCenter, targetDir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 8f,
                                    CoraliteContent.ParticleType<IceFog>(), Color.AliceBlue, Main.rand.NextFloat(0.6f, 0.8f));

                            if (Timer % 20 == 0)
                            {
                                //控制冰块弹幕，让它变大
                                int npcIndex = Helper.GetNPCByType(NPCType<IceCube>());
                                if (npcIndex == -1)
                                {
                                    ResetStates();
                                    break;
                                }

                                Main.npc[npcIndex].ai[0] = 1;
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
                            SetDirection();
                            NPC.directionY = (Target.Center.Y - 200) > NPC.Center.Y ? 1 : -1;
                            float yLength = Math.Abs(Target.Center.Y - 200 - NPC.Center.Y);
                            if (yLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 4f, 0.14f, 0.1f, 0.96f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Math.Abs(Target.Center.X - NPC.Center.X) > 160)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 6f, 0.14f, 0.08f, 0.96f);
                            else
                                NPC.velocity.X *= 0.98f;
                            ChangeFrameNormally();

                            if (Timer == 120)
                            {
                                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                                GetMouseCenter(out _, out Vector2 mouseCenter2);
                                for (int i = 0; i < 4; i++)
                                    IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100), Main.rand.NextVector2CircularEdge(3, 3), 1f, () =>
                                    {
                                        return NPC.Center + (NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * 30;
                                    });
                                Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 0.8f);
                                Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1.2f);
                            }

                            if (Timer < 140)
                                break;

                            if (Timer < 161)
                            {
                                //生成冰吐息弹幕
                                if (Timer % 5 == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
                                    Vector2 targetDir = (Target.Center + Main.rand.NextVector2CircularEdge(30, 30) - NPC.Center).SafeNormalize(Vector2.Zero);
                                    GetMouseCenter(out _, out Vector2 mouseCenter);
                                    for (int i = -1; i < 1; i++)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), mouseCenter, targetDir.RotatedBy(i * 0.05f) * 10f, ProjectileType<IceBreath>(), 30, 5f);
                                }
                                break;
                            }

                            int restTime = Main.masterMode ? 20 : 40;
                            HaveARest(restTime);
                            return;

                        } while (false);

                        Timer++;
                    }
                    break;
                case (int)AIStates.horizontalDash:      //龙车，或者就叫做水平方向冲刺
                    HorizontalDash();
                    break;
                case (int)AIStates.smashDown:      //飞得高点然后下砸，并在周围生成冰刺弹幕
                    SmashDown();
                    break;
                case (int)AIStates.iceThornsTrap:       //冰陷阱，吼叫一声并在目标玩家周围放出冰刺NPC
                    ResetStates();
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
            //只有扇翅膀的时候才会有向上加速度，否则减速
            switch (NPC.frame.Y)
            {
                default:
                case 0:
                case 3:
                case 4:
                    NPC.velocity.Y *= 0.96f;
                    break;
                case 1:
                case 2:
                    NPC.velocity.Y -= 0.4f;
                    break;
            }

            if (NPC.velocity.Y < -12)
                NPC.velocity.Y = -12;

            ChangeFrameNormally();
        }

        public void GetMouseCenter(out Vector2 targetDir, out Vector2 mouseCenter)
        {
            targetDir = (NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2();
            mouseCenter = NPC.Center + targetDir * 30;
        }

        #region 冰刺

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

        #endregion

        public void ResetStates()
        {
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
                    NormalMoveCount = 0;
                    State = Main.rand.Next(2) switch
                    {
                        0 => (int)AIStates.dive,
                        _ => (int)AIStates.accumulate
                    };

                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    break;
                }

                //特殊行动，大师模式专属
                if (CanSpecialMove(phase))
                {
                    NormalMoveCount += 1f;
                    State = Main.rand.Next(2) switch
                    {
                        0 => (int)AIStates.iceTornado,
                        _ => (int)AIStates.iciclesFall
                    };
                    break;
                }

                //普通行动
                NormalMove(phase);

            } while (false);

            //State = (int)AIStates.accumulate;
            Timer = 0;
            NPC.TargetClosest(false);
            NPC.netUpdate = true;
        }

        public void Dizzy(int dizzyTime)
        {
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

            if (NPC.target != Main.myPlayer)
                return;

            State = (int)AIStates.dizzy;

            NPC.velocity = new Vector2(-NPC.direction * 4f, -4f);
            NPC.rotation = 0f;
            NPC.frameCounter = 0;
            NPC.frame.Y = 2;

            Timer = dizzyTime;
            NormalMoveCount = 0;        //归零这个计数（虽说感觉可能没什么用的样子）
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.netUpdate = true;
        }

        public Vector2 GetDizzyStarCenter()
        {
            return NPC.Center + new Vector2(NPC.direction * 18, -20);
        }

        public void HaveARest(int restTime)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            State = (int)AIStates.rest;

            Timer = restTime;
            NPC.TargetClosest();
            SetDirection();
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netUpdate = true;
        }

        public void SetDirection()
        {
            if (Math.Abs(NPC.Center.X - Target.Center.X) < 16)
                return;

            NPC.direction = NPC.Center.X > Target.Center.X ? -1 : 1;
            NPC.spriteDirection = NPC.direction;
        }

        public bool CanSpecialMove(int phase)
        {
            if (phase != 2)
                return false;

            return Main.masterMode && Main.rand.NextBool(3);
        }

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
                    0 => (int)AIStates.iceBreath,
                    1 => (int)AIStates.horizontalDash,
                    2 => (int)AIStates.smashDown,
                    _ => (int)AIStates.iceThornsTrap
                },
                //一阶段及其他
                _ => Main.rand.Next(2) switch
                {
                    0 => (int)AIStates.iceBreath,
                    _ => (int)AIStates.horizontalDash
                },
            };

            NormalMoveCount += 1f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public bool CanVulnerableMove()
        {
            //大师模式行动每5次进行一次有破绽动作
            if (Main.masterMode && NormalMoveCount > 4)
            {
                NormalMoveCount = 0;
                return true;
            }

            //其他模式每3次动作进行一次
            if (NormalMoveCount > 2)
            {
                NormalMoveCount = 0;
                return true;
            }

            return false;
        }

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

        #region NetWork
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
