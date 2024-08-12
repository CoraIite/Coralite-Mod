using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;

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
                case 0: //蓄力，将视角拉向自身
                    {
                        NPC.velocity *= 0.9f;

                        if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                        {
                            if (NCamera.factor < 1)
                            {
                                NCamera.useScreenMove = true;
                                NCamera.factor += 0.02f;
                                if (NCamera.factor > 1)
                                    NCamera.factor = 1;
                            }
                        }

                        if (Timer > 80)
                        {
                            State++;
                            //产生第一次爆炸
                            canDrawWarp = true;
                            warpScale = 0;

                            NCamera.useShake = true;
                            NCamera.shakeLevel = 3f;
                            NCamera.shakeDelay = 2;

                            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, NPC.Center, pitch: -0.5f);
                            Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Dash_Item160, NPC.Center,volumeAdjust:-0.2f,pitchAdjust:-0.75f);
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

                        NPC.rotation += Main.rand.NextFloat(-0.2f, 0.3f);
                        Vector2 dir = Helper.NextVec2Dir();
                        Dust dust = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), ModContent.DustType<NightmareDust>(), dir * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;

                        if (Timer % 3 == 0)
                        {
                            dir = Helper.NextVec2Dir();
                            dust = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), ModContent.DustType<NightmareStar>(), dir * Main.rand.NextFloat(8f, 16f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
                            dust.rotation = dir.ToRotation() + MathHelper.PiOver2;

                            dir = Helper.NextVec2Dir();
                            dust = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), DustID.VilePowder, dir * Main.rand.NextFloat(8f, 16f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 1.3f));
                        }

                        if (Timer > 140)
                        {
                            State++;
                            Timer = 0;
                            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, NPC.Center, pitch: -0.5f);

                            canDrawWarp = true;
                            warpScale = 0;
                            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                            {
                                NCamera.shakeLevel = 8f;
                                NCamera.shakeDelay = 3;
                                NCamera.useShake = true;
                            }
                        }
                    }
                    break;
                case 2: //第二次炸开后产生一大堆的雾
                    {
                        if (Timer % 8 == 0)
                            Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_Item74, NPC.Center, volumeAdjust: -0.4f, pitchAdjust: -0.4f);

                        warpScale += 0.3f;
                        if (warpScale > 10)
                        {
                            canDrawWarp = false;
                            warpScale = 0;
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            Color color = Main.rand.Next(0, 2) switch
                            {
                                0 => new Color(110, 68, 200),
                                _ => new Color(122, 110, 134)
                            };

                            Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir(6, 36f),
                                CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 3f));
                        }

                        if (Timer > 100)
                        {
                            State++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            NPC.frame.Y = 1;

                            Main.OnPostDraw += DrawName;
                            nameScale = 0;
                            nameAlpha = 1;
                            nameDrawTimer = 0;
                            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, NPC.Center, pitch: -0.5f);

                            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                            {
                                NCamera.useShake = false;
                            }

                            //Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/NightmarePlantera");

                            Music = MusicID.OtherworldlyPlantera; //把音乐再打开
                        }
                    }
                    break;
                case 3: //开启开场文字后
                    {
                        if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                        {
                            if (NCamera.factor > 0)
                            {
                                NCamera.factor -= 0.02f;
                                if (NCamera.factor < 0)
                                {
                                    NCamera.factor = 0;
                                    NCamera.useScreenMove = false;
                                }
                            }
                        }

                        if (nameScale < 16)
                            nameScale += 1f;

                        if (Timer == 30)
                        {
                            SkyManager.Instance.Activate("NightmareSky");

                            if (!SkyManager.Instance["NightmareSky"].IsActive())//如果这个天空没激活
                                SkyManager.Instance.Activate("NightmareSky");
                            ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;
                            ((NightmareSky)SkyManager.Instance["NightmareSky"]).color = lightPurple;
                        }

                        ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;

                        if (Timer < 120)
                            break;

                        nameAlpha -= 0.05f;
                        if (nameAlpha < 0.06f)
                        {
                            nameScale = 0;
                            Main.OnPostDraw -= DrawName;
                            OnExchangeToP2();
                        }
                    }
                    break;
                default:
                    OnExchangeToP2();
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
            MoveCount = 0;
            NPC.dontTakeDamage = false;
            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
            {
                NCamera.useShake = false;
                NCamera.useScreenMove = false;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Phase = (int)AIPhases.Dream_P2;
            SetPhase2States();
        }

        public void DrawWarp()
        {
            if (canDrawWarp)
            {
                Texture2D mainTex = CircleWarpTex.Value;

                Main.spriteBatch.Draw(mainTex, NPC.Center - Main.screenPosition, null, Color.White, 0, mainTex.Size() / 2, warpScale, 0, 0);
            }
        }

        private void DrawName(GameTime obj)
        {
            nameDrawTimer++;
            if (nameDrawTimer > 300)
            {
                Main.OnPostDraw -= DrawName;
                nameAlpha = 0;
            }

            Vector2 basePos = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(BlackBack.Value, basePos, null, Color.White * nameAlpha, 0, BlackBack.Size() / 2, nameScale, 0, 0);
            if (Timer > 8)
            {
                Utils.DrawBorderStringBig(Main.spriteBatch, DisplayName.Value, basePos, Color.Red * nameAlpha, 1.6f, 0.5f, 0.5f);

                Main.spriteBatch.Draw(NameLine.Value, basePos + new Vector2(0, 80), null, Color.Red * nameAlpha, 0f, NameLine.Size() / 2, 1.3f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(NameLine.Value, basePos - new Vector2(0, 120), null, Color.Red * nameAlpha, 0f, NameLine.Size() / 2, 1.3f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
        }
    }
}
