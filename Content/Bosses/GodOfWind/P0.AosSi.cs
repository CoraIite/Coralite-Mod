//using Coralite.Core;
//using Coralite.Helpers;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework;
//using System;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Microsoft.Xna.Framework.Graphics;
//using System.IO;
//using Terraria.GameContent;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Bosses.GodOfWind
//{
//    public class AosSi : ModNPC
//    {
//        public override string Texture => AssetDirectory.GodOfWind + "AosSiHead";

//        private Player Target => Main.player[NPC.target];

//        #region tml hooks

//        public override void SetStaticDefaults()
//        {
//            Main.npcFrameCount[Type] = 6;
//            NPCID.Sets.TrailingMode[Type] = 1;
//            NPCID.Sets.TrailCacheLength[Type] = 12;
//            NPCID.Sets.MPAllowedEnemies[Type] = true;
//            NPCID.Sets.BossBestiaryPriority.Add(Type);
//        }

//        public override void SetDefaults()
//        {
//            NPC.GravityMultiplier *= 2f;
//            NPC.width = 60;
//            NPC.height = 85;
//            NPC.scale = 1.5f;
//            NPC.damage = 40;
//            NPC.defense = 8;
//            NPC.lifeMax = 6100;
//            NPC.knockBackResist = 0f;
//            NPC.aiStyle = -1;
//            NPC.npcSlots = 20f;
//            NPC.value = Item.buyPrice(0, 4, 0, 0);
//            NPC.HitSound = CoraliteSoundID.Fleshy_NPCHit1;

//            NPC.noGravity = false;
//            NPC.noTileCollide = true;
//            NPC.boss = true;
//            //NPC.hide = true;
//            //NPC.BossBar = GetInstance<RediancieBossBar>();

//            //BGM：暂无
//            //if (!Main.dedServ)
//            //    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/？？？");
//        }

//        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
//        {
//            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
//            {
//                if (nPCStrengthHelper.IsExpertMode)
//                {
//                    NPC.lifeMax = (int)((5600 + numPlayers * 1460) / journeyScale);
//                    NPC.damage = 75;
//                    NPC.defense = 10;
//                }

//                if (nPCStrengthHelper.IsMasterMode)
//                {
//                    NPC.lifeMax = (int)((6200 + numPlayers * 3030) / journeyScale);
//                    NPC.scale *= 1.25f;
//                    NPC.defense = 14;
//                    NPC.damage = 100;
//                }

//                if (Main.getGoodWorld)
//                {
//                    NPC.damage = 120;
//                    NPC.scale *= 1.25f;
//                    NPC.defense = 16;
//                }

//                return;
//            }

//            NPC.lifeMax = 5600 + numPlayers * 1460;
//            NPC.damage = 75;
//            NPC.defense = 10;

//            if (Main.masterMode)
//            {
//                NPC.lifeMax = 6200 + numPlayers * 3030;
//                NPC.scale *= 1.25f;
//                NPC.defense = 14;
//                NPC.damage = 100;
//            }

//            if (Main.getGoodWorld)
//            {
//                NPC.lifeMax = 6800 + numPlayers * 4060;
//                NPC.damage = 140;
//                NPC.scale *= 1.25f;
//                NPC.defense = 16;
//            }
//        }

//        public override void ModifyNPCLoot(NPCLoot npcLoot)
//        {
//            //npcLoot.Add(ItemDropRule.BossBag(ItemType<EmperorSabre>()));

//            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<GelThrone>()));
//            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<RedianciePet>(), 4));
//            //npcLoot.Add(ItemDropRule.BossBag(ItemType<RediancieBossBag>()));
//            //npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));

//            //LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
//            //notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<RedJade>(), 1, 20, 24));
//            //npcLoot.Add(notExpertRule);
//        }

//        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
//        {
//            if ((projectile.penetrate < 0 || projectile.penetrate > 1) && modifiers.DamageType != DamageClass.Melee)
//                modifiers.SourceDamage *= 0.75f;

//        }

//        public override void Load()
//        {
//            if (Main.dedServ)
//                return;

//        }

//        public override void Unload()
//        {
//        }

//        public override void OnKill()
//        {
//        }

//        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
//        {
//            OnHit(hit.Damage);
//        }

//        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
//        {
//            OnHit(hit.Damage);
//        }

//        public void OnHit(int damage)
//        {
//        }

//        public override bool CheckDead()
//        {
//            //if (State != (int)AIStates.OnKillAnim)
//            //{
//            //    State = (int)AIStates.OnKillAnim;
//            //    Timer = 0;
//            //    NPC.dontTakeDamage = true;
//            //    NPC.life = 1;
//            //    return false;
//            //}

//            return true;
//        }

//        public override bool? CanFallThroughPlatforms() => NPC.Center.Y < (Target.Center.Y - NPC.height);

//        #endregion

//        #region AI

//        public override void Initialize()
//        {
//            NPC.TargetClosest(false);
//            if (Main.netMode != NetmodeID.MultiplayerClient)
//            {
//                //State = (int)AIStates.BodySlam;
//                NPC.netUpdate = true;
//            }
//        }

//        public override void AI()
//        {
//            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)
//            {
//                NPC.TargetClosest();

//                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)//没有玩家存活时离开
//                {
//                    NPC.noGravity = true;
//                    NPC.noTileCollide = true;
//                    NPC.velocity.Y -= 0.3f;
//                    NPC.EncourageDespawn(10);
//                    return;
//                }
//                //else
//                //    ResetStates();
//            }

//        }

//        public override void PostAI()
//        {

//        }

//        #endregion

//        #region NetWork

//        public override void SendExtraAI(BinaryWriter writer)
//        {
//        }

//        public override void ReceiveExtraAI(BinaryReader reader)
//        {
//        }

//        #endregion

//        #region Draw

//        public void DrawSelf()
//        {

//        }

//        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
//        {
//            Texture2D mainTex = TextureAssets.Npc[Type].Value;

//            return false;
//        }

//        #endregion
//    }
//}
