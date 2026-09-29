using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.EoC
{
    public class EOC_Phantom : ModNPC, IDrawOverDark
    {
        public override string Texture => AssetDirectory.Vanilla + "NPC_4";

        public bool Glisten = false;
        public float GlistenValue = 0f;
        public float GlistenValue2 = 0f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(4);
            NPC.aiStyle = -1;
            NPC.alpha = 255;
            NPC.timeLeft = 300;
            NPC.boss = false;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        private static Color GetColor()
        {
            Color color = Color.DarkRed;
            color.A = 0;
            return color;
        }


        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Glisten);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Glisten = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }
            NPC TryEOC = Main.npc[(int)NPC.ai[1]];
            NPC EOC = null;
            if (TryEOC.active && TryEOC.type == NPCID.EyeofCthulhu)
            {
                EOC = TryEOC;
            }
            if (EOC == null)
            {
                NPC.life = 0;
                NPC.active = false;
                NPC.netUpdate = true;
                NPC.ai[0] = -1f;
                NPC.checkDead();
                NPC.HitEffect();
                return;
            }
            if (Glisten)
            {
                if (GlistenValue < 10f)
                    GlistenValue += 1f;
                else if (GlistenValue2 < 20f)
                    GlistenValue2 += 1f;
                else
                {
                    Glisten = false;
                    GlistenValue = 0f;
                    GlistenValue2 = 0f;
                }
            }
            NPC.GivenName = Lang.GetNPCNameValue(4);
            NPC.target = EOC.target;
            Player player = Main.player[NPC.target];
            float LifePercentage2 = EOC.life / (float)(EOC.lifeMax * 0.25f);
            float NPCRotation = Vector2.Normalize(player.Center - NPC.Center).ToRotation() - MathHelper.PiOver2;
            NPC.ai[0] = 2f;
            if (EOC.ai[1] == 0f)
            {
                Vector2 Pos = player.Center;
                Pos.X += player.Center.X > NPC.Center.X ? -2000f : 2000f;
                Pos.Y += player.Center.Y > NPC.Center.Y ? -400f : 400f;
                Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 20f;
                NPC.velocity = (NPC.velocity * 20f + Vec) / 21f;

                if (Vector2.Distance(player.Center, NPC.Center) > 1000f)
                {
                    NPC.life = 0;
                    NPC.lifeMax = 0;
                    NPC.active = false;
                    NPC.checkDead();
                    NPC.netUpdate = true;
                }
            }
            NPC.ai[2] = EOC.ai[2];
            NPC.ai[3] = EOC.ai[3];
            if (NPC.ai[2] < 120f)
            {
                Vector2 Pos = player.Center;
                Pos.X += player.Center.X > NPC.Center.X ? -800f : 800f;
                Pos.Y += player.Center.Y > NPC.Center.Y ? -400f : 400f;
                Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 20f;
                NPC.velocity = (NPC.velocity * 20f + Vec) / 21f;
            }
            else if ((int)NPC.ai[2] == 120f)
            {
                NPC.localAI[2] = EOC.localAI[2] + 120f;
                NPC.position = player.Center + new Vector2(450f, 0f).RotatedBy(MathHelper.ToRadians(NPC.localAI[2]));
                NPC.alpha = 0;
                NPC.velocity *= 0.02f;
                NPC.netUpdate = true;
            }
            else if (NPC.ai[2] < 180f)
            {
                if (NPC.ai[2] == 150f)
                {
                    Glisten = true;
                    NPC.netUpdate = true;
                }
                Vector2 Pos = player.Center + new Vector2(450f, 0f).RotatedBy(MathHelper.ToRadians(NPC.localAI[2]));
                Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 20f;
                NPC.velocity = (NPC.velocity * 5f + Vec) / 6f;
            }
            else if (NPC.ai[2] < 240f)
            {
                SpawnDust(NPC, ModContent.DustType<GlowDust_Prismatic>());
                if (NPC.ai[2] == 195f)
                {
                    Vector2 Pos = player.Center + player.velocity * 5f;
                    float Speed = 25f + (1f - LifePercentage2) * 5f;
                    if (Main.getGoodWorld)
                    {
                        Speed *= 0.85f;
                    }
                    Vector2 Vec = Vector2.Normalize(Pos - NPC.Center);
                    NPC.velocity = Vec * Speed;
                    NPC.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                }
                else if (NPC.ai[2] < 195f)
                {
                    Vector2 Pos = player.Center + new Vector2(450f, 0f).RotatedBy(MathHelper.ToRadians(NPC.localAI[2]));
                    Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 20f;
                    NPC.velocity = (NPC.velocity * 10f + Vec) / 11f;
                }
                else
                {
                    NPC.ai[2] += 1f - LifePercentage2;
                    NPC.velocity *= 1f - 0.02f * LifePercentage2;
                    NPCRotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                }
            }
            else
            {
                NPC.ai[2] = (int)(90f * (1f - LifePercentage2));
                NPC.ai[3] += 1f;
                NPC.netUpdate = true;
            }
            float RotRot = 0.25f;
            NPC.rotation = NPC.rotation.AngleLerp(NPCRotation, RotRot);
        }

        private static void SpawnDust(NPC NPC, int Type)
        {
            if (NPC.alpha > 0)
                return;
            Color color = GetColor();
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, Type, 0, 0, 100, color, 1.5f);
            Main.dust[dust].velocity *= 0.1f;
            Main.dust[dust].velocity += NPC.velocity * 0.5f;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].noLight = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1.0;
            if (NPC.frameCounter < 7.0)
            {
                NPC.frame.Y = 0;
            }
            else if (NPC.frameCounter < 14.0)
            {
                NPC.frame.Y = frameHeight;
            }
            else if (NPC.frameCounter < 21.0)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = 0;
            }
            NPC.frame.Y += frameHeight * 3;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPCID.EyeofCthulhu].Value;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rectangle = new(0, NPC.frame.Y, texture.Width, texture.Height / Main.npcFrameCount[NPC.type]);
            Vector2 Offset = NPC.position - NPC.Center;
            Vector2 Pos = NPC.position - Offset - screenPos;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 OldPos = NPC.oldPos[i] - Offset - screenPos;
                Color color = GetColor();
                color *= 1f - i / (float)NPC.oldPos.Length;
                float Scale = NPC.scale * (1f - i / (float)NPC.oldPos.Length * 0.5f);
                spriteBatch.Draw(texture, OldPos, rectangle, NPC.GetAlpha(color), NPC.oldRot[i], rectangle.Size() / 2f, Scale, spriteEffects, 0f);
            }
            for (int i = 0; i < 6; i++)
            {
                Vector2 Vec = (MathHelper.TwoPi * i / 6f).ToRotationVector2() * 4f;
                Color color = GetColor();
                spriteBatch.Draw(texture, Pos + Vec, rectangle, NPC.GetAlpha(color), NPC.rotation, rectangle.Size() / 2f, NPC.scale, spriteEffects, 0f);
            }
            Color boydColor = Lighting.GetColor((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16));
            spriteBatch.Draw(texture, Pos, rectangle, NPC.GetAlpha(boydColor), NPC.rotation, rectangle.Size() / 2f, NPC.scale, spriteEffects, 0f);
            return false;
        }

        public void DrawOverDark(SpriteBatch spriteBatch)
        {
            if (Glisten)
            {
                Color NPCColor = Color.Purple;
                if (NPC.ai[0] > 1f)
                    NPCColor = Color.Red;
                NPCColor *= GlistenValue / 10f;
                NPCColor *= 1f - GlistenValue2 / 20f;
                NPCColor.A = 0;

                Texture2D texture = NPC.GetTexture();
                SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Rectangle rectangle = new(0, NPC.frame.Y, texture.Width, texture.Height / Main.npcFrameCount[NPC.type]);
                Vector2 Pos = NPC.Center - Main.screenPosition;
                for (int i = 0; i < 6; i++)
                {
                    Vector2 Vec = (MathHelper.TwoPi * i / 6f).ToRotationVector2() * 4f;
                    spriteBatch.Draw(texture, Pos + Vec, rectangle, NPCColor, NPC.rotation, rectangle.Size() / 2f, NPC.scale, spriteEffects, 0f);
                }
                spriteBatch.Draw(texture, Pos, rectangle, Color.Black, NPC.rotation, rectangle.Size() / 2f, NPC.scale, spriteEffects, 0f);
            }
        }

    }
}

