using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Windows.Markup;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai3控制阶段，如果阶段为1的话那么就返回自身后死掉
    /// </summary>
    public class NightmareHook : ModNPC
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private Player OwnerTarget => Main.player[NightmareOwner.target];
        private Player SelfTarget => Main.player[NPC.target];
        /// <summary>
        /// 使用前请检测是否越界！
        /// </summary>
        private static NPC NightmareOwner => Main.npc[NightmarePlantera.NPBossIndex];

        private NightmareTentacle tentacle;
        private NightmareTentacle ownerTentacle;

        public ref float Timer => ref NPC.localAI[0];
        public ref float State => ref NPC.ai[3];
        //public static Asset<Texture2D> FlowTex;
        //public static Asset<Texture2D> GlowTex;
        //public static Asset<Texture2D> BodyTex;

        //public override void Load()
        //{
        //    if (Main.dedServ)
        //        return;

        //    FlowTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LaserTrail");
        //    GlowTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LaserCore");
        //    BodyTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LaserBody");
        //}

        //public override void Unload()
        //{
        //    if (Main.dedServ)
        //        return;

        //    FlowTex = null;
        //    GlowTex = null;
        //    BodyTex = null;
        //}

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ProjectileNPC[Type] = true;
            NPCID.Sets.MustAlwaysDraw[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 48;
            NPC.lifeMax = 200;
            NPC.damage = 100;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;

            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void AI()
        {
            bool isRampage = false;
            bool targetDead = false;

            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                NPC.Kill();
                return;
            }
            else
            {
                NPC.timeLeft = NPC.activeTime;
            }

            tentacle ??= new NightmareTentacle(30, TentacleColor, TentacleWidth, NightmarePlantera.tentacleTex, NightmarePlantera.tentacleFlowTex);
            ownerTentacle ??= new NightmareTentacle(30, TentacleColor, TentacleWidth, NightmarePlantera.tentacleTex, NightmarePlantera.tentacleFlowTex);

            Vector2 dir = NightmareOwner.Center - NPC.Center;
            float tentacleLength = dir.Length() / 2 / 30f;

            tentacle.rotation = NPC.rotation + MathHelper.Pi;
            tentacle.pos = NPC.Center;
            tentacle.UpdateTentacle(tentacleLength, (i) => 6 * MathF.Sin(i / 3 * Main.GlobalTimeWrappedHourly));

            ownerTentacle.rotation = NPC.rotation;
            ownerTentacle.pos = NightmareOwner.Center;
            ownerTentacle.UpdateTentacle(tentacleLength, (i) => 6 * MathF.Cos(i / 3 * Main.GlobalTimeWrappedHourly));


            if (OwnerTarget.dead)
                targetDead = true;

            if (Main.dayTime || targetDead)
            {
                Timer -= 4f;
                isRampage = true;
            }

            switch ((int)State)
            {
                default:
                case 0:
                    {
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            if (NPC.ai[0] == 0f)
                                NPC.ai[0] = (int)(NPC.Center.X / 16f);

                            if (NPC.ai[1] == 0f)
                                NPC.ai[1] = (int)(NPC.Center.X / 16f);
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (NPC.ai[0] == 0f || NPC.ai[1] == 0f)
                                Timer = 0f;

                            Timer -= 1f;
                            if (NightmareOwner.life < NightmareOwner.lifeMax * 15 / 16)
                                Timer -= 2f;

                            if (NightmareOwner.life < NightmareOwner.lifeMax * 13 / 16)
                                Timer -= 2f;

                            if (isRampage)
                                Timer -= 6f;

                            if (!targetDead && Timer <= 0f && NPC.ai[0] != 0f)
                            {
                                for (int i = 0; i < 200; i++)
                                {
                                    if (i != NPC.whoAmI && Main.npc[i].active && Main.npc[i].type == NPC.type && (Main.npc[i].velocity.X != 0f || Main.npc[i].velocity.Y != 0f))
                                        Timer = Main.rand.Next(60, 300);
                                }
                            }

                            if (Timer <= 0f)
                            {
                                Timer = Main.rand.Next(300, 600);
                                bool flag44 = false;
                                int num812 = 0;
                                while (!flag44 && num812 <= 1000)
                                {
                                    num812++;
                                    int num813 = (int)(OwnerTarget.Center.X / 16f);
                                    int num814 = (int)(OwnerTarget.Center.Y / 16f);
                                    if (NPC.ai[0] == 0f)
                                    {
                                        num813 = (int)((OwnerTarget.Center.X + NightmareOwner.Center.X) / 32f);
                                        num814 = (int)((OwnerTarget.Center.Y + NightmareOwner.Center.Y) / 32f);
                                    }

                                    if (targetDead)
                                    {
                                        num813 = (int)NightmareOwner.position.X / 16;
                                        num814 = (int)(NightmareOwner.position.Y + 400f) / 16;
                                    }

                                    int num815 = 20;
                                    num815 += (int)(100f * (num812 / 1000f));
                                    int num816 = num813 + Main.rand.Next(-num815, num815 + 1);
                                    int num817 = num814 + Main.rand.Next(-num815, num815 + 1);
                                    if (NightmareOwner.life < NightmareOwner.lifeMax * 15 / 16 && Main.rand.NextBool(6))
                                    {
                                        NPC.TargetClosest();
                                        num816 = (int)(SelfTarget.Center.X / 16f);
                                        num817 = (int)(SelfTarget.Center.Y / 16f);
                                    }

                                    try
                                    {
                                        if (WorldGen.InWorld(num816, num817) && (num812 > 500 || NightmareOwner.life < NightmareOwner.lifeMax * 15 / 16))
                                        {
                                            flag44 = true;
                                            NPC.ai[0] = num816;
                                            NPC.ai[1] = num817;
                                            NPC.netUpdate = true;
                                        }
                                    }
                                    catch
                                    { }
                                }
                            }
                        }

                        if (!(NPC.ai[0] > 0f) || !(NPC.ai[1] > 0f))
                            return;

                        float num820 = 6f;
                        if (NightmareOwner.life < NightmareOwner.lifeMax * 15 / 16)
                            num820 = 8f;

                        if (NightmareOwner.life < NightmareOwner.lifeMax * 13 / 16)
                            num820 = 10f;

                        if (Main.expertMode)
                            num820 += 1f;

                        if (Main.expertMode && NightmareOwner.life < NightmareOwner.lifeMax * 15 / 16)
                            num820 += 1f;

                        if (isRampage)
                            num820 *= 2f;

                        if (targetDead)
                            num820 *= 2f;

                        float num821 = NPC.ai[0] * 16f - 8f - NPC.Center.X;
                        float num822 = NPC.ai[1] * 16f - 8f - NPC.Center.Y;
                        float num823 = (float)Math.Sqrt(num821 * num821 + num822 * num822);
                        if (num823 < 12f + num820)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld && NPC.localAI[3] == 1f)
                            {
                                NPC.localAI[3] = 0f;
                                //TODO：以后改成生成梦魇相关内容（FTW）
                                //WorldGen.SpawnPlanteraThorns(NPC.Center);
                            }

                            NPC.velocity.X = num821;
                            NPC.velocity.Y = num822;
                        }
                        else
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient && Main.getGoodWorld)
                                NPC.localAI[3] = 1f;

                            num823 = num820 / num823;
                            NPC.velocity.X = num821 * num823;
                            NPC.velocity.Y = num822 * num823;
                        }

                    }
                    break;
                case 1:
                    {
                        float speed = NPC.velocity.Length();
                        if (speed < 16)
                        {
                            speed += 0.25f;
                        }

                        NPC.frameCounter++;
                        if (NPC.frameCounter > 5)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 3)
                                NPC.frame.Y = 0;
                        }

                        NPC.velocity = dir.SafeNormalize(Vector2.Zero) * speed;

                        if (dir.Length() < 32)
                            NPC.Kill();
                    }
                    break;
            }


            NPC.rotation = (NPC.Center - NightmareOwner.Center).ToRotation();
        }

        public override bool PreKill()
        {
            return false;
        }

        public static Color TentacleColor(float factor)
        {
            return Color.Lerp(new Color(100, 100, 130, 250), Color.Transparent, factor);
        }

        public static float TentacleWidth(float factor)
        {
            if (factor > 0.6f)
                return Helper.Lerp(40, 0, (factor - 0.6f) / 0.4f);

            return Helper.Lerp(0, 40, factor / 0.6f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            #region 废弃绘制
            //  Texture2D laserTex = GlowTex.Value;
            //  Texture2D flowTex = FlowTex.Value;
            //  Texture2D bodyTex = BodyTex.Value;

            //  Effect effect = Coralite.Instance.Assets.Request<Effect>("Effects/SolidToBlank").Value;
            //  //effect.Parameters["uColor1"].SetValue(new Color(25,25,25).ToVector3());
            //  effect.Parameters["uTime"] .SetValue( Main.GlobalTimeWrappedHourly/3);

            //  Vector2 dir = NightmareOwner.Center - NPC.Center;
            //  int laserLength = (int)dir.Length() / 3;//这个就是激光长度
            //  float height = laserTex.Height / 4f;

            //  Vector2 selfPos = NPC.Center - screenPos;

            //  var laserTarget = new Rectangle((int)selfPos.X, (int)selfPos.Y, laserLength, (int)(height * 2f));
            //  var flowTarget = new Rectangle((int)selfPos.X, (int)selfPos.Y, laserLength, (int)(height * 0.9f));

            //  var laserSource = new Rectangle(laserTex.Width * 30, 0, laserTex.Width, laserTex.Height);
            //  var flowSource = new Rectangle(flowTex.Width * 30, 0, flowTex.Width, flowTex.Height);
            //  var bodySource = BodyTex.Frame();

            //  var origin = new Vector2(0, laserTex.Height / 2);
            //  var origin2 = new Vector2(0, flowTex.Height / 2);
            //  var origin3 = new Vector2(0, bodyTex.Height / 2);

            //  spriteBatch.End();
            //  spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, default, default, effect, Main.GameViewMatrix.ZoomMatrix);

            //  spriteBatch.Draw(laserTex, laserTarget, laserSource, Color.Black, NPC.rotation + MathHelper.Pi, origin, 0, 0);
            //  spriteBatch.Draw(flowTex, flowTarget, flowSource, Color.Black, NPC.rotation + MathHelper.Pi, origin2, 0, 0);

            //  Vector2 ownerPos = NightmareOwner.Center - screenPos;

            // var laserTarget2 = new Rectangle((int)ownerPos.X, (int)ownerPos.Y, laserLength, (int)(height * 2f));
            //var  flowTarget2 = new Rectangle((int)ownerPos.X, (int)ownerPos.Y, laserLength, (int)(height * 0.9f));

            //  spriteBatch.Draw(laserTex, laserTarget2, laserSource, Color.Black, NPC.rotation, origin, 0, 0);
            //  spriteBatch.Draw(flowTex, flowTarget2, flowSource, Color.Black, NPC.rotation , origin2, 0, 0);

            //  spriteBatch.End();

            //  //effect.Parameters["uColor"].SetValue(new Color(200, 200, 200).ToVector3());
            //  spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, default, default, effect, Main.GameViewMatrix.ZoomMatrix);

            //  spriteBatch.Draw(bodyTex, laserTarget, bodySource, Color.Black, NPC.rotation + MathHelper.Pi, origin3, 0, 0);
            //  spriteBatch.Draw(bodyTex, laserTarget2, bodySource, Color.Black, NPC.rotation, origin3, 0, 0);

            //  spriteBatch.End();
            //  spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, spriteBatch.GraphicsDevice.SamplerStates[0],
            //                 spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.ZoomMatrix);
            #endregion

            tentacle?.DrawTentacle();
            ownerTentacle?.DrawTentacle();

            Texture2D mainTex = TextureAssets.Npc[NPC.type].Value;
            Rectangle frameBox = mainTex.Frame(1, 4, 0, NPC.frame.Y);
            Vector2 selforigin = frameBox.Size() / 2;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, Color.White * 0.8f, NPC.rotation + MathHelper.PiOver2, selforigin, NPC.scale, 0, 0);
            return false;
        }
    }
}
