using System;
using System.IO;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
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

        public int movePhase;
        public bool canDrawShadows;

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("冰龙宝宝");

            Main.npcFrameCount[Type] = 5;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 40;
            NPC.damage = 30;
            NPC.defense = 6;
            NPC.lifeMax = 3400;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 6, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.BossBar = GetInstance<BabyIceDragonBossBar>();

            //BGM：冰结寒流
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/IcyColdStream");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(3820 * bossAdjustment) + numPlayers * 900;
            NPC.damage = 35;
            NPC.defense = 10;

            if (Main.masterMode)
            {
                NPC.lifeMax = (int)(4420 * bossAdjustment) + numPlayers * 1200;
                NPC.damage = 40;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<BabyIceDragonRelic>()));
            npcLoot.Add(ItemDropRule.BossBag(ItemType<BabyIceDragonBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<BabyIceDragonTrophy>(), 10));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleCrystal>(), 1, 3, 5));
            npcLoot.Add(notExpertRule);
        }

        public override void Load()
        {
            //for (int i = 0; i < 5; i++)
            //    GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.BossGores + "Rediancie_Gore" + i);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(CoraliteSoundID.DigIce, NPC.Center);
        }

        public override void OnKill()
        {
            DownedBossSystem.DownBabyIceDragon();
            if (Main.netMode != NetmodeID.MultiplayerClient)
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
            if (State != (int)AIStates.onKillAnim)
            {
                State = (int)AIStates.onKillAnim;
                Timer = 0;
                NPC.dontTakeDamage = true;
                NPC.life = 1;
                return false;
            }

            return true;
        }

        #endregion

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            NPC.oldPos = new Vector2[5];
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
            smashDown = 4,

            //二阶段追加攻击动作
            iceThornsTrap = 5,

            //大师模式专属，特殊攻击动作
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
                NPC.rotation = NPC.rotation.AngleTowards(0f, 0.14f);
                canDrawShadows = false;
                NPC.velocity.X *= 0.98f;
                FlyUp();
                NPC.EncourageDespawn(30);
                return;
            }

            switch ((int)State)
            {
                case (int)AIStates.onKillAnim:      //死亡时的动画
                    NPC.Kill();
                    break;
                case (int)AIStates.onSpawnAnim:      //生成时的动画
                    {
                        do
                        {
                            if ((int)Timer == 0)
                            {
                                //生成动画弹幕
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<BabyIceDragon_OnSpawnAnim>(), 0, 0);
                                NPC.dontTakeDamage = true;
                                NPC.velocity.Y = -0.2f;
                                break;
                            }

                            if (Timer < 100)
                            {
                                ChangeFrameNormally();
                                break;
                            }

                            if ((int)Timer == 100)
                            {
                                NPC.frame.Y = 2;
                                NPC.velocity *= 0;
                            }

                            if (Timer < 130)
                                break;

                            if ((int)Timer == 130)
                            {
                                NPC.frame.Y = 0;
                                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                                PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(0.8f, 0.8f), 5f, 20f, 40, 1000f, "BabyIceDragon");
                                Main.instance.CameraModifiers.Add(modifier);
                            }

                            if (Timer < 170)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                if ((int)Timer % 10 == 0)
                                    Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                                if ((int)Timer % 20 == 0)
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
                    {
                        do
                        {
                            NPC.velocity *= 0.98f;

                            if ((int)Timer == 20)
                            {
                                NPC.frame.Y = 2;
                                NPC.velocity *= 0;
                            }

                            if (Timer < 40)
                                break;

                            if ((int)Timer == 40)
                            {
                                NPC.frame.Y = 0;
                                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);
                                PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(0.8f, 0.8f), 5f, 20f, 40, 1000f, "BabyIceDragon");
                                Main.instance.CameraModifiers.Add(modifier);
                            }

                            if (Timer < 80)
                            {
                                GetMouseCenter(out _, out Vector2 mouseCenter);
                                if (Timer % 10 == 0)
                                    Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Color.White, 0.1f);
                                if (Timer % 20 == 0)
                                    Particle.NewParticle(mouseCenter, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.1f);

                                break;
                            }

                            ResetStates();

                        } while (false);

                        Timer++;
                    }
                    break;
                case (int)AIStates.dizzy:      //原地眩晕
                    {
                        if (Timer < 1)
                            HaveARest(20);

                        NPC.velocity *= 0.96f;
                        Timer--;
                    }
                    break;
                case (int)AIStates.rest:        //休息，原地悬停一会
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
                            break;
                        }

                        Timer--;
                        ChangeFrameNormally();
                    }
                    break;
                case (int)AIStates.dive:      //俯冲攻击，先飞上去再俯冲向玩家，俯冲时如果撞墙会眩晕
                    Dive();
                    break;
                case (int)AIStates.accumulate:      //生成冰块并围绕它飞行，如果冰块被打掉会眩晕
                    {
                        ChangeFrameNormally();
                        do
                        {
                            if (Timer < 50)
                            {
                                NPC.noGravity = true;
                                SetDirection();
                                NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                                float yLength = Math.Abs(Target.Center.Y - NPC.Center.Y);
                                if (yLength > 50)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6f, 0.14f, 0.1f, 0.96f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3f, 0.08f, 0.08f, 0.96f);
                                NPC.rotation = NPC.rotation.AngleTowards(0f, 0.08f);
                                break;
                            }

                            if ((int)Timer == 62)
                            {
                                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                                GetMouseCenter(out _, out Vector2 mouseCenter2);
                                Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 0.8f);
                                Particle.NewParticle(mouseCenter2, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1.2f);
                                for (int i = 0; i < 4; i++)
                                    IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(100, 100), Main.rand.NextVector2CircularEdge(3, 3), 1f, () =>
                                    {
                                        return NPC.Center + (NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2() * 30;
                                    },16);
                            }

                            if (Timer < 80)
                            {
                                NPC.velocity *= 0.92f;
                                break;
                            }

                            //生成冰块弹幕
                            if ((int)Timer == 80 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                movePhase = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + NPC.direction * 120, (int)NPC.Center.Y + 20, NPCType<IceCube>());
                                NPC.netUpdate = true;
                                NPC.noTileCollide = true;
                                NPC.noGravity = true;
                                NPC.netUpdate = true;
                            }

                            Vector2 targetDir = (Main.npc[movePhase].Center - NPC.Center).SafeNormalize(Vector2.One);
                            NPC.velocity = targetDir.RotatedBy(-1.57f) * 4f * NPC.direction;
                            NPC.rotation = NPC.rotation.AngleTowards(NPC.velocity.ToRotation() + (NPC.direction > 0 ? 0 : 3.14f), 0.14f);
                            GetMouseCenter(out _, out Vector2 mouseCenter);
                            Particle.NewParticle(mouseCenter, targetDir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 8f,
                                    CoraliteContent.ParticleType<Fog>(), Color.AliceBlue, Main.rand.NextFloat(0.6f, 0.8f));

                            if ((int)Timer % 20 == 0)
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

                            if (Timer > 900)
                                ResetStates();
                        } while (false);

                        Timer++;
                    }
                    break;
                case (int)AIStates.iceBreath:      //冰吐息
                    IceBreath();
                    break;
                case (int)AIStates.horizontalDash:      //龙车，或者就叫做水平方向冲刺
                    HorizontalDash();
                    break;
                case (int)AIStates.smashDown:      //飞得高点然后下砸，并在周围生成冰刺弹幕
                    SmashDown();
                    break;
                case (int)AIStates.iceThornsTrap:       //冰陷阱，吼叫一声并在目标玩家周围放出冰刺NPC
                    IceThornsTrap();
                    break;
                case (int)AIStates.iceTornado:      //简单准备后冲向玩家并在轨迹上留下冰龙卷风一样的东西
                    IceTornado();
                    break;
                case (int)AIStates.iciclesFall:      //冰雹弹幕攻击，先由下至上吐出一群冰锥，再在玩家头顶随机位置落下冰锥
                    IciclesFall();
                    break;
                default:
                    ResetStates();
                    break;
            }

            if (canDrawShadows)
            {
                for (int i = 0; i < 4; i++)
                    NPC.oldPos[i] = NPC.oldPos[i + 1];

                NPC.oldPos[4] = NPC.Center;
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
                    NPC.velocity.Y *= 0.94f;
                    break;
                case 1:
                case 2:
                    NPC.velocity.Y -= 0.7f;
                    break;
            }

            if (NPC.velocity.Y < -8)
                NPC.velocity.Y = -8;

            ChangeFrameNormally();
        }

        public void GetMouseCenter(out Vector2 targetDir, out Vector2 mouseCenter)
        {
            targetDir = (NPC.rotation + (NPC.direction > 0 ? 0f : 3.141f)).ToRotationVector2();
            mouseCenter = NPC.Center + targetDir * 30;
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
            return NPC.Center + new Vector2(NPC.direction * 18, -20);
        }

        public void InitCaches()
        {
            for (int i = 0; i < 5; i++)
                NPC.oldPos[i] = NPC.Center;
        }

        #region 冰刺

        public void SpawnIceThorns()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(0f, 1f), 20f, 6f, 30, 1000f, "BabyIceDragon");
            Main.instance.CameraModifiers.Add(modifier);

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

        private void TryMakingSpike(ref Point sourceTileCoords, int dir, int howMany, int whichOne, int xOffset, float scaleOffset)
        {
            int position_X = sourceTileCoords.X + xOffset * dir;
            int position_Y = TryMakingSpike_FindBestY(ref sourceTileCoords, position_X);
            if (WorldGen.ActiveAndWalkableTile(position_X, position_Y))
            {
                Vector2 position = new Vector2(position_X * 16 + 8, position_Y * 16 - 8);
                Vector2 velocity = new Vector2(0f, -1f).RotatedBy(whichOne * dir * 0.7f * ((float)Math.PI / 4f / howMany));
                Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ProjectileID.DeerclopsIceSpike, 16, 0f, Main.myPlayer, 0f, 0.4f + scaleOffset + xOffset * 1.1f / howMany);
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
            movePhase = 0;
            canDrawShadows = false;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int phase;

            //大师模式更早进入2阶段
            if (Main.masterMode)
                phase = NPC.life < (int)(NPC.lifeMax * 0.75f) ? 2 : 1;
            else
                phase = NPC.life < (NPC.lifeMax / 2) ? 2 : 1;

            float oldState = State;
            int count = 0;
            while (count < 10)
            {
                count++;
                if (ExchangeState && phase == 2)    //进入2阶段时固定进入吼叫动画
                {
                    State = (int)AIStates.roaringAnim;
                    ExchangeState = false;
                    Main.StartRain();
                    Main.SyncRain();
                    goto Check;
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
                    goto Check;
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
                    goto Check;
                }

                //普通行动
                NormalMove(phase);

            Check:
                if (State == oldState)
                    continue;
                else
                    break;
            }

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            Timer = 0;
            NPC.TargetClosest(false);
            NPC.netUpdate = true;
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

        public void HaveARest(int restTime)
        {
            movePhase = 0;
            canDrawShadows = false;
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

        public static bool CanSpecialMove(int phase)
        {
            if (phase != 2)
                return false;

            return Main.masterMode && Main.rand.NextBool(3);
        }

        public void NormalMove(int phase)
        {
            //这个switch表达式或许让它的可读性变得更加差了......
            State = phase switch
            {
                2 => Main.rand.Next(4) switch  //2阶段
                {
                    0 => (int)AIStates.iceBreath,
                    1 => (int)AIStates.horizontalDash,
                    2 => (int)AIStates.smashDown,
                    _ => (int)AIStates.iceThornsTrap
                },
                _ => Main.rand.Next(3) switch  //一阶段及其他
                {
                    0 => (int)AIStates.iceBreath,
                    1 => (int)AIStates.horizontalDash,
                    _ => (int)AIStates.smashDown
                },
            };

            NormalMoveCount += 1f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
        }

        public bool CanVulnerableMove()
        {
            //大师模式行动每7次进行一次有破绽动作
            if (Main.masterMode)
            {
                if (NormalMoveCount > 6)
                {
                    NormalMoveCount = 0;
                    return true;
                }

                return false;
            }

            //其他模式每5次动作进行一次
            if (NormalMoveCount > 4)
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

            if (canDrawShadows)
            {
                Color color = new Color(43, 255, 198, 100);
                float scale = NPC.scale;
                for (int i = 3; i > -1; i--)
                {
                    spriteBatch.Draw(mainTex, NPC.oldPos[i] - screenPos, frameBox, color, NPC.rotation, origin, scale, effects, 0f);
                    color *= 0.5f;
                    scale *= 0.98f;
                }
            }

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
