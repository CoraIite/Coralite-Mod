using Coralite.Content.ModPlayers;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public sealed partial class NightmarePlantera : ModNPC
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private Player Target => Main.player[NPC.target];

        private ref float Phase => ref NPC.ai[0];
        private ref float State => ref NPC.ai[1];
        private ref float SonState => ref NPC.ai[2];

        public int Timer;
        private bool spawnedHook;
        private bool useMeleeDamage;

        public static Asset<Texture2D> tentacleTex;
        public static Asset<Texture2D> tentacleFlowTex; 

        /// <summary>
        /// 自身BOSS的索引，用于方便爪子获取自身
        /// </summary>
        public static int NPBossIndex = -1;

        public float alpha=1f;

        #region tml hooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 12;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 86;
            NPC.height = 86;
            NPC.lifeMax = 30000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.damage = 50;
            NPC.defense = 14;
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
            //NPC.BossBar = GetInstance<RediancieBossBar>();

            //BGM：来世-世纪之花
            if (!Main.dedServ)
                Music = MusicID.OtherworldlyPlantera;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            //if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            //{
            //    if (nPCStrengthHelper.IsExpertMode)
            //    {
            //        NPC.lifeMax = (int)((5600 + numPlayers * 1460) / journeyScale);
            //        NPC.damage = 75;
            //        NPC.defense = 10;
            //    }

            //    if (nPCStrengthHelper.IsMasterMode)
            //    {
            //        NPC.lifeMax = (int)((6200 + numPlayers * 3030) / journeyScale);
            //        NPC.scale *= 1.25f;
            //        NPC.defense = 14;
            //        NPC.damage = 100;
            //    }

            //    if (Main.getGoodWorld)
            //    {
            //        NPC.damage = 120;
            //        NPC.scale *= 1.25f;
            //        NPC.defense = 16;
            //    }

            //    return;
            //}

            //NPC.lifeMax = 5600 + numPlayers * 1460;
            //NPC.damage = 75;
            //NPC.defense = 10;

            //if (Main.masterMode)
            //{
            //    NPC.lifeMax = 6200 + numPlayers * 3030;
            //    NPC.scale *= 1.25f;
            //    NPC.defense = 14;
            //    NPC.damage = 100;
            //}

            //if (Main.getGoodWorld)
            //{
            //    NPC.lifeMax = 6800 + numPlayers * 4060;
            //    NPC.damage = 140;
            //    NPC.scale *= 1.25f;
            //    NPC.defense = 16;
            //}
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

        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            tentacleTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Tentacle");
            tentacleFlowTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LaserTrail");
            CircleWarpTex= ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "CircleWarp");
            BlackBack = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "BlackBack");
            NameLine= ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NPNameLine");
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
        }

        public override void OnKill()
        {
            NPBossIndex = -1;
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
                //State = (int)AIStates.BodySlam;
                NPC.netUpdate = true;
            }
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 4800 || Main.dayTime) //世花也是4800
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
                    break;
                case (int)AIPhases.WapeUp_P4:
                    break;
                case (int)AIPhases.Rampage:
                    Rampage();
                    break;
                case (int)AIPhases.SuddenDeath:
                    break;
            }
        }

        public override void PostAI()
        {

        }

        #endregion

        #region States

        public enum AIPhases
        {
            /// <summary> 一阶段：入梦 </summary>
            Sleeping_P1,
            /// <summary> 一阶段和二阶段的切换 </summary>
            Exchange_P1_P2,
            /// <summary> 二阶段：噩梦 </summary>
            Dream_P2 ,
            ///<summary> 三阶段：梦魇 </summary>
            Nightemare_P3 ,
            /// <summary> 尾杀：惊醒 </summary>
            WapeUp_P4 ,
            /// <summary> 狂暴 </summary>
            Rampage ,
            /// <summary> 秒杀玩家的动作 </summary>
            SuddenDeath
        }

        public enum AIStates
        {
            /// <summary> 沉眠之雾 </summary>
            hypnotizeFog ,
            /// <summary> 黑暗之触 </summary>
            darkTentacle ,
            /// <summary> 黑暗飞叶 </summary>
            darkLeaves ,
            /// <summary> 一阶段中的idle,这时候为准备攻击阶段，什么也不干 </summary>
            P1_Idle ,

            //以下为2阶段招式

            /// <summary> 噩梦之咬  </summary>
            nightmareBite,

        }

        public void ResetStates()
        {

        }


        public void ChangeToSuddenDeath(Player player)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            NPC.target = player.whoAmI;
            Phase = (int)AIPhases.SuddenDeath;
            NPC.netUpdate = true;
        }

        #endregion

        #region HelperMethods

        public static bool NightmarePlanteraAlive(out NPC np)
        {
            if (NPBossIndex >= 0 && NPBossIndex < 201)
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
                if (cp.nightmareCount < 10)
                {
                    cp.nightmareCount++;

                    //设置阶段并秒杀玩家
                    if (cp.nightmareCount > 9)
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

        #endregion

        #region NetWork

        public override void SendExtraAI(BinaryWriter writer)
        {
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
        }

        #endregion

        #region Draw

        public void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Rectangle frameBox = mainTex.Frame(1, Main.npcFrameCount[NPC.type], NPC.frame.X, NPC.frame.Y);
            Vector2 origin = frameBox.Size() / 2;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, Color.White * alpha, NPC.rotation + MathHelper.PiOver2, origin, NPC.scale, 0, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            DrawSelf(spriteBatch, screenPos);
            return false;
        }

        #endregion
    }
}
