using Coralite.Content.Items.RedJadeItems;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.StoneImitator
{
    [AutoloadBossHead]
    public class StoneImitator : ModNPC
    {
        public override string Texture => AssetDirectory.StoneImitator + "StoneImitator";

        Player target => Main.player[NPC.target];

        internal ref float yFrame => ref NPC.ai[0];
        internal ref float State => ref NPC.ai[1];
        internal ref float Timer => ref NPC.ai[2];

        public bool exchangeState=true;

        #region tml hooks

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("赤玉灵");

            Main.npcFrameCount[Type] = 13;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 70;
            NPC.damage = 25;
            NPC.defense = 8;
            NPC.lifeMax = 770;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            int min = 12;
            int max = 15;

            if (Main.expertMode)
            {
                min = 17;
                max = 20;
            }

            if (Main.masterMode)
            {
                min = 25;
                max = 28;
            }

            npcLoot.Add(ItemDropRule.Common(ItemType<RedJade>(),1,min,max));

        }

        public override void OnKill()
        {
            for (int j = 0; j < 3; j++)
                Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(2, 2), Mod.Find<ModGore>("StoneImitator_Gore0").Type);

            Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(2, 2), Mod.Find<ModGore>("StoneImitator_Gore1").Type);
        }

        public override void Load()
        {
            for (int i = 0; i < 2; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.Gores + "StoneImitator_Gore" + i);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
        }

        #endregion

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest(false);
        }

        public override void AI()
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                yFrame += 1;
                if (yFrame == 13)
                    yFrame = 0;
                NPC.frameCounter = 0;
            }

            if (NPC.target < 0 || NPC.target == 255 || target.dead || !target.active || target.Distance(NPC.Center) > 3000)
                NPC.TargetClosest();

            if (target.dead || !target.active || target.Distance(NPC.Center) > 3000)
            {
                State = -1;
                NPC.velocity.X *= 0.97f;
                NPC.velocity.Y += 0.04f;
                NPC.EncourageDespawn(10);
                return;
            }

            NPC.direction = NPC.spriteDirection = target.position.X > NPC.position.X ? 1 : -1;
            NPC.directionY = target.Center.Y > NPC.Center.Y ? 1 : -1;
            switch (State)
            {
                default:
                case (int)AIStates.explosion:       //追着玩家并爆炸
                    //控制X方向的移动
                    Helper.NPCMovment_OneLine(ref NPC.velocity.X, NPC.direction, 5.5f, 0.09f, 0.22f, 0.97f);

                    //控制Y方向的移动
                    float yLenth = Math.Abs(target.Center.Y - NPC.Center.Y);
                    if (yLenth > 30)
                        Helper.NPCMovment_OneLine(ref NPC.velocity.Y, NPC.directionY, 4f, 0.06f, 0.06f, 0.97f);
                    else
                        NPC.velocity.Y *= 0.96f;

                    if (Timer % 80 == 0)//生成弹幕
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ProjectileType<SIP_Explosion>(), 40, 5f);

                    if (Timer > 250)
                        ResetState();

                    break;

                case (int)AIStates.upShoot:
                    SlowDownAndGoUp();

                    //隔固定时间射弹幕
                    if (Timer % 25 == 0)
                    {
                        int damage = NPC.GetAttackDamage_ForProjectiles(15, 20);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, -8).RotatedBy(((Timer / 20) - 3) * 0.08f), ProjectileType<SIP_Strike>(), damage, 5f);
                        SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                    }

                    if (Timer > 220)
                        ResetState();
                    break;

                case (int)AIStates.magicShoot:      //蓄力射魔法弹幕
                    SlowDownAndGoUp();

                    if (Main.netMode != NetmodeID.Server && Timer % 3 == 0)
                        for (int i = 0; i < 6; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, Vector2.Zero, 0, default, 1.3f);
                            dust.velocity = (NPC.Center - dust.position).SafeNormalize(Vector2.UnitY) * 3f;
                            dust.noGravity = true;
                        }

                    if (Timer < 36)
                        break;

                    if (Timer % 35 == 0)//生成弹幕
                    {
                        int damage = NPC.GetAttackDamage_ForProjectiles(15, 20);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (target.Center - NPC.Center + new Vector2(0, 60 * (Timer / 30) == 1 ? 1 : -1)).SafeNormalize(Vector2.UnitY) * 10f, ProjectileType<SIP_Beam>(), damage, 5f);
                        Helper.PlayPitched("RedJade/RedJadeBeam", 0.13f, 0f, NPC.Center);
                    }

                    if (Timer > 155)
                        ResetState();

                    break;

                case (int)AIStates.summon:
                    SlowDownAndGoUp();

                    if (Main.netMode != NetmodeID.Server && Timer % 10 == 0)
                        for (int i = 0; i < 6; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(20, 20), DustID.GemRuby, Vector2.Zero, 0, default, 1.3f);
                            dust.noGravity = true;
                        }

                    if (Timer % 40 == 0)
                    {
                        if (Main.npc.Count((n) => n.active && n.type == NPCType<StoneImitatorMinion>())<OwnedMinionMax())
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<StoneImitatorMinion>());
                        else
                            ResetState();

                    }

                    if (Timer>200)//防止出BUG
                        ResetState();

                    break;
                    
            }

            NPC.rotation = NPC.velocity.ToRotation() * 0.05f * NPC.direction;
            Timer++;
        }

        public void ResetState()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            do
            {
                if (exchangeState && NPC.life < NPC.lifeMax / 2)    //血量低于一半固定放小弟
                {
                    State = 4;
                    exchangeState = false;
                    break;
                }

                if (NPC.life < NPC.lifeMax / 2)//血量低于一半的攻击动作，加入了放小弟的招式
                {
                    if (State == 1)
                        State = Main.rand.Next(3) switch
                        {
                            0 => 2,
                            1 => 3,
                            2 => 4,
                            _ => 1
                        };
                    else
                        State = 1;
                    break;
                }

                if (State == 1)
                    State = Main.rand.NextBool() ? 2 : 3;
                else
                    State = 1;

            } while (false);

            Timer = 0;
            NPC.netUpdate = true;
        }

        public void SlowDownAndGoUp()
        {
            NPC.velocity.X *= 0.98f;
            NPC.velocity.Y += -0.0005f;
            if (NPC.velocity.Y > -0.8f)
                NPC.velocity.Y = -0.8f;
        }

        public int OwnedMinionMax()
        {
            if (Main.masterMode)
                return 3;

            if (Main.expertMode)
                return 3;

            return 2;
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            int frameWidth = mainTex.Width;
            int frameHeight = mainTex.Height / Main.npcFrameCount[NPC.type];
            Rectangle frameBox = new Rectangle(0, (int)yFrame * frameHeight, frameWidth, frameHeight);

            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);

            if (NPC.spriteDirection != 1)
                effects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }

        #endregion
    }

    public enum AIStates : int
    {

        explosion = 1,
        upShoot,
        magicShoot,
        summon
    }
}
