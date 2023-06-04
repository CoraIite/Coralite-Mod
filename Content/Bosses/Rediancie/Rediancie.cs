using System.Collections.Generic;
using Coralite.Content.Dusts;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.Rediancie
{
    //简单写下Rediancie这个英文名由来
    //其实就是Red+Diancie ,红色+蒂安希，捏他自宝可梦中的钻石公主蒂安希
    //另外它的召唤物，小赤玉灵名字捏他的是小碎钻。
    //
    //                                            饿啊  ， 吃我钻石风暴！！！
    //
    //                      💎💎                                       💎💎                                      💎💎
    //                 💎💎💎💎                             💎💎💎💎                            💎💎💎💎
    //           💎💎💎💎💎💎                   💎💎💎💎💎💎                  💎💎💎💎💎💎
    //      💎💎💎💎💎💎💎              💎💎💎💎💎💎💎             💎💎💎💎💎💎💎
    // 💎💎💎💎💎💎💎💎         💎💎💎💎💎💎💎💎        💎💎💎💎💎💎💎💎
    //      💎💎💎💎💎💎💎              💎💎💎💎💎💎💎             💎💎💎💎💎💎💎
    //           💎💎💎💎💎💎                   💎💎💎💎💎💎                  💎💎💎💎💎💎
    //                 💎💎💎💎                             💎💎💎💎                            💎💎💎💎
    //                      💎💎                                       💎💎                                      💎💎
    //
    //MEGA蒂安希 160物攻 本系100威力群攻技能，你接的下?
    //你十万条命都接不下
    //打完还50%概率上升2段物防，又硬又能打（虽然mega后物防还低了）
    [AutoloadBossHead]
    public class Rediancie : ModNPC
    {
        public override string Texture => AssetDirectory.Rediancie + Name;

        Player Target => Main.player[NPC.target];

        public bool ExchangeState = true;

        internal ref float DamageCount => ref NPC.ai[0];
        internal ref float State => ref NPC.ai[1];
        /// <summary> 招式循环的方式，具体使用MoveCycling查看 </summary>
        internal ref float MoveCyclingType => ref NPC.ai[2];

        /// <summary> 拥有的“弹药”数量 </summary>
        internal ref float OwnedFollowersCount => ref NPC.ai[3];

        internal float Timer;
        /// <summary> 目前的AI循环的计数 </summary>
        internal ref float MoveCount => ref NPC.localAI[1];

        internal readonly Color red = new Color(221, 50, 50);
        internal readonly Color grey = new Color(91, 93, 102);
        public const int ON_KILL_ANIM_TIME = 250;

        public List<RediancieFollower> followers;

        #region tml hooks

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("赤玉灵");

            //Main.npcFrameCount[Type] = 6;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 85;
            NPC.damage = 25;
            NPC.defense = 6;
            NPC.lifeMax = 1500;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.BossBar = GetInstance<RediancieBossBar>();

            //BGM：赤色激流
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/RedTorrent");

        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(1800 * bossAdjustment) + numPlayers * 450;
            NPC.damage = 30;
            NPC.defense = 6;

            if (Main.masterMode)
            {
                NPC.lifeMax = (int)(2000 * bossAdjustment) + numPlayers * 550;
                NPC.damage = 45;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = (int)(2300 * bossAdjustment) + numPlayers * 600;
                NPC.defense = 4;//因为FTW种能够拥有非常多的弹药所以就降低一下基础防御了
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<RediancieRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<RedianciePet>(), 4));
            npcLoot.Add(ItemDropRule.BossBag(ItemType<RediancieBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));

            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<RedJade>(), 1, 20, 24));
            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int j = 0; j < 3; j++)
                {
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore2").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore3").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore4").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)), Mod.Find<ModGore>("Rediancie_Gore0").Type);
                }

                Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)), Mod.Find<ModGore>("Rediancie_Gore1").Type);
            }

            followers.ForEach(fo => fo = null);
            followers = null;
            DownedBossSystem.DownRediancie();
        }

        public override void Load()
        {
            RediancieFollower.tex1 = Request<Texture2D>(AssetDirectory.Rediancie + "RediancieFollower1");
            RediancieFollower.tex2 = Request<Texture2D>(AssetDirectory.Rediancie + "RediancieFollower2");

            for (int i = 0; i < 5; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.BossGores + "Rediancie_Gore" + i);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
                SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            OnHit(hit.Damage);
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            OnHit(hit.Damage);
        }

        public void OnHit(int damage)
        {
            DamageCount += damage;
            int value = Helper.ScaleValueForDiffMode(50, 60, 75, 100);
            while (DamageCount > value)
            {
                DamageCount -= value;
                DespawnFollowers(1);
            }

            NPC.netUpdate = true;
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
            followers = new List<RediancieFollower>();
            SpawnFollowers(3);

            NPC.TargetClosest(false);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                State = (int)AIStates.onSpawnAnim;
                NPC.Center = Target.Center - new Vector2(0, 600);
                NPC.netUpdate = true;
            }
        }

        public enum AIStates : int
        {
            onKillAnim = -5,
            onSpawnAnim = -4,
            /// <summary> 赤色脉冲 </summary>
            pulse = -3,
            /// <summary> 赤玉烟花 </summary>
            firework = -2,
            /// <summary> 蓄力大爆炸 </summary>
            accumulate = -1,
            /// <summary> 赤色爆冲 </summary>
            dash = 0,
            /// <summary> 3连炸 </summary>
            explosion = 1,
            /// <summary> 赤玉雨 </summary>
            upShoot = 2,
            /// <summary> 赤玉激光 </summary>
            magicShoot = 3,
            /// <summary> 召唤小赤玉灵 </summary>
            summon = 4
        }

        public enum CyclingType : int
        {
            /// <summary> 一次近战一次远程循环 </summary>
            one_one,
            /// <summary> 两次近战一次远程循环 </summary>
            two_one,
            /// <summary> 两次近战两次远程循环 </summary>
            two_two,
        }

        public override void AI()
        {
            //#region frame
            //NPC.frameCounter++;
            //int frameTime = State == -3 ? 8 : 4;

            //if (NPC.frameCounter > frameTime)
            //{
            //    NPC.frame.Y += 1;
            //    if (NPC.frame.Y == Main.npcFrameCount[Type])
            //        NPC.frame.Y = 0;
            //    NPC.frameCounter = 0;
            //}
            //#endregion
            
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)
                NPC.TargetClosest();

            if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)//没有玩家存活时离开
            {
                State = -0.1f;
                NPC.velocity.X *= 0.97f;
                NPC.velocity.Y += 0.04f;
                NPC.EncourageDespawn(10);
                ChangeRotationNormally();
                UpdateFollower_Idle();
                return;
            }

            if (OwnedFollowersCount != followers.Count)
                RespawnFollowers();

            float distanceX = Target.Center.X - NPC.Center.X;
            NPC.direction = distanceX > 0 ? 1 : -1;
            //NPC.spriteDirection = (Math.Abs(distanceX) > 24) ? NPC.direction : 1;
            NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
            switch (State)
            {
                case (int)AIStates.onKillAnim:
                    {
                        SlowDownAndGoUp(0.96f, -0.05f, -0.5f);
                        ChangeRotationNormally();
                        UpdateFollower_Summon();

                        if (Timer < 30)
                            break;

                        if (Timer > 40 && Timer % 20 == 0 && Main.netMode != NetmodeID.Server)
                        {
                            Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, 1).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore2").Type);
                            Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, 1).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore3").Type);
                            Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore4").Type);
                        }

                        if (Timer < 230 && Timer % 15 == 0 && Main.netMode != NetmodeID.Server)
                        {
                            Helper.PlayPitched("RedJade/RedJadeBoom", 0.4f, 0f, NPC.Center);

                            Vector2 center = NPC.Center + Main.rand.NextVector2Circular(30, 40);
                            for (int i = 0; i < 8; i++)
                            {
                                Dust.NewDustPerfect(center, DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(6, 6), 0, red, Main.rand.NextFloat(1f, 1.5f));
                                Dust.NewDustPerfect(center, DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(4, 4), 0, grey, Main.rand.NextFloat(0.8f, 1.2f));
                            }
                        }

                        if (Timer == 245)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<Rediancie_BigBoom>(), 55, 8f);
                            if (Main.netMode != NetmodeID.Server)
                            {
                                var modifier = new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1, 1), 10, 6f, 20, 1000f);
                                Main.instance.CameraModifiers.Add(modifier);
                            }
                        }

                        if (Timer > ON_KILL_ANIM_TIME)
                            NPC.Kill();
                    }
                    break;
                case (int)AIStates.onSpawnAnim:         //生成时的动画
                    {
                        if (Timer == 0) //生成动画弹幕
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<Rediancie_OnSpawnAnim>(), 0, 0);
                            NPC.velocity = new Vector2(0, 1.5f);
                            NPC.dontTakeDamage = true;
                        }

                        if (Timer % 5 == 0 && Main.netMode != NetmodeID.Server)
                        {
                            int count = (int)Timer / 25;
                            for (int i = 0; i < count; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 3, count * 3), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.2f);
                                dust.noGravity = true;
                            }
                        }

                        if (Timer > 120)
                            NPC.velocity *= 0.998f;

                        if (Timer == 260)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<Rediancie_BigBoom>(), 55, 8f);
                            if (Main.netMode != NetmodeID.Server)
                            {
                                var modifier = new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1, 1), 10, 6f, 20, 1000f);
                                Main.instance.CameraModifiers.Add(modifier);
                            }
                        }

                        if (Timer == 270 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            State = (int)AIStates.explosion;
                            Timer = 0;
                            NPC.TargetClosest();
                            NPC.dontTakeDamage = false;
                            NPC.netUpdate = true;
                        }

                        ChangeRotationNormally();
                        UpdateFollower_Idle();
                    }
                    break;
                case (int)AIStates.pulse:               //蓄力射出大型弹幕
                    {
                        float yLength = NPC.Center.Y - Target.Center.Y;
                        if (yLength > -150)
                            SlowDownAndGoUp(0.98f, -0.14f, -1.5f);
                        else
                            NPC.velocity *= 0.99f;

                        Vector2 targetVec = Target.Center - NPC.Center;
                        float factor = Math.Clamp(targetVec.Length() / 150f, 0f, 1f);
                        Vector2 targetDir = targetVec.SafeNormalize(Vector2.One);
                        Vector2 targetCenter = NPC.Center + targetDir * (32 + factor * 32);
                        int realTime = (int)Timer - 60;

                        ChangeRotationNormally();

                        if (Timer < 125)
                        {
                            UpdateFollower_Pulse(targetCenter, targetDir, factor, -1, 0.1f + 0.5f * Timer / 60);
                            for (int i = 0; i < 2; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(targetCenter + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, -targetDir * 6f, 0, default, 1.1f);
                                dust.noGravity = true;
                            }
                            break;
                        }
                        else if (Timer < 255)
                        {
                            UpdateFollower_Pulse(targetCenter, targetDir, factor, realTime % 65);

                            Dust dust = Dust.NewDustPerfect(targetCenter + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, -targetDir * 6f, 0, default, 1.1f + 2f * (realTime % 65) / 65f);
                            dust.noGravity = true;
                        }
                        else
                            UpdateFollower_Idle(0.08f);

                        if (realTime % 65 == 0)//生成弹幕
                        {
                            if (!CanDespawnFollower())
                            {
                                ResetState();
                                break;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int damage = NPC.GetAttackDamage_ForProjectiles(10, 15);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), targetCenter, 
                                    (Target.Center - NPC.Center + Main.rand.NextVector2CircularEdge(48,48)).SafeNormalize(Vector2.UnitY) * 12f, 
                                    ProjectileType<RedPulse>(), damage, 5f,NPC.target);
                            }

                            Helper.PlayPitched("RedJade/RedJadeBeam", 0.13f, 0f, NPC.Center);
                            if (!DespawnFollowers(1))
                                ResetState();
                        }

                        if (Timer > 275)
                            ResetState();
                    }
                    break;
                case (int)AIStates.firework:            //向四周生成弹幕，类似烟花一样
                    {
                        //控制X方向的移动
                        Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 2f, 0.1f, 0.1f, 0.97f);

                        //控制Y方向的移动
                        float yLength2 = Math.Abs(Target.Center.Y - NPC.Center.Y);
                        if (yLength2 > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 1f, 0.06f, 0.06f, 0.97f);
                        else
                            NPC.velocity.Y *= 0.96f;

                        if (Timer == 2)
                        {
                            NPC.reflectsProjectiles = true;
                            NPC.dontTakeDamage = true;
                            RedShield.Spawn(NPC, 250);
                            SpawnFollowers(Main.getGoodWorld ? 6 : 3);
                        }

                        ChangeRotationNormally();
                        if (OwnedFollowersCount == 0||followers.Count==0)
                        {
                            ResetState();
                            break;
                        }

                        UpdateFollower_Firework();

                        if (Timer < 49)
                            break;

                        if (Timer % 25 == 0&&Main.netMode!=NetmodeID.MultiplayerClient)
                        {
                            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
                            int damage = NPC.GetAttackDamage_ForProjectiles(10, 15);
                            int timeleft = 16;
                            int howMany = Main.getGoodWorld ? 4 : 3;    //FTW能射出4个，其他模式只射3个
                            for (int i = 0; i < howMany; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), followers[^1].center, rot.ToRotationVector2() *12, ProjectileType<RedFirework>(), damage, 5f, NPC.target, 0, timeleft + i * 10);
                                rot += MathHelper.TwoPi / howMany;
                            }

                            SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                            if (!DespawnFollowers(1))
                            {
                                ResetState();
                                RedShield.Kill();
                            }
                        }

                        if (Timer > 265)
                            ResetState();
                    }
                    break;
                case (int)AIStates.accumulate:          //追逐玩家并蓄力爆炸
                    {
                        //控制X方向的移动
                        if (Timer < 325)
                        {
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 7.5f, 0.12f, 0.15f, 0.97f);

                            //控制Y方向的移动
                            float yLength = Math.Abs(Target.Center.Y - NPC.Center.Y);
                            if (yLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 4.5f, 0.06f, 0.08f, 0.97f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Timer % 5 == 0 && Main.netMode != NetmodeID.Server)
                            {
                                int count = (int)Timer / 25;
                                for (int i = 0; i < count; i++)
                                {
                                    Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 3, count * 3), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.2f);
                                    dust.noGravity = true;
                                }
                            }
                        }
                        else
                            NPC.velocity *= 0.995f;

                        if (Timer == 330)       //生成弹幕
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ProjectileType<Rediancie_BigBoom>(), 55, 8f);
                            if (Main.netMode != NetmodeID.Server)
                            {
                                var modifier = new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1, 1), 10, 6f, 20, 1000f);
                                Main.instance.CameraModifiers.Add(modifier);
                            }
                            SpawnFollowers(6);
                        }

                        if (Timer > 340)
                            ResetState();

                        ChangeRotationNormally();
                        UpdateFollower_Idle();
                    }
                    break;
                case (int)AIStates.dash:            //连续3次冲刺攻击
                    {
                        int realTime = (int)Timer % 100;

                        do
                        {
                            if (realTime < 18 && realTime % 3 == 0)
                            {
                                Dust.NewDustPerfect(NPC.Center, DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(6, 6), 0, red, Main.rand.NextFloat(1f, 1.5f));
                                Dust.NewDustPerfect(NPC.Center, DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(4, 4), 0, grey, Main.rand.NextFloat(0.8f, 1.2f));
                            }
                            if (realTime == 18 && Main.netMode != NetmodeID.Server)
                            {
                                SoundEngine.PlaySound(SoundID.Item4, NPC.Center);
                                Particle.NewParticle(NPC.Center + new Vector2(0, -16), Vector2.Zero, CoraliteContent.ParticleType<Flash_WithOutLine>(), Coralite.Instance.RedJadeRed, 1.5f);
                            }

                            if (realTime < 20)
                            {
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 2f, 0.1f, 0.1f, 0.97f);
                                float yLength = Math.Abs(Target.Center.Y - NPC.Center.Y);

                                if (yLength > 50)//控制Y方向的移动
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 2f, 0.1f, 0.1f, 0.97f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                break;
                            }

                            if (realTime == 22)//开始冲刺
                            {
                                SpawnFollowers(2);
                                NPC.velocity = (Target.Center + new Vector2(0, (int)Timer / 100 % 2 == 0 ? 100 : -100) - NPC.Center).SafeNormalize(Vector2.One) * 10f;
                                NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            }

                            if (realTime < 71)//边冲边炸
                            {
                                if (realTime % 10 == 0)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ProjectileType<Rediancie_Explosion>(), 20, 5f);

                                break;
                            }

                            float targetRot = NPC.velocity.Length() * 0.04f * NPC.direction;
                            NPC.rotation = NPC.rotation.AngleLerp(targetRot, 0.08f);

                            NPC.velocity *= 0.98f;
                        } while (false);

                        if (Timer > 300)
                            ResetState();

                        UpdateFollower_Idle();
                    }
                    break;
                default:
                case (int)AIStates.explosion:       //追着玩家并爆炸
                    {
                        //控制X方向的移动
                        Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 6.5f, 0.12f, 0.22f, 0.97f);

                        //控制Y方向的移动
                        float yLength2 = Math.Abs(Target.Center.Y - NPC.Center.Y);
                        if (yLength2 > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 4.5f, 0.06f, 0.06f, 0.97f);
                        else
                            NPC.velocity.Y *= 0.96f;

                        if (Timer % 3 == 0 && Main.netMode != NetmodeID.Server)
                        {
                            int count = ((int)Timer % 80) / 10;
                            for (int i = 0; i < count; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 3, count * 3), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.1f);
                                dust.noGravity = true;
                            }
                        }

                        if (Timer % 80 == 0)//生成弹幕
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ProjectileType<Rediancie_Explosion>(), 30, 5f);
                            SpawnFollowers(1);
                        }
                        if (Timer > 250)
                            ResetState();

                        ChangeRotationNormally();
                        UpdateFollower_Idle();
                    }
                    break;
                case (int)AIStates.upShoot:         //朝上连续射击
                    {
                        float yLength = NPC.Center.Y - Target.Center.Y;
                        if (yLength > -150)
                            SlowDownAndGoUp(0.98f, -0.14f, -1.5f);
                        else
                            NPC.velocity *= 0.99f;

                        ChangeRotationNormally();

                        if (OwnedFollowersCount == 0|| followers.Count == 0)
                        {
                            ResetState();
                            break;
                        }

                        UpdateFollower_UpShoot();

                        if (Timer < 30)
                            break;

                        if (Timer % 40 == 0)//隔固定时间射弹幕
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int index = Main.rand.Next(followers.Count);
                                int damage = NPC.GetAttackDamage_ForProjectiles(10, 15);
                                int shootCount = Helper.ScaleValueForDiffMode(2, 2, 3, 4);
                                for (int i = 0; i < shootCount; i++)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), followers[index].center, new Vector2(0, -8).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ProjectileType<Rediancie_Strike>(), damage, 5f, NPC.target);
                            }

                            SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                            if (!DespawnFollowers(1))
                                ResetState();
                        }

                        if (Timer > 260)
                            ResetState();
                    }
                    break;
                case (int)AIStates.magicShoot:      //蓄力射3发魔法弹幕
                    {
                        float yLength = NPC.Center.Y - Target.Center.Y;
                        if (yLength > -150)
                            SlowDownAndGoUp(0.98f, -0.14f, -1.5f);
                        else
                            NPC.velocity *= 0.99f;

                        Vector2 targetVec = Target.Center - NPC.Center;
                        float factor = Math.Clamp(targetVec.Length() / 150f, 0f, 1f);
                        Vector2 targetDir = targetVec.SafeNormalize(Vector2.One);
                        Vector2 targetCenter = NPC.Center + targetDir * (32 + factor * 32);

                        if (Timer % 3 == 0)
                            for (int i = 0; i < 6; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(targetCenter + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, Vector2.Zero, 0, default, 1.3f);
                                dust.velocity = (NPC.Center - dust.position).SafeNormalize(Vector2.UnitY) * 3f;
                                dust.noGravity = true;
                            }

                        ChangeRotationNormally();

                        if (Timer < 60)     //其实这里写70也没问题，只是为了避免不必要的麻烦所以填的小一点
                        {
                            UpdateFollower_MagicShoot(targetCenter, targetDir, factor, 0.1f + 0.5f * Timer / 60);
                            break;
                        }
                        else if (Timer < 140)
                            UpdateFollower_MagicShoot(targetCenter, targetDir, factor);
                        else
                            UpdateFollower_Idle(0.1f);


                        if (Timer % 35 == 0)//生成弹幕
                        {
                            if (!CanDespawnFollower())
                            {
                                ResetState();
                                break;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int damage = NPC.GetAttackDamage_ForProjectiles(15, 20);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), targetCenter, (Target.Center - NPC.Center + new Vector2(0, 60 * (Timer / 30) == 1 ? 1 : -1)).SafeNormalize(Vector2.UnitY) * 10f, ProjectileType<Rediancie_Beam>(), damage, 5f);
                            }

                            Helper.PlayPitched("RedJade/RedJadeBeam", 0.13f, 0f, NPC.Center);
                            if (!DespawnFollowers(1))
                                ResetState();
                        }

                        if (Timer > 155)
                            ResetState();
                    }
                    break;
                case (int)AIStates.summon:          //召唤小赤玉灵
                    {
                        float yLength = NPC.Center.Y - Target.Center.Y;
                        if (yLength > -150)
                            SlowDownAndGoUp(0.98f, -0.14f, -1.5f);
                        else
                            NPC.velocity *= 0.99f;

                        ChangeRotationNormally();

                        if (OwnedFollowersCount == 0|| followers.Count == 0)
                        {
                            ResetState();
                            break;
                        }

                        UpdateFollower_Summon();

                        if (Timer % 10 == 0)
                            for (int i = 0; i < 6; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(followers[^1].center + Main.rand.NextVector2Circular(20, 20), DustID.GemRuby, Vector2.Zero, 0, default, 1.3f);
                                dust.noGravity = true;
                            }

                        if (Timer % 40 == 0)
                        {
                            if (!CanDespawnFollower())
                            {
                                ResetState();
                                break;
                            }

                            //为了保证同场召唤物数量不会过多所以还是保留了这一段
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (Main.npc.Count((n) => n.active && n.type == NPCType<RediancieMinion>()) < Helper.ScaleValueForDiffMode(2, 3, 3, 4))
                                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)followers[^1].center.X, (int)followers[^1].center.Y, NPCType<RediancieMinion>());
                                else
                                {
                                    ResetState();
                                    break;
                                }
                            }

                            SoundEngine.PlaySound(CoraliteSoundID.MagicStaff_Item8, NPC.Center);
                            if (!DespawnFollowers(1))
                                ResetState();
                        }

                        if (Timer > 200)//防止出BUG
                            ResetState();
                    }
                    break;
            }

            Timer++;
        }

        public void ResetState()
        {
            NPC.reflectsProjectiles = false;
            NPC.dontTakeDamage = false;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int phese = 1;
            if (NPC.life < NPC.lifeMax / 2)
                phese = 2;

            GetAICycling((CyclingType)MoveCyclingType, out int meleeCount, out int ShootCount);
            bool useMelee = MoveCount < meleeCount;
            bool useShoot = MoveCount < meleeCount + ShootCount;

            switch (phese)
            {
                default:
                case 1:
                    if (Main.masterMode || Main.getGoodWorld)   //大师模式及以上
                    {
                        if (useMelee)   //近战
                        {
                            State = Main.rand.Next(2) switch
                            {
                                0 => (int)AIStates.accumulate,
                                _ => (int)AIStates.explosion
                            };
                            break;
                        }

                        if (useShoot)   //远程
                            State = Main.rand.Next(4) switch
                            {
                                0 => (int)AIStates.upShoot,
                                _ => (int)AIStates.magicShoot
                            };
                    }
                    else        //其他模式
                    {
                        if (useMelee)   //近战，只会普通三连炸
                        {
                            State = (int)AIStates.explosion;
                            break;
                        }

                        if (useShoot)   //远程
                            State = Main.rand.Next(3) switch
                            {
                                0 => (int)AIStates.upShoot,
                                _ => (int)AIStates.magicShoot
                            };
                    }
                    break;
                case 2:     //二阶段
                    if (ExchangeState)    //血量低于一半固定放小弟
                    {
                        State = (int)AIStates.summon;
                        ExchangeState = false;
                        break;
                    }

                    if (Main.masterMode || Main.getGoodWorld)   //大师模式及以上
                    {
                        if (useMelee)   //近战
                        {
                            State = Main.rand.Next(3) switch
                            {
                                0 => (int)AIStates.accumulate,
                                1 => (int)AIStates.explosion,
                                _ => (int)AIStates.dash
                            };
                            break;
                        }

                        if (useShoot)   //远程
                            State = Main.rand.Next(4) switch
                            {
                                0 => (int)AIStates.upShoot,
                                1 => (int)AIStates.firework,
                                2 => (int)AIStates.pulse,
                                _ => (int)AIStates.summon
                            };
                    }
                    else        //其他模式
                    {
                        if (useMelee)   //近战，只会普通三连炸
                        {
                            State = (int)AIStates.explosion;
                            break;
                        }

                        if (useShoot)   //远程
                            State = Main.rand.Next(3) switch
                            {
                                0 => (int)AIStates.upShoot,
                                1 => (int)AIStates.magicShoot,
                                _ => (int)AIStates.summon
                            };
                    }
                    break;
            }

        MoveCount += 1;
            if (MoveCount >= meleeCount + ShootCount)   //如果一轮全部执行完成那么就在次随机一下循环方式
            {
                MoveCount = 0;
                MoveCyclingType = Main.rand.Next(3) switch
                {
                    0 => (int)CyclingType.one_one,
                    1 => (int)CyclingType.two_one,
                    _ => (int)CyclingType.two_two,
                };
            }

            Timer = 0;
            NPC.TargetClosest();
            NPC.netUpdate = true;
        }


        /// <summary>
        /// 获取一次攻击动作循环种AI的近战类与远程类的具体个数
        /// </summary>
        /// <param name="cyclingType"></param>
        /// <param name="meleeMoveCount"></param>
        /// <param name="ShootMoveCount"></param>
        public void GetAICycling(CyclingType cyclingType, out int meleeMoveCount, out int ShootMoveCount)
        {
            switch (cyclingType)
            {
                default:
                case CyclingType.one_one:
                    meleeMoveCount = 1;
                    ShootMoveCount = 1;
                    break;
                case CyclingType.two_one:
                    meleeMoveCount = 2;
                    ShootMoveCount = 1;
                    break;
                case CyclingType.two_two:
                    meleeMoveCount = 2;
                    ShootMoveCount = 2;
                    break;
            }
        }

        #region HelperMethods

        public void SlowDownAndGoUp(float slowDownX, float accelY, float velocityLimitY)
        {
            NPC.velocity.X *= slowDownX;
            NPC.velocity.Y += accelY;
            if (NPC.velocity.Y < velocityLimitY)
                NPC.velocity.Y = velocityLimitY;
        }

        /// <summary> 最普通的改变旋转角度的方式 </summary>
        public void ChangeRotationNormally()
        {
            float targetRot = NPC.velocity.Length() * 0.04f * NPC.direction;
            NPC.rotation = NPC.rotation.AngleTowards(targetRot, 0.01f);
        }
        #endregion

        #endregion

        #region Followers

        /// <summary>
        /// 获得弹药
        /// </summary>
        /// <param name="howMany">获得弹药的数量</param>
        public void SpawnFollowers(int howMany)
        {
            int maxFollowers = Helper.ScaleValueForDiffMode(6, 9, 12, 18);
            if (OwnedFollowersCount >= maxFollowers)    ///弹药数已经达到上限
                return;

            if (OwnedFollowersCount + howMany > maxFollowers)   ///弹药数加上获得的弹药超出弹药上限时
            {
                int count = maxFollowers - (int)OwnedFollowersCount;
                OwnedFollowersCount = maxFollowers;
                for (int i = 0; i < count; i++)
                {
                    followers.Add(new RediancieFollower(NPC.Center));
                }

                NPC.defense = NPC.defDefense + (int)OwnedFollowersCount;
                return;
            }

            ///正常情况下
            OwnedFollowersCount += howMany;
            for (int i = 0; i < howMany; i++)
            {
                followers.Add(new RediancieFollower(NPC.Center));
            }

            NPC.defense = NPC.defDefense + (int)OwnedFollowersCount;
        }

        public void RespawnFollowers()
        {
            followers.Clear();

            int maxFollowers = Helper.ScaleValueForDiffMode(6, 9, 12, 24);
            if (OwnedFollowersCount >= maxFollowers)    ///弹药数已经达到上限
                OwnedFollowersCount = maxFollowers;

            for (int i = 0; i < (int)OwnedFollowersCount; i++)
            {
                followers.Add(new RediancieFollower(NPC.Center));
            }

            NPC.defense = NPC.defDefense + (int)OwnedFollowersCount;
        }

        /// <summary> 消耗弹药，返回false说明无法消耗弹药，返回true说明成功消耗了弹药 </summary>
        /// <param name="howMany"></param>
        /// <returns></returns>
        public bool DespawnFollowers(int howMany)
        {
            if (OwnedFollowersCount == 0 || followers.Count == 0)
                return false;

            for (int i = 0; i < howMany; i++)
            {
                OwnedFollowersCount -= 1;
                followers.RemoveAt(followers.Count - 1);
            }

            NPC.defense = NPC.defDefense + (int)OwnedFollowersCount;

            if (OwnedFollowersCount == 0 || followers.Count == 0)
                return false;

            return true;
        }

        /// <summary> 是否能消耗弹药 </summary>
        public bool CanDespawnFollower() => OwnedFollowersCount != 0;

        public void UpdateFollower_Idle(float centerLerpSpeed = 0.6f)
        {
            float velLength = NPC.velocity.Length();
            float baseRot = Timer * 0.08f + velLength * 0.15f;
            float length = 38 + velLength / 2;
            ///额...总之是非常复杂的立体解析几何，用于计算当前这个圆以X轴为轴的旋转角度，根据玩家位置来的
            float CircleRot = 1.57f - Math.Clamp((Target.Center.Y - NPC.Center.Y) / 200, -1f, 1f) * 0.4f;
            for (int i = 0; i < followers.Count; i++)
            {
                FollowersAI_Idle(followers[i], i, baseRot, length, CircleRot, centerLerpSpeed);
            }
        }

        public void UpdateFollower_UpShoot(float centerLerpSpeed = 0.6f)
        {
            float baseRot = Timer * 0.1f;
            float length = 38 + Timer * 0.5f;
            float CircleRot = 1.57f - Math.Clamp((Target.Center.Y - NPC.Center.Y) / 200, -1f, 1f) * 0.4f;
            for (int i = 0; i < followers.Count; i++)
            {
                FollowersAI_UpShoot(followers[i], i, baseRot, length, CircleRot, centerLerpSpeed);
            }
        }

        public void UpdateFollower_MagicShoot(Vector2 targetCenter, Vector2 targetDir, float factor, float centerLerpSpeed = 0.6f)
        {
            if (OwnedFollowersCount == 0 || followers.Count == 0)
            {
                ResetState();
                return;
            }

            RediancieFollower lastFollower = followers[^1];
            lastFollower.center = Vector2.Lerp(lastFollower.center, targetCenter, 0.6f);
            lastFollower.rotation = lastFollower.rotation.AngleLerp(targetDir.ToRotation() + 1.57f, 0.2f);
            lastFollower.drawBehind = false;
            lastFollower.scale = 1f;

            float baseRot = Timer * 0.1f;
            float lengthFactor;
            //只是计算长度，大概有一个后坐力的效果
            if (Timer < 70)
                lengthFactor = 0f;
            else
            {
                float timeFactor = 1 - Timer % 35 / 35;
                float x = 1.465f * timeFactor;
                lengthFactor = x * MathF.Sin(x * x * x) / 1.186f;
            }

            float length = 36 + lengthFactor * 60;
            Matrix XRot = Matrix.CreateRotationX(0.2f + targetDir.Y * factor * 1.1f);
            Matrix YRot = Matrix.CreateRotationY(-(0.2f + targetDir.X * factor * 1.1f));
            for (int i = 0; i < followers.Count - 1; i++)
            {
                FollowerAI_MagicShoot(followers[i], i, baseRot, length, targetCenter, XRot, YRot, centerLerpSpeed);
            }
        }

        public void UpdateFollower_Summon()
        {
            float baseRot = Timer * 0.06f;
            float length = 38 + Math.Clamp(Timer * 30, 0, 30);
            float CircleRot = 1.57f - Math.Clamp((Target.Center.Y - NPC.Center.Y) / 200, -1f, 1f) * 0.4f;

            for (int i = 0; i < followers.Count; i++)
            {
                FollowersAI_Idle(followers[i], i, baseRot, length, CircleRot, 0.6f);
            }
        }

        public void UpdateFollower_Firework()
        {
            float baseRot = Timer * 0.06f;
            float length = 46 + Math.Clamp(Timer * 30, 0, 30);

            for (int i = 0; i < followers.Count; i++)//因为比较简单所以就直接写在里面了
            {
                RediancieFollower follower = followers[i];

                float rot = baseRot + (i / (float)followers.Count) * MathHelper.TwoPi;
                follower.center = Vector2.Lerp(follower.center, NPC.Center + rot.ToRotationVector2() * length, 0.1f + 0.5f * Math.Clamp(Timer / 60, 0, 1));
                follower.rotation = follower.rotation.AngleLerp(NPC.rotation, 0.6f);
                follower.drawBehind = false;
                follower.scale = 1f;
            }
        }

        public void UpdateFollower_Pulse(Vector2 targetCenter, Vector2 targetDir, float factor, float timer = -1, float centerLerpSpeed = 0.6f)
        {
            if (OwnedFollowersCount == 0 || followers.Count == 0)
            {
                ResetState();
                return;
            }

            RediancieFollower lastFollower = followers[^1];
            lastFollower.center = Vector2.Lerp(lastFollower.center, targetCenter+targetDir*16, 0.6f);
            lastFollower.rotation = lastFollower.rotation.AngleLerp(targetDir.ToRotation() + 1.57f, 0.2f);
            lastFollower.drawBehind = false;
            lastFollower.scale = 1.3f;

            float baseRot = Timer * 0.1f;
            float lengthFactor;
            //只是计算长度，大概有一个后坐力的效果
            if (timer < 0)
                lengthFactor = 0f;
            else
            {
                float timeFactor = 1 - timer / 65;
                float x = 1.465f * timeFactor;
                lengthFactor = x * MathF.Sin(x * x * x) / 1.186f;
            }

            float length = 26 + lengthFactor * 86;
            Matrix XRot = Matrix.CreateRotationX(0.2f + targetDir.Y * factor * 1.1f);
            Matrix YRot = Matrix.CreateRotationY(-(0.2f + targetDir.X * factor * 1.1f));
            for (int i = 0; i < followers.Count - 1; i++)
            {
                FollowerAI_MagicShoot(followers[i], i, baseRot, length, targetCenter, XRot, YRot, centerLerpSpeed);
            }
        }


        /// <summary>
        /// 默认动作，在赤玉灵身边环绕，需要输入计算好的角度及长度数值
        /// </summary>
        /// <param name="follower"></param>
        public void FollowersAI_Idle(RediancieFollower follower, int whoamI, float baseRot, float length, float CircleRot, float centerLerpSpeed)
        {
            float rot = baseRot + (whoamI / (float)followers.Count) * MathHelper.TwoPi;

            Vector2 vector2D = rot.ToRotationVector2();
            Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), Matrix.CreateRotationX(CircleRot));///将二维的向量转为3维的并绕着X轴旋转一下
            vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationZ(NPC.rotation));///以Z为轴旋转，用来配合赤玉灵自身的旋转

            //将3维向量投影到二维
            float k1 = -1000 / (vector3D.Z - 1000);
            Vector2 targetDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = NPC.Center + targetDir * length + new Vector2(0, MathF.Sin(whoamI*1.2f) * 6);
            follower.center = Vector2.Lerp(follower.center, targetCenter, centerLerpSpeed);
            follower.rotation = follower.rotation.AngleLerp(NPC.rotation, 0.2f);
            follower.drawBehind = vector3D.Z > 0;
            follower.scale = 0.9f - vector3D.Z * 0.2f;
        }

        public void FollowersAI_UpShoot(RediancieFollower follower, int whoamI, float baseRot, float length, float CircleRot, float centerLerpSpeed)
        {
            float rot = baseRot + (whoamI / (float)followers.Count) * MathHelper.TwoPi;

            Vector2 vector2D = rot.ToRotationVector2();
            Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), Matrix.CreateRotationX(CircleRot));///将二维的向量转为3维的并绕着X轴旋转一下
            vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationZ(NPC.rotation));///以Z为轴旋转，用来配合赤玉灵自身的旋转

            //将3维向量投影到二维
            float k1 = -1000 / (vector3D.Z - 1000);
            Vector2 targetDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = NPC.Center + targetDir * length + new Vector2(0, MathF.Sin(whoamI * 1.2f) * 6);
            follower.center = Vector2.Lerp(follower.center, targetCenter, centerLerpSpeed);
            follower.rotation = follower.rotation.AngleLerp(NPC.rotation, 0.2f);
            follower.drawBehind = vector3D.Z > 0;
            follower.scale = 1f - vector3D.Z * (0.2f + 0.4f * Math.Clamp(length / 168f, 0, 1));
        }

        public void FollowerAI_MagicShoot(RediancieFollower follower, int whoamI, float baseRot, float length, Vector2 center, Matrix XRot, Matrix YRot, float centerLerpSpeed)
        {
            float totalCount = ((followers.Count - 1) == 0) ? 1 : (followers.Count - 1);  //分母不能为0
            float rot = baseRot + MathHelper.TwoPi * whoamI / totalCount;

            Vector2 vector2D = rot.ToRotationVector2();
            ///在XY平面的圆，先以X轴为轴旋转，再以Y轴为轴旋转，最后达到大概瞄准玩家的圆圈的效果
            Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), XRot);
            vector3D = Vector3.Transform(vector3D, YRot);

            float k1 = -1000 / (vector3D.Z - 1000);
            Vector2 CircleDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = center + CircleDir * length;

            follower.center = Vector2.Lerp(follower.center, targetCenter, centerLerpSpeed);
            follower.rotation = follower.rotation.AngleLerp(CircleDir.ToRotation() + 1.57f, 0.4f);
            follower.drawBehind = vector3D.Z > 0;
            follower.scale = 0.9f - vector3D.Z * 0.3f;
        }

        #endregion

        #region NetWork

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Timer = reader.ReadSingle();
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            foreach (var follower in followers)
            {
                if (follower.drawBehind)
                    follower.Draw(spriteBatch, drawColor);
            }

            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            //int frameWidth = mainTex.Width;
            //int frameHeight = mainTex.Height / Main.npcFrameCount[NPC.type];
            //Rectangle frameBox = new Rectangle(0, NPC.frame.Y * frameHeight, frameWidth, frameHeight);

            Vector2 origin = mainTex.Size() / 2;
            //= new Vector2(frameWidth / 2, frameHeight / 2);

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            foreach (var follower in followers)
            {
                if (!follower.drawBehind)
                    follower.Draw(spriteBatch, drawColor);
            }

            return false;
        }

        #endregion
    }
}
