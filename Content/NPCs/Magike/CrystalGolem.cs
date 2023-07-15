using Coralite.Content.Items.Magike.OtherPlaceables;
using Coralite.Content.Items.Magike;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ReLogic.Content;
using Terraria.Graphics.CameraModifiers;
using Coralite.Content.Biomes;

namespace Coralite.Content.NPCs.Magike
{
    public class CrystalGolem : ModNPC, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.MagikeNPCs + Name;
        public Player Target => Main.player[NPC.target];

        public ref float State => ref NPC.ai[0];
        public ref float MoveTime => ref NPC.ai[1];
        public ref float TargetRot => ref NPC.ai[2];

        public ref float TargetLineLength => ref NPC.localAI[0];
        public ref float FreezeTime => ref NPC.localAI[1];

        public static Asset<Texture2D> WarningLineTex;
        public static Asset<Texture2D> WarningLineSideTex;
        public const int FrameHeight = 848 / 8;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 84;
            NPC.height = 102;
            NPC.damage = 20;
            NPC.defense = 10;

            NPC.lifeMax = 300;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.GlassBroken_Shatter;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            WarningLineTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarningLineBlank");
            WarningLineSideTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarningLineSide");
            for (int i = 0; i < 5; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.MagikeNPCs + "CrystalGolem_Gore" + i);
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            WarningLineTex = null;
            WarningLineSideTex = null;
        }

        public override void AI()
        {
            if (Target.dead || !Target.active || (Target.Center - NPC.Center).Length() > 2000f)
                NPC.EncourageDespawn(10);

            //朝向玩家移动
            //移动时有一定的节奏
            //开始移动时随机一个移动时间
            //移动时间归零后检测
            //可以打到玩家且距离小于一定值时停止并蓄力射激光
            //否则重新开始移动

            switch ((int)State)
            {
                default:
                case 0: //普通向玩家移动
                    if (Math.Abs(NPC.velocity.Y) < 0.2f)
                    {
                        if (FreezeTime>0)
                        {
                            NPC.velocity *= 0;
                            FreezeTime--;
                            return;
                        }

                        NPC.frameCounter++;
                        if (NPC.frameCounter > 8)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y += FrameHeight;
                            if (NPC.frame.Y > 7 * FrameHeight)
                                NPC.frame.Y = 0;

                            NPC.TargetClosest();
                            if (NPC.frame.Y is 0 or 4 * FrameHeight)
                            {
                                FreezeTime = 12;
                                Collision.HitTiles(NPC.BottomLeft, -Vector2.UnitY * 16, NPC.width, 16);
                                SoundEngine.PlaySound(CoraliteSoundID.StaffOfEarth_Item69, NPC.Center);
                                var modifyer = new PunchCameraModifier(NPC.Center, -Vector2.UnitY, 4, 6, 6, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                            }
                        }

                        Helpers.Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 1.2f, 0.08f, 0.08f, 0.98f);
                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
                    }
                    else
                    {
                        FreezeTime = 0;
                        NPC.frame.Y = FrameHeight * 2;
                    }

                    MoveTime--;
                    if (MoveTime < 0)
                    {
                        //检测是否能攻击
                        if (NPC.Distance(Target.Center)<80*16&&Collision.CanHitLine(GetHeadPos(),1,1,Target.Center,1,1))
                        {
                            State = 1;
                            NPC.velocity *= 0;
                            NPC.frame.Y = 0;
                            MoveTime = 180;
                            NPC.TargetClosest(false);
                        }
                        else
                        {
                            NPC.TargetClosest();
                            MoveTime = Main.rand.Next(180, 360);
                        }
                    }
                    break;
                case 1://射激光的蓄力阶段
                    NPC.frame.Y = 0;
                    TargetLineLength+=2;

                    Vector2 center=GetHeadPos();
                    float factor = MoveTime / 180;
                    float width = 2 + factor * 44;

                    if (MoveTime % 5 == 0)
                    {
                        Dust dust = Dust.NewDustPerfect(center + Main.rand.NextVector2CircularEdge(width * 1.5f, width * 1.5f),
                            DustID.FireworksRGB, Vector2.Zero, newColor: Coralite.Instance.MagicCrystalPink);
                        dust.noGravity = true;
                        dust.velocity = (center - dust.position).SafeNormalize(Vector2.UnitX) * width / 10;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(center + Main.rand.NextVector2CircularEdge(width, width), DustID.LastPrism, Vector2.Zero, newColor: Coralite.Instance.MagicCrystalPink);
                        dust.noGravity = true;
                    }

                    if (MoveTime > 30)
                    {
                        TargetRot = TargetRot.AngleTowards((Target.Center - GetHeadPos()).ToRotation(), 0.04f);
                        if ((int)MoveTime % 20 == 0)
                        {
                            NPC.TargetClosest(false);
                            SoundEngine.PlaySound(CoraliteSoundID.LaserSwing_Item15, NPC.Center);
                        }
                    }
                    else if (MoveTime < 0)
                    {
                        TargetLineLength = -1;
                        NPC.TargetClosest(false);
                        MoveTime = 90;
                        State = 2;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), GetHeadPos(), Vector2.Zero, ModContent.ProjectileType<CrystalLaser>(), 20, 8, NPC.target, TargetRot);
                    }
                    MoveTime--;
                    break;
                case 2://射激光阶段
                    NPC.velocity *= 0;
                    NPC.frame.Y = 0;

                    if (MoveTime < 0)
                    {
                        State = 0;
                        NPC.TargetClosest();
                        MoveTime = Main.rand.Next(180, 360);
                    }

                    MoveTime--;
                    break;
            }
        }

        private Vector2 GetHeadPos()
        {
            return NPC.Center + new Vector2(NPC.direction * 10, -30);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<MagicCrystalCave>())
                return 0.01f;

            return 0;
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 5; i++)
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(1, 1), Mod.Find<ModGore>("CrystalGolem_Gore" + i).Type);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Teleporter, Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-2, 2), 100, Coralite.Instance.MagicCrystalPink, 1f);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CrystalSerpent_Pink);
                    dust.noGravity = true;
                    dust.velocity = new Microsoft.Xna.Framework.Vector2(hit.HitDirection, 0).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * hit.Knockback * Main.rand.NextFloat(0.9f, 1.1f);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Basalt>(), 1, 0, 8));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagicCrystal>(), 1, 0, 4));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Vector2 pos = NPC.Center-screenPos;
            var origin = NPC.frame.Size() / 2;

            SpriteEffects effect = NPC.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(mainTex, pos, NPC.frame, drawColor, 0f, origin, NPC.scale, effect, 0f);

            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (TargetLineLength > 0)
            {
                Texture2D lineTex = WarningLineTex.Value;

                float height = lineTex.Height / 5f;
                Vector2 startPos = GetHeadPos() - Main.screenPosition;
                Rectangle target = new Rectangle((int)startPos.X, (int)startPos.Y, (int)TargetLineLength, (int)height);

                Color color = Coralite.Instance.MagicCrystalPink;
                color.A = 200;
                spriteBatch.Draw(lineTex, target, null, color, TargetRot, new Vector2(0, lineTex.Height / 2), 0, 0);
                spriteBatch.Draw(WarningLineSideTex.Value, target, null, Color.White, TargetRot, new Vector2(0, lineTex.Height / 2), 0, 0);
            }
        }
    }
}
