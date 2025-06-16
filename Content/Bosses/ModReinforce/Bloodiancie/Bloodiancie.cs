using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Items.RedJades;
using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    [AutoloadBossHead]
    public partial class Bloodiancie : ModNPC
    {
        public override string Texture => AssetDirectory.Bloodiancie + Name;

        private Player Target => Main.player[NPC.target];

        public bool ExchangeState = true;

        internal ref float DamageCount => ref NPC.ai[0];
        internal ref float State => ref NPC.ai[1];
        /// <summary> 招式循环的方式，具体使用MoveCycling查看 </summary>
        internal ref float MoveCyclingType => ref NPC.ai[2];

        /// <summary> 拥有的“弹药”数量 </summary>
        internal ref float OwnedFollowersCount => ref NPC.ai[3];

        internal int Timer;
        /// <summary> 目前的AI循环的计数 </summary>
        internal ref float MoveCount => ref NPC.localAI[1];

        internal static readonly Color red = new(221, 50, 50);
        internal static readonly Color grey = new(91, 93, 102);
        public const int ON_KILL_ANIM_TIME = 250;

        public List<BloodiancieFollower> followers;

        #region tml hooks

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 80;
            NPC.damage = 75;
            NPC.defense = 40;
            NPC.lifeMax = 23000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;

            NPC.BossBar = GetInstance<RediancieBossBar>();
            GetInstance<RediancieBossBar>().Reset(NPC);

            //BGM：赤色激流
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/RedTorrent");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((21000 + (numPlayers * 9500)) / journeyScale);
                    NPC.damage = 90;
                    NPC.defense = 18;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((25500 + (numPlayers * 11500)) / journeyScale);
                    NPC.damage = 115;
                    NPC.defense = 20;
                }

                if (Main.getGoodWorld)
                {
                    NPC.defense = 14;//因为FTW种能够拥有非常多的弹药所以就降低一下基础防御了
                }

                return;
            }

            NPC.lifeMax = 21000 + (numPlayers * 9500);
            NPC.damage = 90;
            NPC.defense = 18;

            if (Main.masterMode)
            {
                NPC.lifeMax = 25500 + (numPlayers * 11500);
                NPC.damage = 115;
                NPC.defense = 20;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 30000 + (numPlayers * 14500);
                NPC.damage = 115;
                NPC.defense = 14;//因为FTW种能够拥有非常多的弹药所以就降低一下基础防御了
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<BloodiancieRelic>()));
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<RedianciePet>(), 4));
            npcLoot.Add(ItemDropRule.BossBag(ItemType<BloodiancieBossBag>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<BloodJade>(), 1, 30, 38));
            npcLoot.Add(notExpertRule);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                SkyManager.Instance.Deactivate("BloodJadeSky");

                for (int j = 0; j < 5; j++)
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
            DownedBossSystem.DownBloodiancie();
        }

        public override void Load()
        {
            BloodiancieFollower.tex1 = Request<Texture2D>(AssetDirectory.Bloodiancie + "BloodiancieFollower1");
            BloodiancieFollower.tex2 = Request<Texture2D>(AssetDirectory.Bloodiancie + "BloodiancieFollower2");

            //for (int i = 0; i < 5; i++)
            //    GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.BossGores + "Rediancie_Gore" + i);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            OnHit(hit.Damage);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.pick > 0)
                modifiers.SourceDamage.Flat += item.pick;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            OnHit(hit.Damage);
        }

        public void OnHit(int damage)
        {
            DamageCount += damage;
            int value = Helper.ScaleValueForDiffMode(500, 800, 1000, 2000);
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
                SkyManager.Instance.Deactivate("BloodJadeSky");
                return false;
            }

            return true;
        }

        #endregion

        #region NetWork

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Timer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Timer = reader.ReadInt32();
        }

        #endregion

        #region Draw

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
                return Main.DiscoColor;

            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color color = drawColor;
            if (Main.zenithWorld)
                color = Main.DiscoColor;

            if (followers != null)
            {
                foreach (var follower in followers)
                {
                    if (follower.drawBehind)
                        follower.Draw(spriteBatch, color);
                }

                DrawSelf(spriteBatch, screenPos, color);

                foreach (var follower in followers)
                {
                    if (!follower.drawBehind)
                        follower.Draw(spriteBatch, color);
                }
            }
            else
                DrawSelf(spriteBatch, screenPos, color);

            return false;
        }

        private void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //绘制自己
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Vector2 origin = mainTex.Size() / 2;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        }

        #endregion
    }
}
