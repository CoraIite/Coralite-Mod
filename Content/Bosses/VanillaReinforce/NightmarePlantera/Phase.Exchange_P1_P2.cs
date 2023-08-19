using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public sealed partial class NightmarePlantera : IDrawWarp
    {
        public static Asset<Texture2D> CircleWarpTex;
        public static Asset<Texture2D> BlackBack;
        public static Asset<Texture2D> NameLine;

        public static int nameDrawTimer;

        public bool canDrawWarp;
        public float warpScale;

        public float nameScale;
        public float nameAlpha;

        public void Exchange_P1_P2()
        {
            switch ((int)State)
            {
                default:
                case 0: //蓄力，将视角拉向自身
                    {
                        NPC.velocity *= 0.9f;

                        if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                        {
                            if (NCamera.factor < 1)
                            {
                                NCamera.useScreenMove = true;
                                NCamera.factor += 0.01f;
                                if (NCamera.factor > 1)
                                    NCamera.factor = 1;
                            }
                        }

                        if (Timer > 120)
                        {
                            State++;
                            //产生第一次爆炸
                            canDrawWarp = true;
                            warpScale = 0;

                            SoundStyle st = CoraliteSoundID.BigBOOM_Item62;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, NPC.Center);
                            st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                            st.Pitch = -0.75f;
                            st.Volume -= 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }
                    }
                    break;
                case 1: //第一次炸开后产生一些星星粒子和黑色粒子
                    {
                        warpScale += 0.3f;
                        if (warpScale > 10)
                        {
                            canDrawWarp = false;
                            warpScale = 0;
                        }

                        Vector2 dir =Helper.NextVec2Dir();
                        Dust dust = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), ModContent.DustType<NightmareDust>(), dir * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;

                        if (Timer % 3 == 0)
                        {
                            dir = Helpers.Helper.NextVec2Dir();
                            dust = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), ModContent.DustType<NightmareStar>(), dir * Main.rand.NextFloat(8f, 16f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
                            dust.rotation = dir.ToRotation()+MathHelper.PiOver2;
                        }

                        if (Timer > 240)
                        {
                            State++;
                            Timer = 0;
                            SoundStyle st = CoraliteSoundID.BigBOOM_Item62;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, NPC.Center);

                            canDrawWarp = true;
                            warpScale = 0;
                            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                            {
                                NCamera.shakeLevel = 8f;
                                NCamera.ShakeDelay = 3;
                                NCamera.useShake = true;
                            }

                        }
                    }
                    break;
                case 2: //第二次炸开后产生一大堆的雾
                    {
                        if (Timer % 8 == 0)
                        {
                            SoundStyle st = CoraliteSoundID.FireBallExplosion_Item74;
                            st.Volume -= 0.2f;
                            st.Pitch -= 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        warpScale += 0.3f;
                        if (warpScale > 10)
                        {
                            canDrawWarp = false;
                            warpScale = 0;
                        }


                        for (int i = 0; i < 2; i++)
                        {
                            Color color = Main.rand.Next(0, 2) switch
                            {
                                0 => new Color(110, 68, 200),
                                _ => new Color(122, 110, 134)
                            };

                            Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir() * Main.rand.NextFloat(12, 28f),
                                CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 2f));
                        }                            

                        if (Timer > 160)
                        {
                            State++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            NPC.frame.Y = 1;

                            Main.OnPostDraw += DrawName;
                            nameScale = 0;
                            nameAlpha = 1;
                            nameDrawTimer = 0;
                            SoundStyle st = CoraliteSoundID.BigBOOM_Item62;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, NPC.Center);

                            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                            {
                                NCamera.useShake = false;
                            }

                        }
                    }
                    break;
                case 3: //开启开场文字后
                    {
                        if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                        {
                            if (NCamera.factor > 0)
                            {
                                NCamera.factor -= 0.03f;
                                if (NCamera.factor < 0)
                                {
                                    NCamera.factor = 0;
                                    NCamera.useScreenMove = false;
                                }
                            }
                        }

                        if (nameScale < 12)
                        {
                            nameScale +=2f;
                        }

                        if (Timer < 120)
                        {
                            break;
                        }

                        if (Timer == 120)
                        {
                            SkyManager.Instance.Activate("NightmareSky");

                            if (!SkyManager.Instance["NightmareSky"].IsActive())//如果这个天空没激活
                                SkyManager.Instance.Activate("NightmareSky");
                            ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;//之后每帧赋予这个倒计时2，如果npc不在了，天空自动关闭
                        }

                        nameAlpha -= 0.1f;
                        if (nameAlpha < 0.11f)
                        {
                            nameScale = 0;
                            Main.OnPostDraw -= DrawName;
                            OnExchangeToP2();
                        }
                    }
                    break;
            }

            Timer++;
        }


        public void SetPhase1Exchange()
        {
            foreach (var npc in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<NightmareHook>()))
            {
                npc.ai[3] = 1;
            }

            Music = 0; //把音乐掐掉
            NPC.dontTakeDamage = true;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Timer = 0;
            State = 0;
            Phase = (int)AIPhases.Exchange_P1_P2;
            NPC.netUpdate = true;
        }

        public void OnExchangeToP2()
        {
            Music = MusicID.OtherworldlyPlantera; //把音乐再打开
            NPC.dontTakeDamage = false;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Timer = 0;
            State = 0;
            Phase = (int)AIPhases.Dream_P2;
            NPC.netUpdate = true;
        }

        public void DrawWarp()
        {
            if (canDrawWarp)
            {
                Texture2D mainTex = CircleWarpTex.Value;

                Main.spriteBatch.Draw(mainTex, NPC.Center-Main.screenPosition, null, Color.White, 0, mainTex.Size() / 2, warpScale, 0, 0);
            }
        }

        private void DrawName(GameTime obj)
        {
            nameDrawTimer++;
            if (nameDrawTimer > 300)
                nameAlpha = 0;

            Vector2 basePos = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(BlackBack.Value, basePos, null, Color.White*nameAlpha, 0, BlackBack.Size() / 2, nameScale, 0, 0);
            if (Timer > 8)
            {
                Utils.DrawBorderStringBig(Main.spriteBatch, DisplayName.Value, basePos - new Vector2(0, 125), Color.Red * nameAlpha, 1f, 0.5f);

                Main.spriteBatch.Draw(NameLine.Value, basePos - new Vector2(0, 40), null, Color.Red * nameAlpha, 0f, NameLine.Size() / 2, 1.4f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
        }
    }
}
