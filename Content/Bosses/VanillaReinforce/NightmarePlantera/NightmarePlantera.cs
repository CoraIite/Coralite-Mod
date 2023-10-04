using Coralite.Content.Items.Misc_Shoot;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public sealed partial class NightmarePlantera : ModNPC, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private Player Target => Main.player[NPC.target];

        private ref float Phase => ref NPC.ai[0];
        private ref float State => ref NPC.ai[1];
        private ref float SonState => ref NPC.ai[2];
        private ref float MoveCount => ref NPC.ai[3];

        public float EXai1;
        public float ShootCount;

        public int Timer;
        public int tentacleStarFrame;
        private bool spawnedHook;
        private bool useMeleeDamage;
        public bool canOnlyBeHitByFantasyGod;
        public RotateTentacle[] rotateTentacles;
        public Color tentacleColor;

        public static FlowerParticle[] particles_front;
        public static FlowerParticle[] particles_ffront;

        /// <summary>
        /// 击杀美梦光的数量
        /// </summary>
        public int fantasyKillCount;

        public static Asset<Texture2D> tentacleTex;
        public static Asset<Texture2D> tentacleFlowTex;
        public static Asset<Texture2D> waterFlowTex;
        public static Asset<Texture2D> flowerParticleTex;

        /// <summary> 自身BOSS的索引，用于方便爪子获取自身/ </summary>
        public static int NPBossIndex = -1;

        public float alpha = 1f;

        public static Color[] phantomColors;
        public static Color nightPurple = new Color(204, 170, 242, 230);
        public static Color lightPurple = new Color(195, 116, 219, 230);
        public static Color nightmareSparkleColor = new Color(111, 80, 180, 230);
        public static Color nightmareRed = new Color(250, 0, 100);

        #region tml hooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;

            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 12;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.MustAlwaysDraw[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            //NPCID.Sets.DebuffImmunitySets[Type] = new NPCDebuffImmunityData()
            //{
            //    SpecificallyImmuneTo = new int[]
            //    {
            //        BuffID.PotionSickness,
            //        BuffID.OnFire,
            //        BuffID.Bleeding,
            //        BuffID.Ichor,
            //        BuffID.Venom,
            //        BuffID.OnFire3,
            //        BuffID.BloodButcherer,
            //        BuffID.Confused
            //    }
            //};

            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 88;
            NPC.height = 88;
            NPC.lifeMax = 20_0000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.damage = 80;
            NPC.defense = 35;
            NPC.lifeMax = 5_0000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 20f;
            NPC.value = Item.buyPrice(0, 15);
            NPC.HitSound = CoraliteSoundID.Fleshy_NPCHit1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            //NPC.hide = true;
            if (Main.BigBossProgressBar.TryGetSpecialVanillaBossBar(NPCID.QueenSlimeBoss, out IBigProgressBar bossbar))
                NPC.BossBar = bossbar;

            //BGM：来世-世纪之花
            if (!Main.dedServ)
                Music = MusicID.OtherworldlyPlantera;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((23_5000 + numPlayers * 5_4000) / journeyScale);
                    NPC.damage = 100;
                    NPC.defense = 35;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((32_8000 + numPlayers * 7_8000) / journeyScale);
                    NPC.defense = 40;
                    NPC.damage = 120;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 140;
                    NPC.defense = 45;
                }

                return;
            }

            NPC.lifeMax = 23_5000 + numPlayers * 5_4000;
            NPC.damage = 100;
            NPC.defense = 35;

            if (Main.masterMode)
            {
                NPC.lifeMax = 32_8000 + numPlayers * 7_8000;
                NPC.defense = 40;
                NPC.damage = 120;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 43_2000 + numPlayers * 10_0000;
                NPC.damage = 140;
                NPC.defense = 45;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.BossBag(ItemType<EmperorSabre>()));

            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<GelThrone>()));
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<RedianciePet>(), 4));
            //npcLoot.Add(ItemDropRule.BossBag(ItemType<RediancieBossBag>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));

            //LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            //notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<RedJade>(), 1, 20, 24));
            //npcLoot.Add(notExpertRule);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            //if ((projectile.penetrate < 0 || projectile.penetrate > 1) && modifiers.DamageType != DamageClass.Melee)
            //    modifiers.SourceDamage *= 0.75f;
            if (projectile.type == ProjectileID.FinalFractal)
            {
                modifiers.SourceDamage *= 0.6f;
                return;
            }

            if (projectile.type == ModContent.ProjectileType<HyacinthBullet>() || projectile.type == ModContent.ProjectileType<HyacinthBullet2>()
                || projectile.type == ModContent.ProjectileType<HyacinthExplosion>())
            {
                modifiers.SourceDamage *= 0.6f;
                return;
            }

            modifiers.ModifyHitInfo += Modifiers_ModifyHitInfo;
        }

        private void Modifiers_ModifyHitInfo(ref NPC.HitInfo info)
        {
            if (info.Damage > 5000)
                info.Damage = 100;
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            tentacleTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Tentacle");
            tentacleFlowTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "TentacleFlow");
            CircleWarpTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "CircleWarp");
            BlackBack = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "BlackBack");
            NameLine = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NPNameLine");
            waterFlowTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "WaterFlow");
            flowerParticleTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "FlowerParticle");

            phantomColors = new Color[7];
            for (int i = 0; i < 7; i++)
            {
                phantomColors[i] = Main.hslToRgb(i * 1 / 7f, 45 / 100f, 30 / 100f, 200);
            }
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            tentacleTex = null;
            tentacleFlowTex = null;
            CircleWarpTex = null;
            BlackBack = null;
            NameLine = null;
            waterFlowTex = null;
            flowerParticleTex = null;
        }

        public override bool PreKill()
        {
            SoundStyle st = CoraliteSoundID.BigBOOM_Item62;
            st.Pitch = -0.5f;
            SoundEngine.PlaySound(st, NPC.Center);

            for (int i = 0; i < 24; i++)
            {
                Color color = Main.rand.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => nightmareRed
                };

                Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir() * Main.rand.NextFloat(6, 24f),
                    CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1.5f));
            }

            return base.PreKill();
        }

        public override void OnKill()
        {
            NPBossIndex = -1;
            DownedBossSystem.DownNightmarePlantera();
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (canOnlyBeHitByFantasyGod)
                return projectile.type == ModContent.ProjectileType<FantasyBall>();

            return null;
        }

        public override bool CanBeHitByNPC(NPC attacker)
        {
            if (canOnlyBeHitByFantasyGod)
                return false;

            return true;
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            if (canOnlyBeHitByFantasyGod)
                return false;

            return null;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<FantasyBall>() || projectile.type == ModContent.ProjectileType<FantasySpike>())
            {
                NPC.rotation += Main.rand.NextFloat(-0.2f, 0.2f);
            }
            OnHit(hit.Damage);
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            OnHit(hit.Damage);
        }

        public void OnHit(int damage)
        {
        }

        public override bool CheckDead()
        {
            //if (State != (int)AIStates.OnKillAnim)
            //{
            //    State = (int)AIStates.OnKillAnim;
            //    Timer = 0;
            //    NPC.dontTakeDamage = true;
            //    NPC.life = 1;
            //    return false;
            //}

            return true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => useMeleeDamage;

        #endregion

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest(false);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Phase = (int)AIPhases.OnSpawnAnmi_P0;
                NPC.netUpdate = true;
            }

            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                NCamera.Reset();

            NPC.Center = Target.Center + new Vector2(Target.direction * 300, -200);
            alpha = 0;
            NPC.dontTakeDamage = true;

            Helper.PlayPitched("Music/Heart", 1f, 0f, NPC.Center);
            Music = 0;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || /*Target.Distance(NPC.Center) > 4800 ||*/ Main.dayTime) //世花也是4800
            {
                NPC.TargetClosest();

                do
                {
                    if (Main.dayTime)
                    {
                        Phase = (int)AIPhases.Rampage; //狂暴的AI
                        break;
                    }

                    if (Target.dead || !Target.active)
                    {
                        NPC.EncourageDespawn(10);
                        NPC.dontTakeDamage = true;  //脱战无敌
                        NPC.velocity.Y += 0.25f;
                        ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;
                        if (rotateTentacles != null)
                        {
                            NormallySetTentacle();
                            NormallyUpdateTentacle();
                        }

                        return;
                    }
                    else
                        ResetStates();
                } while (false);
            }

            NPBossIndex = NPC.whoAmI;

            switch ((int)Phase)
            {
                default:
                    ResetStates();
                    break;
                case (int)AIPhases.OnSpawnAnmi_P0:
                    OnSpawnAnmi();
                    break;
                case (int)AIPhases.Sleeping_P1:
                    Sleeping_Phase1();
                    break;
                case (int)AIPhases.Exchange_P1_P2:
                    Exchange_P1_P2();
                    break;
                case (int)AIPhases.Dream_P2:
                    Dream_Phase2();
                    break;
                case (int)AIPhases.Nightemare_P3:
                    Nightmare_Phase3();
                    break;
                case (int)AIPhases.WakeUp_P4:
                    break;
                case (int)AIPhases.Rampage:
                    Rampage();
                    break;
                case (int)AIPhases.SuddenDeath:
                    ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;
                    NormallySetTentacle();
                    SuddenDeath();
                    NormallyUpdateTentacle();
                    break;
            }
        }

        #endregion

        #region States

        public enum AIPhases
        {
            /// <summary>
            /// 生成动画
            /// </summary>
            OnSpawnAnmi_P0,
            /// <summary> 一阶段：入梦 </summary>
            Sleeping_P1,
            /// <summary> 一阶段和二阶段的切换 </summary>
            Exchange_P1_P2,
            /// <summary> 二阶段：噩梦 </summary>
            Dream_P2,
            ///<summary> 三阶段：梦魇 </summary>
            Nightemare_P3,
            /// <summary> 尾杀：惊醒 </summary>
            WakeUp_P4,
            /// <summary> 狂暴 </summary>
            Rampage,
            /// <summary> 秒杀玩家的动作 </summary>
            SuddenDeath
        }

        public enum AIStates
        {
            /// <summary> 沉眠之雾 </summary>
            hypnotizeFog,
            /// <summary> 黑暗之触 </summary>
            darkTentacle,
            /// <summary> 黑暗飞叶 </summary>
            darkLeaves,
            /// <summary> 一阶段中的idle,这时候为准备攻击阶段，什么也不干 </summary>
            P1_Idle,

            //以下为2阶段招式

            /// <summary> 噩梦之咬  </summary>
            nightmareBite,
            /// <summary> 噩梦冲刺 </summary>
            nightmareDash,
            /// <summary> 假装咬但张开嘴后消失并射出2个荆棘刺 </summary>
            fakeBite,
            /// <summary> 转圈圈放弹幕，之后瞬移到另一方向咬下 </summary>
            rollingThenBite,
            /// <summary> 在玩家下方转圈圈并放弹幕，之后瞬移到玩家面朝方向上方向斜上方咬下 </summary>
            belowSparkleThenBite,
            /// <summary> 射出一些球球，然后从中刺出尖刺 </summary>
            spikeBalls,
            /// <summary> 放出蝙蝠限制玩家走位，同时射出蝙蝠和绕圈的乌鸦 </summary>
            batsAndCrows,
            /// <summary> 爪击 </summary>
            hookSlash,
            /// <summary> 在一边生成刺+，自身吐出弹幕</summary>
            spikesAndSparkles,
            /// <summary> 旋转并放出尖刺 </summary>
            spikeHell,
            /// <summary> 瞬移冲刺，之后放出鬼手 </summary>
            ghostDash,
            /// <summary> 瞬移后射弹幕 </summary>
            teleportSparkle,
            /// <summary> 梦境之光 </summary>
            dreamSparkle,
            /// <summary> 第二阶段的idle，只是在玩家身边绕圈圈 </summary>
            P2_Idle,
            /// <summary> 美梦猎杀，持续生成并尝试击杀美梦光 </summary>
            fantasyHunting,
            /// <summary> 黑洞，未能收集梦境之光的惩罚招式 </summary>
            blackHole,

            //三阶段
            /// <summary> 二三阶段切换，只是简单爆开而已 </summary>
            exchange_P2_P3,
            /// <summary> 幻象之咬，随机放出几个幻象，之后才自己咬上来 </summary>
            illusionBite,
            /// <summary> 三重尖刺地狱 </summary>
            tripleSpikeHell,
            /// <summary> 花之舞，转圈圈弹幕 </summary>
            flowerDance,
            /// <summary> 一堆爪击 </summary>
            superHookSlash,
            /// <summary> 召唤荆棘刺然后刺出 </summary>
            vineSpurt,
            /// <summary> 三阶段的蝙蝠渡鸦 </summary>
            P3_batsAndCrows,
            /// <summary> 射出追踪的荆棘，并在沿途释放种子球 </summary>
            vinesAndSeeds,
            /// <summary> 三阶段的刺+弹幕 </summary>
            P3_SpikesAndSparkles,
            /// <summary> 瞬移转圈圈 </summary>
            P3_teleportSparkles,
            P3_nightmareBite,
            P3_nightmareDash,
            P3_fakeBite,
        }

        public void ResetStates()
        {
            if (!haveBeenPhase2 && NPC.life > NPC.lifeMax * 3 / 4)
            {
                Phase = (int)AIPhases.Sleeping_P1;
                SetPhase1Idle();
                return;
            }

            if (NPC.life > NPC.lifeMax / 8)
            {
                Phase = (int)AIPhases.Dream_P2;
                SetPhase2States();
                return;
            }

            //设置P3的状态
            SetPhase3States();
        }

        public void ChangeToSuddenDeath(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if ((int)Phase is (int)AIPhases.Sleeping_P1 or (int)AIPhases.Exchange_P1_P2)
            {
                return;
            }

            NPC.target = player.whoAmI;
            NPC.NewProjectileInAI<SuddenDeath>(Target.Center, Vector2.Zero, 0, 0, NPC.target);
            Phase = (int)AIPhases.SuddenDeath;
            State = 0;
            SonState = 0;
            Timer = 0;
            ShootCount = 0;

            NPC.netUpdate = true;
        }

        #endregion

        #region HelperMethods

        public static bool NightmarePlanteraAlive(out NPC np)
        {
            if (NPBossIndex >= 0 && NPBossIndex < 201 && Main.npc[NPBossIndex].active && Main.npc[NPBossIndex].type == ModContent.NPCType<NightmarePlantera>())
            {
                np = Main.npc[NPBossIndex];
                return true;
            }

            np = null;
            return false;
        }

        public static void NightmareHit(Player player)
        {
            player.AddBuff(ModContent.BuffType<DreamErosion>(), 18000);
            if (!NightmarePlanteraAlive(out NPC np))
                return;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.nightmareCount < 14)
                    cp.nightmareCount += (byte)Helper.ScaleValueForDiffMode(1, 1, 2, 14);

                //设置阶段并秒杀玩家
                if (cp.nightmareCount > 13)
                    (np.ModNPC as NightmarePlantera).ChangeToSuddenDeath(player);

                if (player.whoAmI == Main.myPlayer)
                    Filters.Scene.Activate("NightmareScreen", player.position);
            }
        }

        public Vector2 GetPhase1MousePos()
        {
            return NPC.Center + NPC.rotation.ToRotationVector2() * 100;
        }

        public void DoRotation(float maxChange)
        {
            NPC.rotation = NPC.rotation.AngleTowards((Target.Center - NPC.Center).ToRotation(), maxChange);
        }

        public Color TentacleColor(float factor)
        {
            return Color.Lerp(tentacleColor, Color.Transparent, factor) * alpha;
        }

        public static float TentacleWidth(float factor)
        {
            if (factor > 0.5f)
                return Helper.Lerp(25, 0, (factor - 0.5f) / 0.5f);

            return Helper.Lerp(0, 25, factor / 0.5f);
        }

        public void NormallySetTentacle()
        {
            Vector2 center = NPC.Center - NPC.velocity * 2;
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = rotateTentacles[i];
                float factor = MathF.Sin((float)Main.timeForVisualEffects / 12 + i * 1.5f);
                float targetRot = tentacle.rotation.AngleLerp(NPC.rotation + factor * 1.3f, 0.4f);
                Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                    center + (i * 30 + 140) * (NPC.rotation + factor * 0.65f + MathHelper.Pi).ToRotationVector2(), 0.2f);
                tentacle.SetValue(selfPos, NPC.Center, targetRot);
            }
        }

        public void NormallyUpdateTentacle()
        {
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = rotateTentacles[i];
                tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
            }
        }

        #endregion

        #region NetWork

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ShootCount);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ShootCount = reader.ReadSingle();
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Rectangle frameBox = mainTex.Frame(1, Main.npcFrameCount[NPC.type], NPC.frame.X, NPC.frame.Y);
            Vector2 origin = frameBox.Size() / 2;
            Vector2 pos = NPC.Center - screenPos;
            float selfRot = NPC.rotation + MathHelper.PiOver2;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

            if (rotateTentacles != null)
                for (int j = 0; j < 3; j++)
                    rotateTentacles[j]?.DrawTentacle_NoEndBegin(i => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly), 2);

            for (int k = 0; k < Main.maxProjectiles; k++) //Projectiles
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is INightmareTentacle)
                    (Main.projectile[k].ModProjectile as INightmareTentacle).DrawTentacle();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.Transform);

            if (alpha != 1)
            {
                //绘制7个幻影
                float angle = alpha * MathHelper.Pi;
                float distance = MathF.Sin(alpha * MathHelper.Pi) * 64;

                for (int i = 0; i < 7; i++)
                {
                    Color c = phantomColors[i] * alpha;
                    spriteBatch.Draw(mainTex, pos + (i * 1 / 7f * MathHelper.TwoPi + angle).ToRotationVector2() * distance, frameBox, c * alpha, selfRot, origin, NPC.scale, 0, 0);
                }
            }

            //绘制自己
            spriteBatch.Draw(mainTex, pos, frameBox, Color.White * alpha, selfRot, origin, NPC.scale, 0, 0);

            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (rotateTentacles != null)
            {
                Texture2D sparkleTex = ConfusionHole.SparkleTex.Value;
                var frameBox = sparkleTex.Frame(1, 2, 0, tentacleStarFrame);
                Vector2 origin = frameBox.Size() / 2;
                //float rot = Main.GlobalTimeWrappedHourly * 0.5f;
                Color c = Color.White;
                c.A = (byte)(200 * alpha);
                for (int j = 0; j < 3; j++) //绘制触手上的三个小星星
                {
                    Vector2 pos = rotateTentacles[j].pos - Main.screenPosition;
                    spriteBatch.Draw(sparkleTex, pos, frameBox, c, rotateTentacles[j].rotation, origin, 0.3f + Main.rand.NextFloat(0, 0.02f), 0, 0);
                }
            }
        }

        #endregion
    }
}
