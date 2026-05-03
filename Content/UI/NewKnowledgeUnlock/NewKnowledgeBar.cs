using Coralite.Content.CoraliteNotes;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.UI.NewKnowledgeUnlock
{
    public class NewKnowledgeBar : UIElement
    {
        public PRTGroup group;
        public float Timer;

        public Vector2 size;

        public int state;
        public const int TextTime = 60 * 4;
        public const int StartTime = 20;
        public const int ContiuneTime = 60 * 5;
        public const int FadeOutTime = 10;

        public SlotId soundSlot;
        public SlotId flipSlot;

        public NewKnowledgeBar()
        {
            this.SetSize(new Vector2(550, 240));
            OverflowHidden = false;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            switch (state)
            {
                default:
                case 0:
                    state = 1;
                    Timer = 0;
                    break;
                case 1:
                    if (Timer < StartTime + ContiuneTime)
                        Timer = StartTime + ContiuneTime;

                    break;
                case 2:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateBar();
        }

        public void UpdateBar()
        {
            if (Timer == 0)//初始化
            {
                if (NewKnowledgeState.Infos.Count > 0)
                {
                    NewKnowledgeInfo info = NewKnowledgeState.Infos.First.Value;

                    if (state == 0 && !info.ShowFirstPhase)
                        state = 1;
                }
            }

            Timer++;


            switch (state)
            {
                default:
                case 0:
                    SpawnBubble_State0();

                    if (Timer == 1)
                        Helper.PlayPitched(AssetDirectory.Sounds.CoraliteNote + "PageFlips", 0.8f, 0, maxInstances: 1);
                    if (Timer == 20 * 3)
                        flipSlot = Helper.PlayPitched(AssetDirectory.Sounds.CoraliteNote + "CoralGrowX4", 0.7f, 0);

                    if (Timer > TextTime)
                    {
                        state = 1;
                        Timer = 0;
                    }
                    break;
                case 1:
                    {
                        SpawnBubble_State1();
                        if (Timer == 1)
                        {
                            Helper.PlayPitched(AssetDirectory.Sounds.CoraliteNote + "BubblesLot", 0.8f, 0, maxInstances: 1);
                            soundSlot = Helper.PlayPitched(AssetDirectory.Sounds.CoraliteNote + "BubblesFew", 0.8f, 0, soundAdjust: (st) => st.IsLooped = true, maxInstances: 1);
                        }
                        if (Timer > StartTime + ContiuneTime)
                        {
                            if (SoundEngine.TryGetActiveSound(flipSlot, out ActiveSound result1))
                            {
                                result1.Volume -= 1f / FadeOutTime;
                                if (result1.Volume <= 0)
                                    result1.Stop();
                            }
                            if (SoundEngine.TryGetActiveSound(soundSlot, out ActiveSound result))
                            {
                                result.Volume -= 1f / FadeOutTime;
                                if (result.Volume <= 0)
                                    result.Stop();
                            }
                        }
                        if (Timer > StartTime + ContiuneTime + FadeOutTime)
                        {
                            Timer = 0;
                            state = 0;

                            if (SoundEngine.TryGetActiveSound(soundSlot, out ActiveSound result))
                                result.Stop();

                            if (NewKnowledgeState.Restart())
                                Recalculate();
                            else
                                state = 2;
                        }
                    }
                    break;
                case 2:
                    {
                        if (!group.Any())
                        {
                            UILoader.GetUIState<NewKnowledgeState>().Hide();
                            Reset();
                        }
                    }
                    break;
            }

            group?.Update();
        }

        public void Reset()
        {
            Timer = 0;
            state = 0;
            group?.Clear();
        }

        public void SpawnBubble_State0()
        {
            group ??= new PRTGroup();
            if (Timer < 10)
            {
                var dimensions = GetDimensions();

                float lineFactor = Helper.SqrtEase(Timer / 10);
                float width = dimensions.Width * 0.4f;
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 pos = dimensions.Center() - new Vector2(lineFactor * i * width, 5) + Main.rand.NextVector2Circular(20, 20);

                    group.NewParticle<NewKnowledgeBubble>(pos, new Vector2(-i * Main.rand.NextFloat(1, 2), Main.rand.NextFloat(-1, 2)));
                }
            }
            if (Timer % 6 == 0 && Main.rand.NextBool())
            {
                var dimensions = GetDimensions();

                Vector2 pos = dimensions.Center() - new Vector2(Main.rand.NextFloat(-1, 1) * dimensions.Width * 0.4f, 5) + Main.rand.NextVector2Circular(20, 20);

                group.NewParticle<NewKnowledgeBubble>(pos, new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-1, 1)));
            }
        }

        public void SpawnBubble_State1()
        {
            group ??= new PRTGroup();

            if (Timer < StartTime)
            {
                var dimensions = GetDimensions();

                float lineFactor = Timer / StartTime;
                float width = dimensions.Width * 0.4f;
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 pos = dimensions.Center() - new Vector2(lineFactor * i * width, dimensions.Height / 6 + 5) + Main.rand.NextVector2Circular(20, 20);

                    group.NewParticle<NewKnowledgeBubble>(pos, new Vector2(-i * Main.rand.NextFloat(0.2f, 2), Main.rand.NextFloat(-1, 2)));
                }
            }
            else if (Timer < StartTime + ContiuneTime - 40)
            {
                if (Timer % 4 == 0 && Main.rand.NextBool())
                {
                    var dimensions = GetDimensions();

                    Vector2 pos = dimensions.Center() - new Vector2(Main.rand.NextFloat(-1, 1) * dimensions.Width * 0.4f, Main.rand.NextFloat(-dimensions.Height / 2, dimensions.Height / 6 + 5)) + Main.rand.NextVector2Circular(20, 20);

                    group.NewParticle<NewKnowledgeBubble>(pos, new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-1, 1)));
                }
            }
        }

        #region Draw

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            group?.DrawInUI(spriteBatch);

            switch (state)
            {
                default:
                case 0:
                    DrawState0(spriteBatch);
                    break;
                case 1:
                    DrawState1(spriteBatch);
                    break;
                case 2:
                    return;
            }

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(CoraliteNoteSystem.ClickToClose.Value);
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public void DrawState0(SpriteBatch spriteBatch)
        {
            if (NewKnowledgeState.Infos.Count < 1)
                return;

            NewKnowledgeInfo info = NewKnowledgeState.Infos.First.Value;
            var dimensions = GetDimensions();
            Vector2 center = dimensions.Center();
            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            float scale = 0.7f;
            float alpha = 1;
            if (Timer < TextTime / 8)
            {
                float f = Helper.HeavyEase(Timer / (TextTime / 8));
                scale = f * scale;
                alpha = f;
            }
            else if (Timer > TextTime * 6 / 7)
            {
                float f = Helper.BezierEase((Timer - TextTime * 6 / 7) / (TextTime / 7));
                scale = (1 - f) * scale;
                alpha = 1 - f;
            }

            Utils.DrawBorderStringBig(spriteBatch, KnowledgeSystem.NewKnowledgeUnlock.Value, center, info.color * alpha, scale, 0.5f, 1f);

            //绘制珊瑚笔记图标
            Texture2D tex = CoraliteNoteSystem.CoraliteNoteOpenAnmi.Value;

            int frameY;
            if (Timer < 20 * 3)
                frameY = (int)Timer / 3;
            else
                frameY = 20 + (int)(Timer / 3) % 35;

            Rectangle frame = tex.Frame(1, 56, 0, frameY);
            Vector2 origin = new Vector2(frame.Width / 2, 0);

            Main.spriteBatch.Draw(tex, center + new Vector2(0, 10), frame, Color.White * alpha, 0, origin, 1, 0, 0);

            #region 绘制线条

            Vector2 linePos = center - new Vector2(0, 5);
            float lineFactor;
            if (Timer <= 10)
                lineFactor = Helper.SqrtEase(Timer / 10);
            else if (Timer < TextTime)
                lineFactor = 1 - (Timer - 10) / (TextTime - 10);
            else
                lineFactor = 0;

            if (lineFactor != 0)
            {
                Effect e = ShaderLoader.GetShader("SinLine");
                e.Parameters["flowPercent"].SetValue(0.06f);
                float time = (float)Main.timeForVisualEffects * 0.02f;
                float flowTime = -(float)Main.timeForVisualEffects * 0.003f;
                e.Parameters["uTime"].SetValue(time);
                e.Parameters["uFlowTime"].SetValue(flowTime);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

                DrawLine(spriteBatch, linePos, info.color, lineFactor, dimensions.Width / 2);
            }

            #endregion

            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

            float alpha2 = 1;
            if (Timer < TextTime / 6)
            {
                alpha2 = Helper.BezierEase(Timer / (TextTime / 6));
            }

            DrawIcon(spriteBatch, info, linePos, alpha2);
        }

        private void DrawState1(SpriteBatch spriteBatch)
        {
            if (NewKnowledgeState.Infos.Count < 1)
                return;

            NewKnowledgeInfo info = NewKnowledgeState.Infos.First.Value;
            var dimensions = GetDimensions();
            Vector2 center = dimensions.Center();

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

            Effect e;

            Vector2 BackTop = center - new Vector2(0, dimensions.Height * 0.15f);
            float coralEx = -35;
            float backFactor;
            if (Timer <= StartTime)
            {
                float f = Timer / StartTime;
                backFactor = Helper.BezierEase(f);
                coralEx *= Helper.HeavyEase(f);
            }
            else if (Timer < StartTime + ContiuneTime)
                backFactor = 1;
            else
                backFactor = 1 - (Timer - StartTime - ContiuneTime) / FadeOutTime;

            //绘制珊瑚
            Texture2D tex = CoraliteNoteSystem.CoralBack.Value;

            spriteBatch.Draw(tex, BackTop + new Vector2(0, coralEx), null, Color.White * 0.8f * backFactor, 0, new Vector2(tex.Width / 2, 0), 0.25f, 0, 0);

            #region 绘制线条

            Vector2 linePos = center - new Vector2(0, dimensions.Height / 6 + 5);
            float lineFactor;
            if (Timer <= StartTime)
                lineFactor = Helper.SqrtEase(Timer / StartTime);
            else if (Timer < StartTime + ContiuneTime)
                lineFactor = 1 - (Timer - StartTime) / ContiuneTime;
            else
                lineFactor = 0;

            if (lineFactor != 0)
            {
                e = ShaderLoader.GetShader("SinLine");
                e.Parameters["flowPercent"].SetValue(0.06f);
                float time = (float)Main.timeForVisualEffects * 0.02f;
                float flowTime = -(float)Main.timeForVisualEffects * 0.003f;
                e.Parameters["uTime"].SetValue(time);
                e.Parameters["uFlowTime"].SetValue(flowTime);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

                DrawLine(spriteBatch, linePos, info.color, lineFactor, dimensions.Width / 2);
            }

            #endregion

            #region 绘制背景

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

            DrawBack(spriteBatch, BackTop, new Vector2(dimensions.Width, dimensions.Height * 0.66f), backFactor, Color.Black * 0.7f);

            e = ShaderLoader.GetShader("Waterflow");
            e.Parameters["uFlowTex"].SetValue(CoraliteNoteSystem.Water1.Value);
            e.Parameters["uTime"].SetValue(-(float)Main.timeForVisualEffects * 0.02f);
            e.Parameters["addCount"].SetValue(0.1f);
            e.Parameters["addCount2"].SetValue(3.1f);
            e.Parameters["yScale2"].SetValue(0.3f);
            e.Parameters["uResolution"].SetValue(new Vector2(dimensions.Width, dimensions.Height * 0.66f));


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

            DrawBack(spriteBatch, BackTop, new Vector2(dimensions.Width, dimensions.Height * 0.66f), backFactor, new Color(21, 120, 176) * 0.6f);

            #endregion

            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);


            DrawName(spriteBatch, info, center + new Vector2(0, backFactor * -dimensions.Height / 3), backFactor);

            Vector2 iconPos = center + new Vector2(0, -dimensions.Height / 6 - 5);
            float backFactor2 = backFactor;

            if (Timer <= StartTime)
            {
                iconPos = Vector2.SmoothStep(center - new Vector2(0, 5), iconPos, Timer / StartTime);

                backFactor2 = 1;
            }

            DrawIcon(spriteBatch, info, iconPos, backFactor2);

            DrawText(spriteBatch, info, center + new Vector2(0, dimensions.Height / 6), backFactor);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 center, Color c, float factor, float width)
        {
            Texture2D tex = CoraliteNoteSystem.NoteConnectLine.Value;
            Vector2 left = center + new Vector2(-width * factor, 0);
            Vector2 Right = center + new Vector2(width * factor, 0);
            Vector2 dir = Right - left;

            spriteBatch.Draw(tex, left, null, c, dir.ToRotation(), new Vector2(0, tex.Height / 2), new Vector2(dir.Length() / tex.Width, 84f / tex.Height), 0, 0);
        }

        public static void DrawBack(SpriteBatch spriteBatch, Vector2 top, Vector2 size, float factor, Color color)
        {
            Texture2D tex = CoraliteNoteSystem.NewTextBarBack.Value;
            Vector2 origin = new Vector2(tex.Width / 2, 0);

            spriteBatch.Draw(tex, top, null, color * factor, 0, origin, size / tex.Size(), 0, 0);
        }

        public static void DrawIcon(SpriteBatch spriteBatch, NewKnowledgeInfo info, Vector2 center, float factor)
        {
            Texture2D tex = info.knowledge.Texture2D.Value;
            spriteBatch.Draw(tex, center, null, Color.White * factor, 0, tex.Size() / 2, 1, 0, 0);
        }

        public static void DrawText(SpriteBatch spriteBatch, NewKnowledgeInfo info, Vector2 center, float factor)
        {
            string text = info.OverrideText ?? info.knowledge.Description.Value;

            Utils.DrawBorderString(spriteBatch, text, center, Color.White * factor, 1, 0.5f, 0.5f);
        }

        public static void DrawName(SpriteBatch spriteBatch, NewKnowledgeInfo info, Vector2 center, float factor)
        {
            string name = info.OverrideName == null ? info.knowledge.KnowledgeName.Value : info.OverrideName.Value;

            Utils.DrawBorderStringBig(spriteBatch, name, center, info.color * factor, 0.6f, 0.5f, 0.5f);
        }

        #endregion
    }

    public class NewKnowledgeBubble : Particle
    {
        public override string Texture => AssetDirectory.CoraliteNote + Name;

        public int SpawnTime = Main.rand.Next(12, 20);
        public int FloatTime = Main.rand.Next(30, 70);

        public override void SetProperty()
        {
            int scale = Main.rand.Next(0, 4);
            Frame = new Rectangle(scale, 0, 4, 8);
            Color = Color.White * Main.rand.NextFloat(0.5f, 1f);
        }

        public override void AI()
        {
            Opacity++;
            if (Opacity < SpawnTime)
            {
                if (Opacity % 3 == 0 && Frame.Y < 3)
                    Frame.Y++;

                Velocity *= 0.97f;
            }
            else if (Opacity < SpawnTime + FloatTime)
            {
                if (Velocity.Y > -3.5f)
                {
                    Velocity.Y -= 0.06f;
                }

                Velocity.X += 0.06f * MathF.Sin(Opacity * 0.2f);
                if (Opacity > SpawnTime + FloatTime - 20)
                {
                    Rotation += (SpawnTime % 2 == 0 ? 1 : -1) * FloatTime / 30 * 0.08f;
                }
            }
            else
            {
                Velocity *= 0.8f;
                if (Opacity % 3 == 0)
                    Frame.Y++;

                if (Frame.Y > 7)
                {
                    active = false;
                }
            }
        }

        public override void DrawInUI(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            Rectangle frame = tex.Frame(Frame.Width, Frame.Height, Frame.X, Frame.Y);
            spriteBatch.Draw(tex, Position, frame, Color, Rotation, frame.Size() / 2, Scale, 0, 0);
        }
    }
}
