using Coralite.Content.Dusts;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;

namespace Coralite.Content.Bosses.Originate.Pioneer
{
    //[AutoloadBossHead]
    //public class Pioneer : ModNPC
    //{
    //    public override string Texture => AssetDirectory.Pioneer;

    //    public override void Load()
    //    {
    //        string Name = "Pioneer_Extra";
    //        OriginateMod.Extra.Add(Name, ModContent.Request<Texture2D>("OriginateMod/Content/NPCs/Pioneer/" + Name));
    //    }
    //    public override void SetStaticDefaults()
    //    {
    //        Main.npcFrameCount[NPC.type] = 1;
    //        NPCID.Sets.TrailingMode[NPC.type] = 3;
    //        NPCID.Sets.TrailCacheLength[NPC.type] = 6;
    //    }
    //    public override void SetDefaults()
    //    {
    //        NPC.width = 62;
    //        NPC.height = 128;
    //        NPC.scale = 1f;
    //        NPC.alpha = 0;
    //        NPC.npcSlots = 0f;
    //        NPC.value = 10000f;
    //        NPC.aiStyle = -1;
    //        NPC.damage = 20;
    //        NPC.defense = 15;
    //        NPC.lifeMax = 2000;
    //        NPC.knockBackResist = 0f;
    //        NPC.boss = true;
    //        NPC.noGravity = true;
    //        NPC.dontTakeDamage = false;
    //        NPC.noTileCollide = true;
    //        NPC.HitSound = SoundID.NPCHit4;
    //        NPC.DeathSound = SoundID.NPCDeath14;
    //        NPC.Originate().HitDamage = false;
    //    }
    //    public override void BossLoot(ref string name, ref int potionType)
    //    {
    //        potionType = ItemID.Heart;
    //    }
    //    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    //    {
    //        NPC.lifeMax = NPC.lifeMax.IntMult(bossAdjustment * 0.85f * balance);
    //        NPC.damage = NPC.damage.IntMult(bossAdjustment * 0.85f);
    //    }
    //    private static Color GetColor()
    //    {
    //        Color color = Color.Orange;
    //        color.A = 0;
    //        return color;
    //    }
    //    private static Color DustColor()
    //    {
    //        Color color = new(35, 160, 200);
    //        return color;
    //    }
    //    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    //    {
    //        Texture2D texture = TextureAssets.Npc[NPC.type].Value;
    //        Rectangle rectangle = new(0, NPC.frame.Y, texture.Width, texture.Height / Main.npcFrameCount[NPC.type]);
    //        Vector2 Offset = NPC.position - NPC.Center;
    //        Vector2 Pos = NPC.position - Offset - screenPos;
    //        for (int i = 0; i < NPC.oldPos.Length; i++)
    //        {
    //            Vector2 OldPos = NPC.oldPos[i] - Offset - screenPos;
    //            Color color = GetColor();
    //            color *= 1f - i / (float)NPC.oldPos.Length;
    //            float Scale = NPC.scale * (1f - i / (float)NPC.oldPos.Length * 0.5f);
    //            spriteBatch.Draw(texture, OldPos, rectangle, NPC.GetAlpha(color), NPC.oldRot[i], rectangle.Size() / 2f, Scale * 1.25f, SpriteEffects.None, 0f);
    //        }
    //        for (int i = 0; i < 8; i++)
    //        {
    //            Vector2 Vec = (MathHelper.TwoPi * i / 8f + OriginateWorld.Time * 0.02f).ToRotationVector2() * 3f;
    //            Color color = GetColor();
    //            spriteBatch.Draw(texture, Pos + Vec, rectangle, NPC.GetAlpha(color), NPC.rotation, rectangle.Size() / 2f, NPC.scale * 1.25f, SpriteEffects.None, 0f);
    //        }
    //        spriteBatch.Draw(texture, Pos, rectangle, NPC.GetAlpha(drawColor), NPC.rotation, rectangle.Size() / 2f, NPC.scale * 1.25f, SpriteEffects.None, 0f);
    //        {
    //            texture = OriginateMod.Extra.GetValueOrDefault(GetType().Name + "_Extra").Value;
    //            rectangle = new(0, NPC.frame.Y, texture.Width, texture.Height / Main.npcFrameCount[NPC.type]);
    //            spriteBatch.Draw(texture, Pos, rectangle, Color.White, NPC.rotation, rectangle.Size() / 2f, NPC.scale * 1.25f, SpriteEffects.None, 0f);
    //        }
    //        if (NPC.ai[0] == 0f && NPC.ai[1] == 2f)
    //        {
    //            Player player = Main.player[NPC.target];
    //            List<VertexInfo2> Vertex = [];
    //            List<VertexInfo2> Vertex2 = [];
    //            float Max = 100f;
    //            float Mult = NPC.ai[2] / 90f;
    //            if (NPC.ai[2] >= 90f)
    //                Mult = 1f - (NPC.ai[2] - 90f) / 90f;
    //            float X = player.width * 4f;
    //            float Y = player.height * 2f;
    //            for (float i = 0; i < Max; i += 1f)
    //            {
    //                Vector2 CenOffset = new(0f, Y - Y * 2f * Mult);
    //                Vector2 Cen = Vector2.Lerp(ShootPos, player.Center + CenOffset, i / Max) - screenPos;
    //                Color color = DustColor();
    //                Vertex.Add(new VertexInfo2(Cen + Vector2.UnitX * X * i / Max * NPC.ai[3], new Vector3(i / (float)Max, 0f, 1f), color));
    //                Vertex.Add(new VertexInfo2(Cen - Vector2.UnitX * X * i / Max * NPC.ai[3], new Vector3(i / (float)Max, 1f, 1f), color));
    //            }
    //            for (float i = 0; i < Max; i += 1f)
    //            {
    //                Vector2 CenOffset = new(0f, Y - Y * 2f * Mult);
    //                Vector2 Cen = Vector2.Lerp(ShootPos, player.Center + CenOffset, i / Max) - screenPos;
    //                Color color = DustColor();
    //                Vertex2.Add(new VertexInfo2(Cen - Vector2.UnitX * X * i / Max * NPC.ai[3], new Vector3(i / Max, 0f, 1f), color));
    //                Vertex2.Add(new VertexInfo2(Cen + Vector2.UnitX * X * i / Max * NPC.ai[3], new Vector3(i / (float)Max, 1f, 1f), color));
    //            }
    //            Main.spriteBatch.End();
    //            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp,
    //                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
    //            Main.graphics.GraphicsDevice.Textures[0] = OriginateMod.Extra.GetValueOrDefault("Blade_Old").Value;
    //            if (Vertex.Count >= 3)
    //            {
    //                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertex.ToArray(), 0, Vertex.Count - 2);
    //                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertex.ToArray(), 0, Vertex.Count - 2);
    //                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertex2.ToArray(), 0, Vertex2.Count - 2);
    //                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertex2.ToArray(), 0, Vertex2.Count - 2);
    //            }
    //            Main.spriteBatch.End();
    //            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
    //                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
    //        }
    //        if (NPC.ai[0] == 0f && NPC.ai[1] == 3f || NPC.Originate().ModAI[3] > 0f && NPC.ai[1] != -2f)
    //        {
    //            Color color = DustColor();
    //            color *= 0.75f;
    //            color.A = 0;
    //            float Rot = NPC.Originate().ModAI[3] * 0.02f;
    //            float Scale = NPC.Originate().ModAI[4];
    //            Texture2D Tex = OriginateMod.Extra.GetValueOrDefault("Extra_Strike").Value;
    //            Rectangle Rect = new(0, 0, Tex.Width, Tex.Height);
    //            for (int i = 0; i < 2; i++)
    //            {
    //                Main.spriteBatch.Draw(Tex, ShootPos - screenPos, Rect, color, Rot, Rect.Size() / 2f, new Vector2(1f, 4f) * Scale, SpriteEffects.None, 0f);
    //                Main.spriteBatch.Draw(Tex, ShootPos - screenPos, Rect, color, Rot, Rect.Size() / 2f, new Vector2(4f, 1f) * Scale, SpriteEffects.None, 0f);
    //            }
    //            Tex = TextureAssets.GlowMask[239].Value;
    //            Rect = new(0, 0, Tex.Width, Tex.Height);
    //            Scale = NPC.Originate().ModAI[3] / 60f;
    //            for (int i = 0; i < 3; i++)
    //            {
    //                Scale *= Scale;
    //                color = DustColor();
    //                color *= 0.75f;
    //                color *= Scale;
    //                color.A = 0;
    //                float SS = 1f - Scale;
    //                Main.spriteBatch.Draw(Tex, ShootPos - screenPos, Rect, color, 0f, Rect.Size() / 2f, SS * (1f + i * (1f + i * 0.5f)) * 2f, SpriteEffects.None, 0f);
    //            }
    //        }
    //        return false;
    //    }
    //    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    //    {
    //        if (!NPC.Originate().HitDamage)
    //        {
    //            return false;
    //        }
    //        return base.CanHitPlayer(target, ref cooldownSlot);
    //    }
    //    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
    //    {
    //        if(!NPC.AnyNPCs(ModContent.NPCType<EnergyShield>()))
    //        {
    //            if(projectile.type == ModContent.ProjectileType<Pioneer_Pro6>() && NPC.ai[1] != -2f)
    //            {
    //                NPC.ai[1] = -2f;
    //                NPC.ai[2] = 0f;
    //                NPC.ai[3] = 0f;
    //                NPC.Originate().ModAI[1] = 0f;
    //                NPC.Originate().ModAI[2] = 0f;
    //                NPC.Originate().ModAI[3] = 0f;
    //                NPC.noTileCollide = false;
    //            }
    //        }
    //    }
    //    public override bool CheckActive()
    //    {
    //        return false;
    //    }
    //    public override void HitEffect(NPC.HitInfo hit)
    //    {
    //        if (NPC.life > 0)
    //        {
    //            for (int i = 0; i < hit.Damage * 0.25f; i++)
    //            {
    //                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust_Circle>(), hit.HitDirection * 10f, -1f, 100, GetColor(), 0.75f);
    //                dust.noGravity = true;
    //            }
    //            return;
    //        }
    //        base.HitEffect(hit);
    //    }
    //    public override void ModifyNPCLoot(NPCLoot npcLoot)
    //    {
    //        OriginateWorld.downedPioneer = true;
    //        npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBag_Pioneer>()));

    //        LeadingConditionRule ConditionRule = new(new Conditions.NotExpert());
    //        ConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RedSteelAlloyBar>(), 1, 16, 24));
    //        ConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Trophy_Pioneer>(), 10, 1, 1));//纪念章
    //        ConditionRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<BossRelic_Pioneer>(), 1));//圣物
    //        ConditionRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<ShieldGenerator>(), 4));//宠物
    //        ConditionRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.IsExpert(), ModContent.ItemType<ChargingShields>()));//专家饰品
    //        ConditionRule.OnSuccess(ItemDropRule.ByCondition(new Conditions.IsMasterMode(), ModContent.ItemType<Transient>(), 10));//大师武器
    //        ConditionRule.OnSuccess(ItemDropRule.OneFromOptions(1,
    //        [
    //            ModContent.ItemType<GuidedRocketLauncher>(),
    //            ModContent.ItemType<BeamGun>(),
    //            ModContent.ItemType<SignalControlStaff>(),
    //            ModContent.ItemType<RestrictedAirspace>()
    //        ]));
    //        npcLoot.Add(ConditionRule);
    //    }
    //    public override void FindFrame(int frameHeight)
    //    {
    //    }
    //    private Vector2 Pos = Vector2.Zero;
    //    private Vector2 ShootPos
    //    {
    //        get { return new Vector2(NPC.Center.X, NPC.position.Y + NPC.height * 0.55f); }
    //    }
    //    private float ShieldCD = 0f;
    //    private float TrueSpeedMult = 1f;
    //    private float VelMult = 1f;
    //    public override void SendExtraAI(BinaryWriter writer)
    //    {
    //        writer.Write(ShieldCD);
    //        writer.WriteVector2(Pos);
    //        writer.Write(TrueSpeedMult);
    //        writer.Write(NPC.spriteDirection);
    //        writer.WriteVector2(NPC.position);
    //        writer.Write(NPC.Originate().ModAI[1]);
    //        writer.Write(NPC.Originate().ModAI[2]);
    //        writer.Write(NPC.Originate().ModAI[3]);
    //        writer.Write(NPC.Originate().ModAI[4]);
    //        int Stack = NotAI.Count;
    //        writer.Write(Stack);
    //        for (int i = 0; i < Stack; i++)
    //        {
    //            writer.Write(NotAI[i]);
    //        }
    //    }
    //    public override void ReceiveExtraAI(BinaryReader reader)
    //    {
    //        ShieldCD = reader.ReadSingle();
    //        Pos = reader.ReadVector2();
    //        TrueSpeedMult = reader.ReadSingle();
    //        NPC.spriteDirection = reader.ReadInt32();
    //        NPC.position = reader.ReadVector2();
    //        NPC.Originate().ModAI[1] = reader.ReadSingle();
    //        NPC.Originate().ModAI[2] = reader.ReadSingle();
    //        NPC.Originate().ModAI[3] = reader.ReadSingle();
    //        NPC.Originate().ModAI[4] = reader.ReadSingle();
    //        int Stack = reader.ReadInt32();
    //        List<float> ints = [];
    //        for (int i = 0; i < Stack; i++)
    //        {
    //            ints.Add(reader.ReadSingle());
    //        }
    //        NotAI = ints;
    //    }
    //    public override void AI()
    //    {
    //        if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
    //        {
    //            NPC.TargetClosest(true);
    //        }
    //        Player player = Main.player[NPC.target];
    //        NPC.dontTakeDamage = NPC.AnyNPCs(ModContent.NPCType<EnergyShield>());
    //        if (NPC.Originate().ModAI[3] > 0f && NPC.ai[1] != -2f)
    //        {
    //            float MaxTime = 60f;
    //            float AddTime = 15f;
    //            float DownTime = 45f;
    //            if (NPC.Originate().ModAI[3] < MaxTime)
    //            {
    //                NPC.Originate().ModAI[3] += 1f;
    //            }
    //            else
    //            {
    //                NPC.Originate().ModAI[3] = 0f;
    //                NPC.Originate().ModAI[4] = 0f;
    //            }
    //            if (NPC.Originate().ModAI[3] <= AddTime)
    //            {
    //                NPC.Originate().ModAI[4] += 1f / 15f;
    //            }
    //            else if (NPC.Originate().ModAI[3] >= DownTime)
    //            {
    //                NPC.Originate().ModAI[4] -= 1f / 15f;
    //            }
    //            for (int i = 0; i < 2; i++)
    //            {
    //                Vector2 Vel = Vector2.One.RotateRandom(MathHelper.Pi) * Main.rand.NextFloat(5f);
    //                if (Main.rand.NextBool(4)) Vel *= 1.5f;
    //                else if (Main.rand.NextBool(8)) Vel *= 2f;
    //                Vector2 Cen = ShootPos - Vel * 10f;
    //                int dust = Dust.NewDust(ShootPos, 0, 0, ModContent.DustType<GlowDust_Prismatic>(), 0, 0, 100, DustColor(), NPC.Originate().ModAI[3] / MaxTime);
    //                Main.dust[dust].position = Cen + NPC.velocity;
    //                Main.dust[dust].velocity = Vel;
    //                Main.dust[dust].customData = NPC;
    //                Main.dust[dust].noGravity = true;
    //                Main.dust[dust].scale *= 1.5f;
    //            }
    //        }//激光蓄力动画
    //        if (NPC.ai[0] == 0f)
    //        {
    //            if (NPC.ai[1] == 0f)
    //            {
    //                NPC.spriteDirection = player.Center.X > NPC.Center.X ? -1 : 1;
    //                NPC.Center = player.Center + new Vector2(NPC.spriteDirection * 150f, -2000f);
    //                Pos = player.Center + new Vector2(NPC.spriteDirection * 150f, -200f);
    //                if (Main.netMode != NetmodeID.MultiplayerClient)
    //                {
    //                    int a = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<EnergyShield>());
    //                    Main.npc[a].ai[0] = NPC.whoAmI;
    //                    if (Main.netMode == NetmodeID.Server && a < 200)
    //                    {
    //                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, a);
    //                    }
    //                }
    //                NPC.ai[1] = 1f;
    //                NPC.netUpdate = true;
    //            }
    //            else if (NPC.ai[1] == 1f)
    //            {
    //                ToPos(NPC, Pos, 0.5f);
    //                if (Vector2.Distance(NPC.Center, Main.LocalPlayer.Center) < 2000f)
    //                {
    //                    Main.LocalPlayer.Originate().ToScreen = NPC.Center;
    //                    Main.LocalPlayer.Originate().UseScreen = true;
    //                }
    //                if (Vector2.Distance(NPC.Center, Pos) < 50f)
    //                {
    //                    NPC.ai[1] = 2f;
    //                    NPC.netUpdate = true;
    //                }
    //            }
    //            else if (NPC.ai[1] == 2f)
    //            {
    //                Pos = player.Center + new Vector2(NPC.spriteDirection * 150f, -200f);
    //                ToPos(NPC, Pos, 0.2f);
    //                if (NPC.ai[2] >= 180f)
    //                {
    //                    if (NPC.ai[3] > 0f) NPC.ai[3] -= 1f / 30f;
    //                    else
    //                    {
    //                        NPC.ai[1] = 3f;
    //                        NPC.ai[2] = 0f;
    //                        NPC.ai[3] = 0f;
    //                        NPC.netUpdate = true;
    //                    }
    //                }
    //                else if (NPC.ai[3] < 1f)
    //                {
    //                    NPC.ai[3] += 1f / 30f;
    //                }
    //                else
    //                {
    //                    NPC.ai[2] += 1f;
    //                    NPC.ai[3] = 1f;
    //                }
    //            }
    //            else if (NPC.ai[1] == 3f)
    //            {
    //                Vector2 APos = player.Center + new Vector2(NPC.spriteDirection * 200f, -250f);
    //                ToPos(NPC, APos, 0.25f);
    //                if (NPC.ai[2] > 60f)
    //                {
    //                    Vector2 Vec = (Pos - ShootPos).SafeNormalize(NPC.velocity);
    //                    float Rot = 0.25f;
    //                    int Damage = NPC.GetNPCProDamage(0.8f);
    //                    if (Main.getGoodWorld)
    //                    {
    //                        Rot = 0.5f;
    //                        Damage = 167;
    //                    }
    //                    if (Main.netMode != NetmodeID.MultiplayerClient)
    //                    {
    //                        Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos, Vec, ModContent.ProjectileType<Pioneer_Pro>(),
    //                            Damage, 0f, -1, NPC.whoAmI, -NPC.spriteDirection, Rot);
    //                        Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos, Vec, ModContent.ProjectileType<Pioneer_Pro>(),
    //                            Damage, 0f, -1, NPC.whoAmI, NPC.spriteDirection, Rot);
    //                    }
    //                    NPC.ai[1] = 4f;
    //                    NPC.ai[2] = 0f;
    //                    NPC.ai[3] = 0f;
    //                    NPC.netUpdate = true;
    //                }
    //                else
    //                {
    //                    if (NPC.ai[2] <= 45f) Pos = player.Center;
    //                    if (NPC.ai[2] == 0f)
    //                    {
    //                        NPC.Originate().ModAI[3] = 1f;
    //                        NPC.netUpdate = true;
    //                    }
    //                    NPC.ai[2] += 1f;
    //                }
    //            }
    //            else if (NPC.ai[1] == 4f)
    //            {
    //                Vector2 APos = player.Center + new Vector2(NPC.spriteDirection * 200f, -250f);
    //                ToPos(NPC, APos, 0.1f);
    //                if (NPC.ai[2] < 60f)
    //                {
    //                    NPC.ai[2] += 1f;
    //                }
    //                else
    //                {
    //                    NPC.ai[0] = 1f;
    //                    NPC.ai[1] = -1f;
    //                    NPC.ai[2] = 0f;
    //                    NPC.ai[3] = 0f;
    //                    NPC.netUpdate = true;
    //                }
    //            }
    //        }//开场动画
    //        else if (NPC.ai[0] == 1f)
    //        {
    //            float SpeedMult = 1f;
    //            if (Main.expertMode)
    //            {
    //                float Life = 1f - NPC.life / (float)NPC.lifeMax;
    //                SpeedMult *= 1f + Life * 0.5f;
    //            }
    //            if (Main.masterMode)
    //            {
    //                float Life = 1f - NPC.life / (float)NPC.lifeMax;
    //                SpeedMult *= 1f + Life;
    //            }
    //            if (NPC.ai[1] == -2f)
    //            {
    //                Paralyzed();
    //            }//瘫痪状态
    //            else if (NPC.AnyNPCs(ModContent.NPCType<EnergyShield>()))
    //            {
    //                if (Main.masterMode)
    //                    SpeedMult *= 0.9f;
    //                if (Main.expertMode)
    //                    SpeedMult *= 0.8f;
    //                else
    //                    SpeedMult *= 0.7f;
    //            }
    //            else
    //            {
    //                ShieldGeneration();
    //            }
    //            TrueSpeedMult = SpeedMult;
    //            if (NPC.ai[1] == -1f)
    //            {
    //                StateSwitch(player);
    //                NPC.netUpdate = true;
    //            }//随机AI筛选
    //            if (NPC.ai[1] == 0f)
    //            {
    //                LaserBarrage(player);
    //            }
    //            if (NPC.ai[1] == 1f)
    //            {
    //                GuidedMissile(player);
    //            }
    //            if (NPC.ai[1] == 2f)
    //            {
    //                BlockingAttack(player);
    //            }
    //            if (NPC.ai[1] == 3f)
    //            {
    //                LaserCleaning(player);
    //            }
    //            if (NPC.ai[1] == 4f)
    //            {
    //                EnergyBomb(player);
    //            }
    //            if (NPC.ai[1] == 5f)
    //            {
    //                LaserBlockade(player);
    //            }
    //            if (NPC.ai[1] == 6f)
    //            {
    //                SpinBombing(player);
    //            }
    //            if (NPC.ai[1] == 7f)
    //            {
    //                LaserSweep(player);
    //            }
    //            if (NPC.ai[1] == 8f)
    //            {
    //                LaserCollision(player);
    //            }
    //        }//攻击状态
    //        {
    //            Color Light = DustColor();
    //            Light *= 0.5f;
    //            Lighting.AddLight(ShootPos, Light.ToVector3());
    //            if (Main.rand.NextBool(3))
    //            {
    //                Color color = Color.OrangeRed;
    //                int DustType = ModContent.DustType<GlowDust_Circle>();
    //                float Scale = 0.75f;
    //                if (NPC.velocity.Length() > 10f)
    //                {
    //                    DustType = ModContent.DustType<GlowDust_Prismatic>();
    //                    Scale = 1.5f;
    //                }
    //                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustType, 0, 0, 100, color, Scale);
    //                Main.dust[dust].velocity *= 0.1f;
    //                Main.dust[dust].velocity += NPC.velocity * 0.5f;
    //                Main.dust[dust].noGravity = true;
    //            }
    //            float R = NPC.life / (float)NPC.lifeMax;
    //            if(R < 0.8f)
    //            {
    //                if(Main.rand.NextBool(2 + 8.IntMult(R)))
    //                {
    //                    Color color = Colors.RarityTrash * 0.5f;
    //                    int b = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<FogDust>(), 0, 0, 0, color, 1f);
    //                    Main.dust[b].scale *= 1f - R;
    //                    Main.dust[b].velocity.Y -= 2f;
    //                }
    //            }
    //        }
    //        Vector2 OldPos = new(NPC.oldPos[2].X, NPC.position.Y + 100f);
    //        float ToRot = (NPC.position - OldPos).ToRotation() + MathHelper.PiOver2;
    //        NPC.rotation = NPC.rotation.AngleLerp(ToRot, 0.1f);
    //    }
    //    public void ToPos(NPC NPC, Vector2 TargetPos, float Mult = 1f)
    //    {
    //        Mult *= TrueSpeedMult;
    //        float Speed = NPC.Distance(TargetPos) / 15f;
    //        Speed = Math.Clamp(Speed, 0f, 42f);
    //        VelMult = MathHelper.Lerp(VelMult, Mult, 0.1f);
    //        Speed *= VelMult;
    //        Vector2 Vec = (TargetPos - NPC.Center).SafeNormalize(NPC.velocity * 0.1f) * Speed;
    //        NPC.velocity = Vec;
    //    }
    //    public void StateSwitch(Player player)
    //    {
    //        int MaxAttack = 5;
    //        float Life = NPC.life / (float)NPC.lifeMax;
    //        if(Main.masterMode)
    //        {
    //            if (Life < 0.85f) MaxAttack = 6;
    //            if (Life < 0.7f) MaxAttack = 7;
    //            if (Life < 0.55f) MaxAttack = 8;
    //            if (Life < 0.4f) MaxAttack = 9;
    //        }
    //        else if (Main.expertMode)
    //        {
    //            if (Life < 0.75f) MaxAttack = 6;
    //            if (Life < 0.5f) MaxAttack = 7;
    //            if (Life < 0.25f) MaxAttack = 8;
    //            if (Life < 0.1f) MaxAttack = 9;
    //        }
    //        if (NotAI.Count >= MaxAttack)
    //        {
    //            float i = NotAI[MaxAttack - 1];
    //            NotAI.Clear();
    //            NotAI.Add(i);
    //            NPC.netUpdate = true;
    //        }
    //        int Rand = Main.rand.Next(MaxAttack);
    //        if (!NotAI.Contains(Rand))
    //        {
    //            NPC.ai[1] = Rand;
    //            NPC.spriteDirection = player.Center.X > NPC.Center.X ? 1 : -1;
    //            NotAI.Add(Rand);
    //            NPC.netUpdate = true;
    //            return;
    //        }
    //    }
    //    public void Paralyzed()
    //    {
    //        NPC.velocity.X *= 0.96f;
    //        float WorldX = NPC.position.X / 16f;
    //        float WorldY = (NPC.position.Y + NPC.height) / 16f;
    //        for (float X = WorldX; X < WorldX + NPC.width / 16f; X += 1f)
    //        {
    //            for (float Y = WorldY; Y < WorldY + 5f; Y += 1f)
    //            {
    //                var tile = Main.tile[(int)X, (int)Y];
    //                if (tile.HasTile && (TileID.Sets.Platforms[tile.TileType] || Main.tileSolid[tile.TileType]))
    //                {
    //                    NPC.noTileCollide = true;
    //                }
    //            }
    //        }
    //        ShieldCD = 3600;
    //        if (NPC.Originate().ModAI[4] > 0f)
    //        {
    //            NPC.Originate().ModAI[4] -= 1f / 15f;
    //        }
    //        if (NPC.ai[2] < 120f)
    //        {
    //            NPC.velocity.Y *= 0.9f;
    //            if (NPC.velocity.Y < 2f && !NPC.noTileCollide)
    //                NPC.velocity.Y += 0.5f;
    //        }
    //        if (NPC.ai[2] < 600f)
    //        {
    //            NPC.ai[2] += 0.5f;
    //            if (NPC.noTileCollide)
    //                NPC.ai[2] += 0.5f;
    //        }
    //        else
    //        {
    //            NPC.ai[1] = -1f;
    //            NPC.ai[2] = 0f;
    //            NPC.Originate().ModAI[1] = 0f;
    //            NPC.Originate().ModAI[2] = 0f;
    //            NPC.netUpdate = true;
    //            return;
    //        }
    //        if (NPC.noTileCollide)
    //        {
    //            if (NPC.Originate().ModAI[1] == 0f)
    //            {
    //                NPC.Originate().ModAI[2] += 1f;
    //                if (NPC.velocity.Y > -0.5f)
    //                    NPC.velocity.Y -= 0.01f;
    //                else
    //                    NPC.velocity.Y *= 0.96f;
    //                if (NPC.Originate().ModAI[2] > 120f)
    //                {
    //                    NPC.Originate().ModAI[1] = 1f;
    //                    NPC.Originate().ModAI[2] = 0f;
    //                    NPC.netUpdate = true;
    //                }
    //            }
    //            else
    //            {
    //                NPC.Originate().ModAI[2] += 1f;
    //                if (NPC.velocity.Y < 0.5f)
    //                    NPC.velocity.Y += 0.01f;
    //                else
    //                    NPC.velocity.Y *= 0.96f;
    //                if (NPC.Originate().ModAI[2] > 120f)
    //                {
    //                    NPC.Originate().ModAI[1] = 0f;
    //                    NPC.Originate().ModAI[2] = 0f;
    //                    NPC.netUpdate = true;
    //                }
    //            }
    //        }
    //    }
    //    public void ShieldGeneration()
    //    {
    //        ShieldCD += 1;
    //        int Time = 1800;
    //        if (Main.expertMode) Time = 1200;
    //        if (Main.masterMode) Time = 900;
    //        if (ShieldCD > Time)
    //        {
    //            ShieldCD = 0;
    //            if (Main.netMode != NetmodeID.MultiplayerClient)
    //            {
    //                int a = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<EnergyShield>());
    //                Main.npc[a].ai[0] = NPC.whoAmI;
    //                if (Main.netMode == NetmodeID.Server && a < 200)
    //                {
    //                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, a);
    //                }
    //            }
    //        }
    //    }
    //    public void LaserBarrage(Player player)
    //    {
    //        Vector2 Cen = player.Center + player.velocity + new Vector2(400f * NPC.spriteDirection, -300f);
    //        if (NPC.ai[2] < 90f)
    //        {
    //            Pos = player.Center;
    //            ToPos(NPC, Cen);
    //        }
    //        else if (NPC.ai[2] > 120f)
    //        {
    //            if(Main.getGoodWorld)
    //            {
    //                if (NPC.ai[3] % 6f == 0f)
    //                {
    //                    Vector2 Vec = Vector2.Normalize(Pos - ShootPos) * 18f;
    //                    Vec = -Vec.RotatedByRandom(MathHelper.PiOver4);
    //                    float OffsetX = Main.rand.NextFloat(-100f, 100f);
    //                    float OffsetY = Main.rand.NextFloat(-100f, 100f);
    //                    Vector2 ShootToPos = player.Center + player.velocity * 30f + new Vector2(OffsetX, OffsetY);
    //                    if (Main.netMode != NetmodeID.MultiplayerClient)
    //                    {
    //                        Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, Vec, ModContent.ProjectileType<Pioneer_Pro3>(),
    //                            NPC.GetNPCProDamage(0.8f), 0f, -1, 1f, ShootToPos.X, ShootToPos.Y);
    //                    }
    //                    Pos = player.Center;
    //                    NPC.netUpdate = true;
    //                }
    //            }
    //            else if (NPC.ai[3] % 30f == 0f)
    //            {
    //                for (int i = -1; i <= 1; i += 2)
    //                {
    //                    Vector2 Vec = Vector2.Normalize(Pos - ShootPos) * 12f;
    //                    Vector2 finalVec = Vec.RotatedBy(MathHelper.PiOver2 * i);
    //                    Vec = Vec.RotatedByRandom(0.25f);
    //                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, finalVec, ModContent.ProjectileType<Pioneer_Pro3>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                }
    //            }
    //            if (NPC.ai[3] % 6f == 0f)
    //            {
    //                Vector2 Vec = Vector2.Normalize(Pos - ShootPos) * 12f;
    //                Vec = Vec.RotatedByRandom(0.2f);
    //                Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, Vec, ModContent.ProjectileType<Pioneer_Pro2>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                Pos = player.Center;
    //                NPC.netUpdate = true;
    //            }
    //            NPC.ai[3] += 1f;
    //            if (NPC.ai[3] > 60f)
    //            {
    //                NPC.ai[1] = -1f;
    //                NPC.ai[2] = 0f;
    //                NPC.ai[3] = 0f;
    //                NPC.Originate().ModAI[1] = 0f;
    //                NPC.Originate().ModAI[2] = 0f;
    //                NPC.netUpdate = true;
    //                return;
    //            }
    //        }
    //        else ToPos(NPC, Cen, 0.15f);
    //        NPC.ai[2] += 1f * TrueSpeedMult;
    //    }
    //    public void GuidedMissile(Player player)
    //    {
    //        Vector2 Cen = player.Center + player.velocity + new Vector2(450f * NPC.spriteDirection, 0f);
    //        if (NPC.ai[2] < 90f)
    //        {
    //            ToPos(NPC, Cen);
    //        }
    //        else if (NPC.Originate().ModAI[1] == 0f)
    //        {
    //            Pos = player.Center;
    //            NPC.Originate().ModAI[1] = 1f;
    //            NPC.netUpdate = true;
    //        }
    //        else if (NPC.ai[2] > 120f)
    //        {
    //            ToPos(NPC, Cen, 0.5f);
    //            float Time = 6f;
    //            if (Main.getGoodWorld) Time = 4f;
    //            if (NPC.ai[3] % Time == 0f)
    //            {
    //                Vector2 Vec = Vector2.Normalize(Pos - ShootPos) * 18f;
    //                Vec = -Vec.RotatedByRandom(MathHelper.PiOver4);
    //                float OffsetX = Main.rand.NextFloat(-100f, 100f);
    //                float OffsetY = Main.rand.NextFloat(-100f, 100f);
    //                Vector2 ShootToPos = player.Center + player.velocity * 30f + new Vector2(OffsetX, OffsetY);
    //                if(Main.getGoodWorld)
    //                {
    //                    ShootToPos += player.velocity * 15f;
    //                }
    //                if (Main.netMode != NetmodeID.MultiplayerClient)
    //                {
    //                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, Vec, ModContent.ProjectileType<Pioneer_Pro3>(),
    //                        NPC.GetNPCProDamage(0.8f), 0f, -1, 1f, ShootToPos.X, ShootToPos.Y);
    //                }
    //                Pos = player.Center;
    //                NPC.netUpdate = true;
    //            }
    //            NPC.ai[3] += 1f;
    //            if (NPC.ai[3] > 60f)
    //            {
    //                NPC.ai[1] = -1f;
    //                NPC.ai[2] = 0f;
    //                NPC.ai[3] = 0f;
    //                NPC.Originate().ModAI[1] = 0f;
    //                NPC.Originate().ModAI[2] = 0f;
    //                NPC.netUpdate = true;
    //                return;
    //            }
    //        }
    //        else ToPos(NPC, Cen, 0.15f);
    //        NPC.ai[2] += 1f * TrueSpeedMult;
    //    }
    //    public void BlockingAttack(Player player)
    //    {
    //        Vector2 Offset = new(500f * NPC.spriteDirection, -325f);
    //        if (NPC.Originate().ModAI[1] > 1f) Offset.Y *= -1f;
    //        Vector2 Cen = player.Center + player.velocity + Offset;
    //        if (NPC.ai[2] < 90f)
    //        {
    //            ToPos(NPC, Cen);
    //        }
    //        else if (NPC.Originate().ModAI[1] == 0f)
    //        {
    //            Pos = player.Center;
    //            NPC.Originate().ModAI[1] = 1f;
    //            NPC.netUpdate = true;
    //        }
    //        else if (NPC.ai[2] > 120f)
    //        {
    //            NPC.ai[3] += 1f;
    //            if (NPC.ai[3] < 30f)
    //            {
    //                ToPos(NPC, Cen, 0.3f);
    //                Pos = player.Center;
    //                NPC.Originate().ModAI[2] = -3f;
    //            }
    //            else if (NPC.ai[3] < 60f)
    //            {
    //                ToPos(NPC, Cen, 0.1f);
    //                if (NPC.ai[3] % 5 == 0)
    //                {
    //                    Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 8f;
    //                    Vector2 RVec = Vec.RotatedBy(0.25f * NPC.Originate().ModAI[2]);
    //                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, RVec, ModContent.ProjectileType<Pioneer_Pro2>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                    NPC.Originate().ModAI[2] += 1f;
    //                }
    //            }
    //            else if (NPC.ai[3] == 60f)
    //            {
    //                NPC.spriteDirection *= -1;
    //                NPC.netUpdate = true;
    //            }
    //            else if (NPC.ai[3] < 90f)
    //            {
    //                ToPos(NPC, Cen);
    //                if (Main.getGoodWorld)
    //                {
    //                    if (NPC.ai[3] % 5f == 0f)
    //                    {
    //                        Vector2 Vec = Vector2.Normalize(Pos - ShootPos) * 18f;
    //                        Vec = -Vec.RotatedByRandom(MathHelper.PiOver4);
    //                        float OffsetX = Main.rand.NextFloat(-100f, 100f);
    //                        float OffsetY = Main.rand.NextFloat(-100f, 100f);
    //                        Vector2 ShootToPos = player.Center + player.velocity * 15f + new Vector2(OffsetX, OffsetY);
    //                        if (Main.netMode != NetmodeID.MultiplayerClient)
    //                        {
    //                            int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, Vec, ModContent.ProjectileType<Pioneer_Pro3>(),
    //                                NPC.GetNPCProDamage(0.8f), 0f, -1, 1f, ShootToPos.X, ShootToPos.Y);
    //                        }
    //                        Pos = player.Center;
    //                        NPC.netUpdate = true;
    //                    }
    //                }
    //                else if (NPC.ai[3] % 5 == 0)
    //                {
    //                    Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 8f;
    //                    Vec = Vec.RotatedByRandom(MathHelper.PiOver4);
    //                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, Vec, ModContent.ProjectileType<Pioneer_Pro3>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                }
    //            }
    //            else if (NPC.ai[3] < 120f)
    //            {
    //                ToPos(NPC, Cen, 0.3f);
    //                Pos = player.Center;
    //                NPC.netUpdate = true;
    //            }
    //            else
    //            {
    //                ToPos(NPC, Cen, 0.1f);
    //                if (NPC.ai[3] % 5 == 0)
    //                {
    //                    Vector2 Vec = Vector2.Normalize(Pos - NPC.Center) * 9f;
    //                    Vector2 RVec = Vec.RotatedBy(0.25f * NPC.Originate().ModAI[2]);
    //                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, RVec, ModContent.ProjectileType<Pioneer_Pro2>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                    NPC.Originate().ModAI[2] -= 1f;
    //                }
    //            }
    //            if (NPC.ai[3] >= 150f)
    //            {
    //                if (Main.getGoodWorld && NPC.Originate().ModAI[1] == 1f)
    //                {
    //                    NPC.ai[3] = 0f;
    //                    NPC.Originate().ModAI[1] += 1f;
    //                    NPC.Originate().ModAI[2] = 0f;
    //                    return;
    //                }

    //                NPC.ai[1] = -1f;
    //                NPC.ai[2] = 0f;
    //                NPC.ai[3] = 0f;
    //                NPC.Originate().ModAI[1] = 0f;
    //                NPC.Originate().ModAI[2] = 0f;
    //                NPC.netUpdate = true;
    //                return;
    //            }
    //        }
    //        else ToPos(NPC, Cen, 0.15f);
    //        NPC.ai[2] += 1f * TrueSpeedMult;
    //    }
    //    public void LaserCleaning(Player player)
    //    {
    //        Vector2 Cen = player.Center + player.velocity + new Vector2(150f * NPC.spriteDirection, -300f);
    //        if (NPC.ai[2] < 120f)
    //        {
    //            ToPos(NPC, Cen);
    //        }
    //        else if (NPC.Originate().ModAI[1] == 0f)
    //        {
    //            NPC.Originate().ModAI[1] = 1f;
    //            NPC.Originate().ModAI[3] = 1f;
    //            NPC.netUpdate = true;
    //        }
    //        else
    //        {
    //            if (NPC.ai[3] < 60f)
    //            {
    //                ToPos(NPC, Cen);
    //            }
    //            else if (NPC.ai[3] == 60f)
    //            {
    //                Vector2 Vec = (player.Center - ShootPos).SafeNormalize(NPC.velocity);
    //                float Rot = MathHelper.Pi * 0.25f;
    //                if(Main.getGoodWorld) Rot *= 1.25f;
    //                Vec = Vec.RotatedBy(Rot * NPC.spriteDirection);
    //                if (Main.netMode != NetmodeID.MultiplayerClient)
    //                {
    //                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos, Vec, ModContent.ProjectileType<Pioneer_Pro>(),
    //                        NPC.GetNPCProDamage(0.8f), 0f, -1, NPC.whoAmI, -NPC.spriteDirection, Rot * 2f);
    //                }
    //            }
    //            else
    //            {
    //                ToPos(NPC, Cen, 0.1f);
    //                if (NPC.ai[3] > 120f)
    //                {
    //                    NPC.ai[1] = -1f;
    //                    NPC.ai[2] = 0f;
    //                    NPC.ai[3] = 0f;
    //                    NPC.Originate().ModAI[1] = 0f;
    //                    NPC.Originate().ModAI[2] = 0f;
    //                    NPC.netUpdate = true;
    //                    return;
    //                }
    //            }
    //            NPC.ai[3] += 1f;
    //        }
    //        NPC.ai[2] += 1f * TrueSpeedMult;
    //    }
    //    public void EnergyBomb(Player player)
    //    {
    //        Vector2 Offset = new(400f * NPC.spriteDirection, -250f);
    //        if (NPC.spriteDirection == 0) Offset.Y = 450f;
    //        Vector2 Cen = player.Center + player.velocity + Offset;
    //        if (NPC.ai[2] < 90f)
    //        {
    //            ToPos(NPC, Cen);
    //        }
    //        else if (NPC.ai[2] > 120f)
    //        {
    //            NPC.ai[3] += 1f;
    //            if (NPC.ai[3] < 30f)
    //            {
    //                ToPos(NPC, Cen);
    //                if (NPC.ai[3] % 6f == 0f)
    //                {
    //                    Vector2 Vec = Vector2.Normalize(Pos - ShootPos) * 15f;
    //                    Vec = -Vec.RotatedByRandom(MathHelper.PiOver4);
    //                    float OffsetX = Main.rand.NextFloat(-100f, 100f);
    //                    float OffsetY = Main.rand.NextFloat(-100f, 100f);
    //                    Vector2 ShootToPos = player.Center + player.velocity * 60f + new Vector2(OffsetX, OffsetY);
    //                    if (Main.netMode != NetmodeID.MultiplayerClient)
    //                    {
    //                        int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, Vec, ModContent.ProjectileType<Pioneer_Pro3>(),
    //                            NPC.GetNPCProDamage(0.8f), 0f, -1, 1f, ShootToPos.X, ShootToPos.Y);
    //                    }
    //                    Pos = player.Center;
    //                    NPC.netUpdate = true;
    //                }
    //            }
    //            else if (NPC.ai[3] < 60f)
    //            {
    //                ToPos(NPC, Cen, 0.5f);
    //            }
    //            else if (NPC.ai[3] == 60f)
    //            {
    //                Vector2 Vec = (player.Center - ShootPos).SafeNormalize(NPC.velocity) * 12f;
    //                Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos, Vec, ModContent.ProjectileType<Pioneer_Pro5>(),
    //                    NPC.GetNPCProDamage(1f), 0f, 0);
    //            }
    //            else if (NPC.ai[3] < 150f)
    //            {
    //                ToPos(NPC, Cen, 0.1f);
    //            }
    //            else
    //            {
    //                if(NPC.Originate().ModAI[1] == 0f)
    //                {
    //                    NPC.ai[3] = 0f;
    //                    NPC.spriteDirection *= -1;
    //                }
    //                else if(NPC.Originate().ModAI[1] == 1f)
    //                {
    //                    NPC.ai[3] = 0f;
    //                    NPC.spriteDirection *= 0;
    //                }
    //                else
    //                {
    //                    NPC.ai[1] = -1f;
    //                    NPC.ai[2] = 0f;
    //                    NPC.ai[3] = 0f;
    //                    NPC.Originate().ModAI[1] = 0f;
    //                    NPC.Originate().ModAI[2] = 0f;
    //                    return;
    //                }
    //                NPC.Originate().ModAI[1] += 1f;
    //                NPC.netUpdate = true;
    //            }
    //        }
    //        else ToPos(NPC, Cen, 0.15f);
    //        NPC.ai[2] += 1f * TrueSpeedMult;
    //    }
    //    public void LaserBlockade(Player player)
    //    {
    //        Vector2 Cen = player.Center + player.velocity + new Vector2(450f * NPC.spriteDirection, -250f);
    //        if (NPC.ai[2] < 90f)
    //        {
    //            Pos = player.Center;
    //            ToPos(NPC, Cen);
    //        }
    //        else if (NPC.ai[2] > 120f)
    //        {
    //            ToPos(NPC, Cen, 0.15f);
    //            if (NPC.ai[3] % 30f == 0f)
    //            {
    //                Vector2 Vec = Vector2.Normalize(Pos - ShootPos) * 9f;
    //                for (int i = -1; i <= 1; i += 2)
    //                {
    //                    Vector2 finalVec = Vec.RotatedBy(MathHelper.PiOver2 * i);
    //                    Vec = Vec.RotatedByRandom(0.25f);
    //                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, finalVec, ModContent.ProjectileType<Pioneer_Pro3>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                }
    //            }
    //            if(Main.getGoodWorld)
    //            {
    //                if (NPC.ai[3] % 6f == 0f)
    //                {
    //                    for (int i = 0; i <= 2; i++)
    //                    {
    //                        Vector2 Vec = Vector2.Normalize(Pos - ShootPos).RotatedByRandom(MathHelper.PiOver2);
    //                        Vector2 finalVec = Vec * 4f;
    //                        Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + finalVec, finalVec, ModContent.ProjectileType<Pioneer_Pro2>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                    }
    //                    Pos = player.Center;
    //                    NPC.netUpdate = true;
    //                }
    //            }
    //            else if (NPC.ai[3] % 20f == 0f)
    //            {
    //                Vector2 Vec = Vector2.Normalize(Pos - ShootPos).RotatedByRandom(0.2f);
    //                for (int i = -4; i <= 4; i++)
    //                {
    //                    Vector2 finalVec = Vec.RotatedBy(0.6f * i) * 4f;
    //                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + finalVec, finalVec, ModContent.ProjectileType<Pioneer_Pro2>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                }
    //                Pos = player.Center;
    //                NPC.netUpdate = true;
    //            }
    //            NPC.ai[3] += 1f;
    //            if (NPC.ai[3] > 60f)
    //            {
    //                NPC.ai[1] = -1f;
    //                NPC.ai[2] = 0f;
    //                NPC.ai[3] = 0f;
    //                NPC.Originate().ModAI[1] = 0f;
    //                NPC.Originate().ModAI[2] = 0f;
    //                NPC.netUpdate = true;
    //                return;
    //            }
    //        }
    //        else
    //        {
    //            Pos += NPC.velocity;
    //            ToPos(NPC, Cen, 0.1f);
    //        }
    //        NPC.ai[2] += 1f * TrueSpeedMult;
    //    }
    //    public void SpinBombing(Player player)
    //    {
    //        Vector2 Cen = player.Center + player.velocity + new Vector2(400f * NPC.spriteDirection, 0f);
    //        if (NPC.ai[2] < 90f)
    //        {
    //            ToPos(NPC, Cen);
    //        }
    //        else if (NPC.ai[2] > 120f)
    //        {
    //            Cen = player.Center + Utils.RotatedBy(new Vector2(800f * NPC.spriteDirection, 0f), MathHelper.ToRadians((-30f + NPC.ai[3] * 4f) * NPC.spriteDirection));
    //            ToPos(NPC, Cen);
    //            NPC.ai[3] += 1f;
    //            if (NPC.ai[3] % 6f == 0f)
    //            {
    //                Vector2 Vec = Vector2.Normalize(Pos - ShootPos) * 10f;
    //                Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec, Vec, ModContent.ProjectileType<Pioneer_Pro3>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                Pos = player.Center;
    //                NPC.netUpdate = true;
    //            }
    //            float CD = 60f;
    //            if (Main.getGoodWorld) CD = 90f;
    //            if (NPC.ai[3] > CD)
    //            {
    //                NPC.ai[1] = -1f;
    //                NPC.ai[2] = 0f;
    //                NPC.ai[3] = 0f;
    //                NPC.Originate().ModAI[1] = 0f;
    //                NPC.Originate().ModAI[2] = 0f;
    //                NPC.netUpdate = true;
    //                return;
    //            }
    //        }
    //        else ToPos(NPC, Cen, 0.15f);
    //        NPC.ai[2] += 1f * TrueSpeedMult;
    //    }
    //    public void LaserSweep(Player player)
    //    {
    //        Vector2 Cen = player.Center + player.velocity + new Vector2(300f * NPC.spriteDirection, 0f);
    //        if (NPC.ai[2] < 120f)
    //        {
    //            ToPos(NPC, Cen);
    //        }
    //        else if (NPC.Originate().ModAI[1] == 0f)
    //        {
    //            NPC.Originate().ModAI[1] = 1f;
    //            NPC.Originate().ModAI[3] = 1f;
    //            NPC.netUpdate = true;
    //        }
    //        else
    //        {
    //            if (NPC.ai[3] < 60f)
    //            {
    //                ToPos(NPC, Cen, 0.75f);
    //            }
    //            else if (NPC.ai[3] == 60f)
    //            {
    //                Vector2 Vec = -Vector2.UnitX * NPC.spriteDirection;
    //                float Rot = 0.25f;
    //                if (Main.netMode != NetmodeID.MultiplayerClient)
    //                {
    //                    if(Main.getGoodWorld)
    //                    {
    //                        Rot = MathHelper.PiOver2;
    //                        Vec = Vec.RotatedBy(Rot * NPC.spriteDirection * 0.9f);
    //                        Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos, Vec, ModContent.ProjectileType<Pioneer_Pro>(),
    //                            NPC.GetNPCProDamage(0.8f), 0f, -1, NPC.whoAmI, -NPC.spriteDirection, Rot * 2.25f);
    //                    }
    //                    else
    //                    {
    //                        Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos, Vec, ModContent.ProjectileType<Pioneer_Pro>(),
    //                            NPC.GetNPCProDamage(0.8f), 0f, -1, NPC.whoAmI, -NPC.spriteDirection, Rot);
    //                        Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos, Vec, ModContent.ProjectileType<Pioneer_Pro>(),
    //                            NPC.GetNPCProDamage(0.8f), 0f, -1, NPC.whoAmI, NPC.spriteDirection, Rot);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                ToPos(NPC, Cen, 0.05f);
    //                if (NPC.ai[3] > 120f)
    //                {
    //                    NPC.ai[1] = -1f;
    //                    NPC.ai[2] = 0f;
    //                    NPC.ai[3] = 0f;
    //                    NPC.Originate().ModAI[1] = 0f;
    //                    NPC.Originate().ModAI[2] = 0f;
    //                    NPC.netUpdate = true;
    //                    return;
    //                }
    //            }
    //            NPC.ai[3] += 1f;
    //        }
    //        NPC.ai[2] += 1f * TrueSpeedMult;
    //    }
    //    public void LaserCollision(Player player)
    //    {
    //        Vector2 Cen = player.Center + player.velocity + new Vector2(0f, -450f);
    //        if (NPC.ai[2] < 120f)
    //        {
    //            ToPos(NPC, Cen);
    //            Pos = player.Center;
    //            if (NPC.Originate().ModAI[1] == 0f)
    //                NPC.ai[2] += 1f * TrueSpeedMult;
    //            else
    //                NPC.ai[2] += 1f;
    //            if (NPC.ai[2] > 90f && NPC.Originate().ModAI[1] == 0f)
    //            {
    //                NPC.Originate().ModAI[1] = 1f;
    //                NPC.Originate().ModAI[2] = NPC.spriteDirection;
    //                NPC.Originate().ModAI[3] = 1f;
    //                NPC.ai[2] = 90f;
    //                NPC.netUpdate = true;
    //            }
    //        }
    //        else
    //        {
    //            if (NPC.ai[3] < 30f)
    //            {
    //                NPC.ai[3] += 1f;
    //                Cen = player.Center + new Vector2(600f * NPC.spriteDirection, -300f);
    //                ToPos(NPC, Cen);
    //            }
    //            else if (NPC.ai[3] == 30f)
    //            {
    //                NPC.ai[3] += 1f;
    //                NPC.velocity *= 0f;
    //                Pos = new Vector2(player.Center.X - 1200f * NPC.spriteDirection, NPC.Center.Y);
    //                Vector2 Vec = Vector2.UnitY;
    //                if (Main.netMode != NetmodeID.MultiplayerClient)
    //                {
    //                    Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos, Vec, ModContent.ProjectileType<Pioneer_Pro7>(), NPC.GetNPCProDamage(0.8f), 0f, -1, NPC.whoAmI);
    //                }
    //                NPC.netUpdate = true;
    //            }
    //            else if (NPC.ai[3] < 45f)
    //            {

    //                if (NPC.Center.X < Pos.X && NPC.spriteDirection == -1 || NPC.Center.X > Pos.X && NPC.spriteDirection == 1)
    //                {
    //                    NPC.ai[3] = 31f;
    //                    NPC.Originate().ModAI[2] *= 1.04f;
    //                    NPC.velocity.X -= 0.02f * NPC.Originate().ModAI[2];
    //                    NPC.velocity.Y = 0f;

    //                    if (Main.getGoodWorld)
    //                    {
    //                        if (Math.Abs(NPC.velocity.X) > 3f)
    //                        {
    //                            if (OriginateWorld.Time % 3f == 0)
    //                            {
    //                                Vector2 Vec = Vector2.UnitY;
    //                                Projectile.NewProjectile(NPC.GetSource_FromAI(), ShootPos + Vec * 1000f, Vec * -12f, ModContent.ProjectileType<Pioneer_Pro2>(), NPC.GetNPCProDamage(0.8f), 0f, 0);
    //                            }
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    NPC.velocity *= 0.9f;
    //                    NPC.ai[3] += 1f;
    //                }
    //            }
    //            else
    //            {
    //                NPC.ai[1] = -1f;
    //                NPC.ai[2] = 0f;
    //                NPC.ai[3] = 0f;
    //                NPC.Originate().ModAI[1] = 0f;
    //                NPC.Originate().ModAI[2] = 0f;
    //                NPC.netUpdate = true;
    //                return;
    //            }
    //        }
    //    }
    //    private List<float> NotAI = [];
    //}
}