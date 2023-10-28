using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class GelFlippy : ModNPC
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        private Player Target => Main.player[NPC.target];

        private ref float Timer => ref NPC.ai[0];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 200;
            NPC.damage = 30;
            NPC.width = 50;
            NPC.height = 30;
            NPC.noGravity = true;
            NPC.HitSound = CoraliteSoundID.Fleshy_NPCHit1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = 200;
        }

        public override bool CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[1] = -300 + Main.rand.Next(-80, 80);
        }

        public override void AI()
        {
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
            Vector2 aimCenter = Target.Center + new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 2 + NPC.ai[1]) * 100, NPC.ai[1]);
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

            if (Timer > 180 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Timer = 0;
                //生成弹幕和音效，并掉血
                SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, NPC.Center);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY * Main.rand.NextFloat(1f, 3f), ModContent.ProjectileType<SmallGelBall>(), 15, 0, NPC.target);

                NPC.life -= NPC.lifeMax / 6;
                if (NPC.life < 1)
                    NPC.Kill();

                NPC.netUpdate = true;
            }

            Timer++;

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
            Vector2 origin = new Vector2(frameBox.Width / 2, frameBox.Height);
            SpriteEffects effect = SpriteEffects.FlipHorizontally;

            if (NPC.spriteDirection < 0)
                effect = SpriteEffects.None;
            if (Main.zenithWorld)
                drawColor = SlimeEmperor.BlackSlimeColor;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, drawColor, NPC.rotation, origin, 1f, effect, 0f);

            return false;
        }
    }
}
