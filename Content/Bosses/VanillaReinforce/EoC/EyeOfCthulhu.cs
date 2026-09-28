using Coralite.Content.Dusts;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Bosses.VanillaReinforce.EoC
{
    /// <summary>
    /// 来自始源的NPC，不要改动它的AI与代码结构！！！
    /// </summary>
    public class EyeOfCthulhu : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID. EyeofCthulhu);
        }


        private bool Stealth = false;
        private Vector2 TargetPos = Vector2.Zero;
        private bool TwoStage = false;

        public bool Glisten = false;
        public float GlistenValue = 0f;
        public float GlistenValue2 = 0f;

        private Color GetColor()
        {
            Color color = Color.Purple;
            if (TwoStage)
            {
                color = Color.DarkRed;
            }
            color.A = 0;
            return color;
        }

        private static float LifeMult(NPC npc)
        {
            return (float)npc.life / npc.lifeMax;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Stealth || NPC.ai[0] == 0f && NPC.ai[1] == 0f && NPC.ai[2] == 0f)
            {
                Texture2D texture = TextureAssets.Npc[NPC.type].Value;
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
                    spriteBatch.Draw(texture, OldPos, rectangle, color, NPC.oldRot[i], rectangle.Size() / 2f, Scale, spriteEffects, 0f);
                }
                for (int i = 0; i < 6; i++)
                {
                    Vector2 Vec = (MathHelper.TwoPi * i / 6f).ToRotationVector2() * 4f;
                    Color color = GetColor();
                    spriteBatch.Draw(texture, Pos + Vec, rectangle, color, NPC.rotation, rectangle.Size() / 2f, NPC.scale, spriteEffects, 0f);
                }
                Color boydColor = Lighting.GetColor((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16));
                spriteBatch.Draw(texture, Pos, rectangle, NPC.GetAlpha(boydColor), NPC.rotation, rectangle.Size() / 2f, NPC.scale, spriteEffects, 0f);
            }

            return false;
        }

        public override bool CheckDead()
        {
            return NPC.timeLeft > 0;
        }

        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(TwoStage);
            binaryWriter.Write(Glisten);
            binaryWriter.Write(GlistenValue);
            binaryWriter.Write(GlistenValue2);
            binaryWriter.Write(NPC.Originate().DamageReduction);
            binaryWriter.Write(NPC.localAI[0]);
            binaryWriter.Write(NPC.localAI[1]);
            binaryWriter.Write(NPC.localAI[2]);
            binaryWriter.Write(NPC.localAI[3]);
            binaryWriter.WriteVector2(TargetPos);
            binaryWriter.WriteVector2(NPC.position);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            TwoStage = binaryReader.ReadBoolean();
            Glisten = binaryReader.ReadBoolean();
            GlistenValue = binaryReader.ReadSingle();
            GlistenValue2 = binaryReader.ReadSingle();
            NPC.Originate().DamageReduction = binaryReader.ReadSingle();
            NPC.localAI[0] = binaryReader.ReadSingle();
            NPC.localAI[1] = binaryReader.ReadSingle();
            NPC.localAI[2] = binaryReader.ReadSingle();
            NPC.localAI[3] = binaryReader.ReadSingle();
            TargetPos = binaryReader.ReadVector2();
            NPC.position = binaryReader.ReadVector2();
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }

            Player player = Main.player[NPC.target];
            float DrakMult = 0f;
            float NPCRotation = Vector2.Normalize(player.Center - NPC.Center).ToRotation() - MathHelper.PiOver2;
            if (!TwoStage && LifeMult(NPC) < 0.5f)
            {
                float DRAdd = 0.5f;
                if (Main.getGoodWorld) DRAdd = 0.7f;
                NPC.Originate().DamageReduction = NPC.Originate().DefDamageReduction + DRAdd;
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
            if (Main.dayTime || player.dead)
            {
                NPC.velocity.X *= 0.98f;
                NPC.velocity.Y -= 0.1f;
                DrakMult = 0.5f;
                if (Vector2.Distance(NPC.Center, Main.LocalPlayer.Center) < 3000f)
                {
                    Main.LocalPlayer.Originate().TrueToDarkValue = DrakMult;
                    Main.LocalPlayer.Originate().DrakCen = Main.LocalPlayer.Center;
                }
                if (Vector2.Distance(NPC.Center, player.Center) > 2000f)
                {
                    NPC.timeLeft = 0;
                }
                NPC.rotation = NPC.rotation.AngleLerp(NPCRotation, 0.1f);
                return false;
            }
            NPC.timeLeft = 60;
            if (NPC.ai[0] == 0f)
            {
                SpawnDust(NPC, ModContent.DustType<GlowDust_Circle>());
                if (NPC.ai[1] == 0f)
                {
                    OpeningAnimation(NPC, player, ref DrakMult);
                }
                else if (NPC.ai[1] == 1f)
                {
                    Attack_EyeLaser(NPC, player);
                }
                else if (NPC.ai[1] == 2f)
                {
                    Attack_EyeImpact(NPC, player);
                }
                else if (NPC.ai[1] == 3f)
                {
                    Attack_LaserScattering(NPC, player, ref DrakMult, ref NPCRotation);
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                SpawnDust(NPC, ModContent.DustType<GlowDust_Circle>());
                Attack_EyeFlock(NPC, player, ref DrakMult);
            }
            else
            {
                float LifePercentage = NPC.life / (float)(NPC.lifeMax * 0.5f);
                DrakMult = (1f - LifePercentage) * 0.5f;
                if (NPC.ai[1] == 0f)
                {
                    Attack_Dash(NPC, player, ref NPCRotation);
                }
                else
                {
                    Attack_DrakSprint(NPC, player, ref DrakMult, ref NPCRotation);
                }
            }
            if (Vector2.Distance(NPC.Center, Main.LocalPlayer.Center) < 3000f)
            {
                if(Main.getGoodWorld)
                {
                    float Value = 0.25f + 0.25f * (1f - LifeMult(NPC));
                    if (DrakMult < Value) 
                        DrakMult = Value;
                }
                Main.LocalPlayer.Originate().TrueToDarkValue = DrakMult;
                Main.LocalPlayer.Originate().DrakCen = Main.LocalPlayer.Center;
            }
            float RotRot = 0.1f;
            if (TwoStage)
                RotRot = 0.25f;
            NPC.rotation = NPC.rotation.AngleLerp(NPCRotation, RotRot);
        }
        public override void FindFrame(int frameHeight)
        {
            if (Main.masterMode)
            {
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
                if (TwoStage)
                {
                    NPC.frame.Y += frameHeight * 3;
                }
            }
        }
       
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (Main.LocalPlayer.Originate().DarkValue > 0.45f) 
                return false;
            return Stealth;
        }

        public override void BossHeadSlot( ref int index)
        {
            if (Main.LocalPlayer.Originate().DarkValue > 0.45f)
                index = -1;
            if (!Stealth) 
                index = -1;
        }

        private void OpeningAnimation(NPC NPC, Player player, ref float DrakValue)
        {
            float DrakV = 0.9f;
            if (NPC.ai[2] > 540f)
            {
                DrakValue = DrakV - DrakV * (NPC.ai[2] - 540f) / 60f;
            }
            else if (NPC.ai[2] < 300f)
            {
                NPC.dontTakeDamage = true;
                DrakValue = DrakV * NPC.ai[2] / 300f;
            }
            else
            {
                DrakValue = DrakV;
            }
            if (NPC.ai[2] == 510f)
            {
                Glisten = true;
                Stealth = true;
                NPC.dontTakeDamage = false;
            }
            NPC.ai[2] += 1f;
            if (NPC.ai[2] < 600f)
            {
                NPC.ai[2] += 1f;
            }
            else
            {
                NPC.ai[1] = 1f;
                NPC.ai[2] = 0f;
                NPC.velocity *= 0f;
                SoundEngine.PlaySound(SoundID.Roar, player.Center);
                PunchCameraModifier modifier = new(NPC.Center, Main.rand.NextVector2Unit(), 5f, 10f, 60);
                Main.instance.CameraModifiers.Add(modifier);
                NPC.netUpdate = true;
            }
            Vector2 Pos = player.Center;
            Pos.X += player.Center.X > NPC.Center.X ? -225f : 225f;
            Pos.Y -= 275f;
            Vector2 Vec = (Pos - NPC.Center).SafeNormalize(Vector2.UnitX);
            Vec *= MathHelper.Clamp(Vector2.Distance(Pos, NPC.Center) / 5f, 1f, 30f);
            NPC.velocity = Vec;
        }

        private static void Attack_EyeLaser(NPC NPC, Player player)
        {
            NPC.ai[2] += 1f;
            if (NPC.ai[2] < 240f)
            {
                if (Main.getGoodWorld) NPC.ai[2] += 1f;
                Vector2 Pos = player.Center;
                Pos.X += player.Center.X > NPC.Center.X ? -450f : 450f;
                Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 10f;
                NPC.velocity = (NPC.velocity * 30f + Vec) / 31f;
            }
            else if (NPC.ai[2] < 300f)
            {
                Vector2 Pos = player.Center;
                Pos.X += player.Center.X > NPC.Center.X ? -100f : 100f;
                Pos.Y += player.Center.Y > NPC.Center.Y ? -450f : 450f;
                Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 20f;
                NPC.velocity = (NPC.velocity * 20f + Vec) / 21f;
                float Interval = 10f;
                if (Main.getGoodWorld) Interval = 5f;
                if (NPC.ai[2] % Interval == 0f)
                {
                    Pos = player.Center;
                    Vec = Vector2.Normalize(Pos - NPC.Center) * 4f;
                    Vec = Vec.RotatedByRandom(0.15f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vec * 8f, Vec, ModContent.ProjectileType<EyeShoot_Pro>(), NPC.damage.IntMult(0.25f), 0f, 0);
                }
            }
            else
            {
                if (LifeMult(NPC) < 0.5f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
                float Stack = 4f;
                if (Main.getGoodWorld)
                    Stack = 6f;
                if (NPC.ai[3] > Stack)
                {
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
                else
                {
                    NPC.ai[1] = 2f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] += 1f;
                    NPC.netUpdate = true;
                }
            }
        }

        private static void Attack_EyeImpact(NPC NPC, Player player)
        {
            NPC.ai[2] += 1f;
            if (NPC.ai[2] > 120f)
            {
                float Interval = 45f;
                if (Main.getGoodWorld) Interval = 60f;
                if (NPC.ai[2] % Interval == 0f)
                {
                    Vector2 Vel = Vector2.Normalize(player.Center - NPC.Center) * 8f;
                    int Star = 0, End = 0;
                    if(Main.getGoodWorld)
                    {
                        Star = -1;
                        End = 1;
                    }
                    for(int i = Star; i <= End; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int a = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ServantofCthulhu>());
                            Main.npc[a].velocity = Vel.RotatedBy(MathHelper.PiOver4 * i);
                            Main.npc[a].ai[0] = 90f;
                            Main.npc[a].ai[1] = NPC.whoAmI;
                            if (Main.netMode == NetmodeID.Server && a < 200)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, a);
                            }
                        }
                    }
                    SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);
                }
            }
            if (NPC.ai[2] > 300f)
            {
                if (LifeMult(NPC) < 0.5f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
                float Stack = 4f;
                if (Main.getGoodWorld)
                    Stack = 6f;
                if (NPC.ai[3] > Stack)
                {
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
                else
                {
                    NPC.ai[1] = 1f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] += 1f;
                    NPC.netUpdate = true;
                }
            }
            Vector2 Pos = player.Center;
            Pos.X += player.Center.X > NPC.Center.X ? -300f : 300f;
            Pos.Y -= 200f;
            Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 10f;
            NPC.velocity = (NPC.velocity * 30f + Vec) / 31f;
        }

        private void Attack_LaserScattering(NPC NPC, Player player, ref float DrakMult, ref float NPCRotation)
        {
            NPC.ai[2] += 1f;
            if (NPC.ai[2] < 120f)
            {
                if (NPC.ai[3] == 0f)
                {
                    DrakMult = 0.9f * NPC.ai[2] / 120f;
                }
                else
                {
                    DrakMult = 0.3f + 0.6f * NPC.ai[2] / 120f;
                }
                Vector2 Pos = player.Center;
                Pos.X += player.Center.X > NPC.Center.X ? -800f : 800f;
                Pos.Y -= 400f;
                Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 10f;
                NPC.velocity = (NPC.velocity * 30f + Vec) / 31f;
            }
            else if (NPC.ai[2] == 120f)
            {
                DrakMult = 0.9f;
                NPC.position = player.Center + Main.rand.NextVector2Unit() * 400f;
                NPC.velocity *= 0.02f;
                NPC.netUpdate = true;
            }
            else if (NPC.ai[2] < 180f)
            {
                DrakMult = 0.9f;
                if (NPC.ai[2] == 150f)
                {
                    NPC.localAI[1] = Main.rand.NextBool() ? 1f : -1f;
                    Glisten = true;
                    NPC.netUpdate = true;
                }
                else if (NPC.ai[2] < 150f)
                {
                    NPC.position += player.velocity;
                }
            }
            else if (NPC.ai[2] < 210f)
            {
                float Mult = (NPC.ai[2] - 180f) / 30f;

                TargetPos = player.Center;
                float rot = MathHelper.Pi * 0.45f * NPC.localAI[1] * -1f * Mult;
                Vector2 ToVec = Vector2.Normalize(TargetPos - NPC.Center).RotatedBy(rot);
                NPCRotation = ToVec.ToRotation() - MathHelper.PiOver2;

                DrakMult = 0.9f - 0.6f * Mult;
            }
            else if (NPC.ai[2] == 210f)
            {
                TargetPos = player.Center;
                float rot = MathHelper.Pi * 0.45f * NPC.localAI[1] * -1f;
                Vector2 ToVec = Vector2.Normalize(TargetPos - NPC.Center).RotatedBy(rot);
                NPCRotation = ToVec.ToRotation() - MathHelper.PiOver2;

                DrakMult = 0.3f;
                TargetPos = player.Center + player.velocity * 10f;
                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                NPC.netUpdate = true;
            }
            else if (NPC.ai[2] <= 330f)
            {
                DrakMult = 0.3f;
                float Rot = 0.4f;
                if (Main.getGoodWorld) Rot = 0.5f;
                float rot = MathHelper.Pi * Rot * NPC.localAI[1] * ((NPC.ai[2] - 270f) / 60f);
                Vector2 ToVec = Vector2.Normalize(TargetPos - NPC.Center).RotatedBy(rot);
                NPCRotation = ToVec.ToRotation() - MathHelper.PiOver2;
                if (NPC.ai[2] % 2 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + ToVec * 70f, ToVec * 3f, ModContent.ProjectileType<EyeShoot_Pro>(), NPC.damage.IntMult(0.25f), 0f, 0);
                }
            }
            else if (NPC.ai[3] > 3f)
            {
                if (LifeMult(NPC) < 0.5f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.localAI[1] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
                NPC.ai[1] = 2f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.localAI[1] = 0f;
                NPC.netUpdate = true;
            }
            else
            {
                if (NPC.ai[3] != 3f) DrakMult = 0.3f;
                NPC.ai[2] = 0f;
                NPC.ai[3] += 1f;
            }
        }

        private void Attack_EyeFlock(NPC NPC, Player player, ref float DrakMult)
        {
            if (NPC.ai[1] == 0f)
            {
                if (NPC.ai[3] == 0f)
                {
                    NPC.ai[3] = (NPC.Center - player.Center).SafeNormalize(Vector2.UnitX).ToRotation();
                }
                float Dis = NPC.ai[2];
                if (Dis > 150f) Dis = 150f;
                Vector2 Pos = player.Center + new Vector2(350f + Dis * 2f, 0f).RotatedBy(NPC.ai[3] + MathHelper.ToRadians(NPC.ai[2]));
                Vector2 Vec = (Pos - NPC.Center).SafeNormalize(Vector2.UnitX);
                Vec *= MathHelper.Clamp(Vector2.Distance(Pos, NPC.Center) / 5f, 1f, 30f);
                NPC.velocity = Vec;
                NPC.ai[2] += 1f;
                if (NPC.ai[2] < 90f)
                {
                    DrakMult = 0.5f * NPC.ai[2] / 90f;
                }
                else if (NPC.ai[2] == 90f)
                {
                    NPC.dontTakeDamage = true;
                    DrakMult = 0.5f;
                }
                else if (NPC.ai[2] <= 360f)
                {
                    if (NPC.ai[2] == 120f || NPC.ai[2] == 240f || NPC.ai[2] == 360f)
                    {
                        Vector2 Vel = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 4f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int a = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<FuriousEye>());
                            Main.npc[a].velocity.X = Vel.X;
                            Main.npc[a].velocity.Y = Vel.Y;
                            Main.npc[a].ai[2] = NPC.ai[2];
                            if (Main.netMode == NetmodeID.Server && a < 200)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, a);
                            }
                        }
                        SoundEngine.PlaySound(SoundID.NPCHit1, NPC.Center);
                    }
                    DrakMult = 0.5f;
                }
                else
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<FuriousEye>()))
                    {
                        NPC.ai[1] = 1f;
                        NPC.ai[3] = 0f;
                        TwoStage = true;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        if (NPC.ai[2] % 90f == 0f)
                        {
                            Glisten = true;
                        }
                    }
                    DrakMult = 0.5f;
                }
            }
            else
            {
                if (NPC.ai[3] == 0f)
                {
                    NPC.ai[2] = (NPC.Center - player.Center).SafeNormalize(Vector2.UnitX).ToRotation();
                }
                Vector2 Pos = player.Center + new Vector2(600f, 0f).RotatedBy(NPC.ai[2]);
                if (NPC.ai[3] > 60f)
                    Pos = player.Center + new Vector2(300f, 0f).RotatedBy(NPC.ai[2]);
                Vector2 Vec = (Pos - NPC.Center).SafeNormalize(Vector2.UnitX);
                Vec *= MathHelper.Clamp(Vector2.Distance(Pos, NPC.Center) / 5f, 1f, 30f);
                NPC.velocity = Vec;
                NPC.ai[3] += 1f;
                if (NPC.ai[3] < 60f)
                {
                    float Mult = NPC.ai[3] / 60f;
                    DrakMult = 0.5f + 0.4f * Mult;
                }
                else
                {
                    if (NPC.ai[3] < 90f)
                    {
                        float Mult = 1f - (NPC.ai[3] - 60f) / 30f;
                        DrakMult = 0.9f * Mult;
                    }
                    if (NPC.ai[3] == 90f)
                    {
                        SoundEngine.PlaySound(SoundID.ForceRoarPitched, player.Center);
                    }
                    Vector2 Cen = NPC.Center + (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * NPC.width * 0.66f;
                    if (NPC.ai[3] > 90f)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 Vel = Vector2.One.RotateRandom(MathHelper.Pi) * Main.rand.NextFloat(60f);
                            if (Main.rand.NextBool(4)) Vel *= 1.5f;
                            else if (Main.rand.NextBool(8)) Vel *= 2f;
                            int dust = Dust.NewDust(Cen, 0, 0, ModContent.DustType<GlowDust_Line>(), 0, 0, 100, Color.Gray, 1.5f);
                            Main.dust[dust].position = Cen;
                            Main.dust[dust].velocity = Vel;
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].scale *= 1f;
                        }
                    }
                    if (NPC.ai[3] == 90f || NPC.ai[3] == 100f || NPC.ai[3] == 110f)
                    {
                        NPC.velocity *= 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), Cen, Vector2.Zero, ModContent.ProjectileType<FightingRoar>(), 0, 0, 0, 0f, NPC.whoAmI);
                        PunchCameraModifier modifier = new(NPC.Center, Main.rand.NextVector2Unit(), 5f, 10f, 60);
                        Main.instance.CameraModifiers.Add(modifier);
                    }
                }
                if (NPC.ai[3] >= 120f)
                {
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.velocity *= 0f;
                    NPC.dontTakeDamage = false;
                    NPC.damage = NPC.defDamage.IntMult(1.5f);
                    NPC.defense = NPC.defDefense.IntMult(0.5f);
                    NPC.Originate().DamageReduction = NPC.Originate().DefDamageReduction + 0.1f;
                    NPC.netUpdate = true;
                }
            }
        }

        private void Attack_Dash(NPC NPC, Player player, ref float NPCRotation)
        {
            float LifePercentage = NPC.life / (float)(NPC.lifeMax * 0.5f);
            if (NPC.ai[3] == 0f)
            {
                NPC.ai[2] += 1f;
                Vector2 Pos = player.Center;
                Pos.X += player.Center.X > NPC.Center.X ? -300f : 300f;
                Pos.Y += player.Center.Y > NPC.Center.Y ? -200f : 200f;
                Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 10f;
                NPC.velocity = (NPC.velocity * 30f + Vec) / 31f;
                SpawnDust(NPC, ModContent.DustType<GlowDust_Circle>());
                if (NPC.ai[2] > 120f)
                {
                    NPC.ai[3] = 1f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[3] == 1f)
            {
                NPC.ai[2] += 1f;
                if (NPC.ai[2] > 15f + 30f * LifePercentage)
                {
                    Vector2 PlayerPos = player.Center + new Vector2(0f, Main.rand.NextFloat(-50f, 50f));
                    Vector2 Vec = Vector2.Normalize(PlayerPos - NPC.Center) * 28f;
                    float VecX = Vec.X;
                    float VecY = Vec.Y;
                    while (Math.Abs(VecX) > 20f)
                    {
                        VecX *= 0.95f;
                    }
                    while (Math.Abs(VecY) < 20f)
                    {
                        VecY *= 1.05f;
                    }
                    NPC.velocity = new Vector2(VecX, VecY);
                    NPC.ai[3] = 2f;
                    NPC.ai[2] = 0f;
                    if (Main.getGoodWorld) 
                        NPC.ai[2] = 5f;
                    NPC.localAI[2] += 1f;
                   Helper. CircularDust(NPC.Center + Vec, NPC.velocity.ToRotation(), ModContent.DustType<GlowDust_Circle>(), 48, new Vector2(5f, 15f), GetColor(), 1.5f, -NPC.velocity * 0.5f);
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
                }
                else
                {
                    Vector2 Pos = player.Center;
                    Pos.X += player.Center.X > NPC.Center.X ? -300f : 300f;
                    Pos.Y += player.Center.Y > NPC.Center.Y ? -200f : 200f;
                    Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 10f;
                    NPC.velocity = (NPC.velocity * 20f + Vec) / 21f;
                    SpawnDust(NPC, ModContent.DustType<GlowDust_Circle>());
                }
                NPC.netUpdate = true;
            }
            else if (NPC.ai[3] == 2f)
            {
                NPC.ai[2] += 1f;
                NPCRotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                NPC.velocity *= 0.975f;
                SpawnDust(NPC, ModContent.DustType<GlowDust_Prismatic>());
                if (NPC.ai[2] > 15f + 30f * LifePercentage)
                {
                    if (NPC.localAI[2] > 3f + 9f * (1f - LifePercentage))
                    {
                        NPC.ai[3] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.localAI[2] = 0f;
                        if (LifePercentage < 0.5f && NPC.localAI[3] >= 2f)
                        {
                            NPC.ai[1] = 1f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            NPC.localAI[3] = 0f;
                            NPC.netUpdate = true;
                            return;
                        }
                        NPC.localAI[3] += 1f;
                    }
                    else
                    {
                        NPC.ai[3] = 1f;
                        NPC.ai[2] = 0f;
                        if (Main.getGoodWorld) NPC.ai[2] = 5f;
                    }
                }
            }
        }

        private void Attack_DrakSprint(NPC NPC, Player player, ref float DrakMult, ref float NPCRotation)
        {
            float LifePercentage2 = NPC.life / (float)(NPC.lifeMax * 0.25f);
            if (NPC.ai[2] == 0f && NPC.ai[3] == 0f && Main.getGoodWorld)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int a = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<EOC_Phantom>());
                    Main.npc[a].ai[1] = NPC.whoAmI;
                    Main.npc[a].netUpdate = true;
                    if (Main.netMode == NetmodeID.Server && a < 200)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, a);
                    }
                }
            }
            NPC.ai[2] += 1f;
            if (NPC.ai[2] < 120f)
            {
                if (NPC.ai[3] == 0f)
                {
                    DrakMult = Math.Max(DrakMult, 0.75f * NPC.ai[2] / 120f);
                }
                else
                {
                    DrakMult = 0.3f + 0.45f * NPC.ai[2] / 120f;
                }
                Vector2 Pos = player.Center;
                Pos.X += player.Center.X > NPC.Center.X ? -800f : 800f;
                Pos.Y += player.Center.Y > NPC.Center.Y ? -400f : 400f;
                Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 20f;
                NPC.velocity = (NPC.velocity * 10f + Vec) / 11f;
            }
            else if ((int)NPC.ai[2] == 120f)
            {
                DrakMult = 0.75f;
                NPC.localAI[2] = Main.rand.NextFloat(30f, 150f);
                if (Main.rand.NextBool(2))
                {
                    NPC.localAI[2] += 180f;
                }
                NPC.position = player.Center + new Vector2(450f, 0f).RotatedBy(MathHelper.ToRadians(NPC.localAI[2]));
                NPC.velocity *= 0.02f;
                NPC.netUpdate = true;
            }
            else if (NPC.ai[2] < 180f)
            {
                DrakMult = 0.75f;
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
                    if(Main.getGoodWorld)
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
                if (LifePercentage2 < 0.4f)
                {
                    if (NPC.ai[2] < 210f)
                        DrakMult = 0.75f - 0.15f * (NPC.ai[2] - 180f) / 30f;
                    else 
                        DrakMult = 0.6f;
                }
                else
                {
                    if (NPC.ai[2] < 210f)
                        DrakMult = 0.75f - 0.45f * (NPC.ai[2] - 180f) / 30f;
                    else 
                        DrakMult = 0.3f;
                }
            }
            else
            {
                if (NPC.ai[3] > 1f + 5f * (1f - LifePercentage2))
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.localAI[2] = 0f;
                    NPC.localAI[3] = NPC.ai[3] / 2f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
                NPC.ai[2] = (int)(90f * (1f - LifePercentage2));
                NPC.ai[3] += 1f;
                NPC.netUpdate = true;
            }
        }

        private void SpawnDust(NPC NPC, int Type)
        {
            if (!Stealth)
                return;
            Color color = GetColor();
            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, Type, 0, 0, 100, color, 1.5f);
            Main.dust[dust].velocity *= 0.1f;
            Main.dust[dust].velocity += NPC.velocity * 0.5f;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].noLight = true;
        }
    }
}