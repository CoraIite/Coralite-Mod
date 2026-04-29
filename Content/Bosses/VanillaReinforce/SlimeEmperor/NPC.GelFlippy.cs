using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class GelFlippy : ModNPC
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        private Player Target => Main.player[NPC.target];

        private ref float Timer => ref NPC.ai[0];
        private ref float XMove => ref NPC.ai[2];
        private ref float GelSpawnTime => ref NPC.ai[3];
        private bool span;
        private bool Bonused;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.CannotDropSouls[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 200;
            NPC.damage = 30;
            NPC.width = 50;
            NPC.height = 30;
            NPC.noGravity = true;
            NPC.HitSound = CoraliteSoundID.Fleshy_NPCHit1;
            NPC.SpawnedFromStatue = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = 200;
            if (((Slime1Knowledge)CoraliteContent.GetKnowledge<Slime1Knowledge>()).DangerousSet(Slime1Knowledge.Dangerous.FlippyBonus_1))
                NPC.lifeMax = (int)(NPC.lifeMax * 1.25f);
        }

        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public void Initialize()
        {
            NPC.ai[1] = -300 + Main.rand.Next(-80, 80);
            XMove = 100;
            GelSpawnTime = 180;
            if (((Slime1Knowledge)CoraliteContent.GetKnowledge<Slime1Knowledge>()).DangerousSet(Slime1Knowledge.Dangerous.FlippyBonus_S_2))
            {
                XMove = 300;
                GelSpawnTime = 120;
                Bonused = true;
            }
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }

            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 2000)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 2000)//没有玩家存活时离开
                {
                    NPC.velocity.Y -= 0.2f;
                    NPC.EncourageDespawn(10);
                    return;
                }
            }

            //尝试飞到玩家上方并隔一段时间滴出一个小凝胶球
            Vector2 aimCenter = Target.Center + new Vector2(MathF.Sin((Timer * 0.02f) + NPC.ai[1]) * XMove, NPC.ai[1]);
            float length2TargetX = aimCenter.X - NPC.Center.X;
            float length2TargetY = aimCenter.Y - NPC.Center.Y;
            bool shouldMoveX = Math.Abs(length2TargetX) > 40;
            bool shouldMoveY = Math.Abs(length2TargetY) > 10;
            NPC.direction = NPC.spriteDirection = Math.Abs(length2TargetX) < 10 ? 1 : Math.Sign(length2TargetX);
            NPC.directionY = Math.Sign(length2TargetY);

            if (shouldMoveX)
                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 8, 0.5f, 0.5f, 0.98f);
            else
                NPC.velocity.X *= 0.98f;

            if (shouldMoveY)
                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6, 0.2f, 0.3f, 0.98f);
            else
                NPC.velocity.X *= 0.97f;

            float targetRot = NPC.velocity.Length() * 0.04f * NPC.direction;
            NPC.rotation = NPC.rotation.AngleTowards(targetRot, 0.01f);

            if (Timer % GelSpawnTime == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                //生成弹幕和音效，并掉血
                SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, NPC.Center);
                Vector2 speed = Vector2.UnitY * Main.rand.NextFloat(1f, 3f);

                if (!((Slime1Knowledge)CoraliteContent.GetKnowledge<Slime1Knowledge>()).DangerousSet(Slime1Knowledge.Dangerous.FlippyBonus_1))
                {
                    NPC.life -= NPC.lifeMax / 6;
                    if (NPC.life < 1)
                        NPC.Kill();
                }
                if (((Slime1Knowledge)CoraliteContent.GetKnowledge<Slime1Knowledge>()).DangerousSet(Slime1Knowledge.Dangerous.FlippyBonus_S_2))
                    speed = Vector2.UnitY.RotatedBy(MathF.Sin(Timer / GelSpawnTime * MathHelper.PiOver2) * MathHelper.PiOver4) *4;

                NPC.NewProjectileDirectInAI<SmallGelBall>(NPC.Center, speed, Helper.GetProjDamage(40, 55, 70), 0, NPC.target);

                NPC.netUpdate = true;
            }

            Timer++;
            if (Timer > GelSpawnTime * 12)
                NPC.Kill();

            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y++;
                if (NPC.frame.Y > 3)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            Rectangle frameBox = mainTex.Frame(1, Main.npcFrameCount[Type], 0, NPC.frame.Y);
            Vector2 origin = new(frameBox.Width / 2, frameBox.Height);
            SpriteEffects effect = SpriteEffects.FlipHorizontally;
            Vector2 pos = NPC.Center - screenPos;
            if (NPC.spriteDirection < 0)
                effect = SpriteEffects.None;
            if (Main.zenithWorld)
                drawColor = SlimeEmperor.BlackSlimeColor;

            if (Bonused)//超强化的特殊绘制
            {
                Color c = Color.Red;
                c.A = 0;
                for (int i = 0; i < 3; i++)
                {
                    spriteBatch.Draw(mainTex, pos + (i * MathHelper.TwoPi / 3 + (int)Main.timeForVisualEffects * 0.1f).ToRotationVector2() * 3f, frameBox, c, NPC.rotation, origin, 1f, effect, 0f);
                }
            }

            spriteBatch.Draw(mainTex, pos, frameBox, drawColor, NPC.rotation, origin, 1f, effect, 0f);
            if (Bonused)//超强化的特殊绘制
                spriteBatch.Draw(mainTex, pos, frameBox, Color.Red with { A = 50 }, NPC.rotation, origin, 1f, effect, 0f);


            return false;
        }
    }
}
