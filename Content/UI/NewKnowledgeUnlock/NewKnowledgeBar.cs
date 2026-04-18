using Coralite.Content.CoraliteNotes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
        public static int TextTime = 60 * 4;
        public static int StartTime = 20;
        public static int ContiuneTime = 60 * 5;
        public static int FadeOutTime = 10;


        public NewKnowledgeBar()
        {
            this.SetSize(new Vector2(550,240));
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
                    {
                        Timer = StartTime + ContiuneTime;
                    }

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

            }

            Timer++;

            switch (state)
            {
                default:
                case 0:
                    if (Timer>TextTime)
                    {
                        state = 1;
                        Timer = 0;
                    }
                    break;
                case 1:
                    {
                        if (Timer > StartTime + ContiuneTime + FadeOutTime)
                        {
                            Timer = 0;
                            state = 0;

                            if (NewKnowledgeState.Restart())
                            {
                                Recalculate();
                            }
                            else
                            {
                                UILoader.GetUIState<NewKnowledgeState>().Hide();
                            }
                        }
                    }
                    break;
            }
        }

        #region Draw

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                default:
                case 0:
                    DrawState0(spriteBatch);
                    break;
                case 1:
                    DrawState1(spriteBatch);
                    break;
            }

            group?.DrawInUI(spriteBatch);
            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(CoraliteNoteSystem.ClickToClose.Value);
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public void DrawState0(SpriteBatch spriteBatch)
        {
            NewKnowledgeInfo info = NewKnowledgeState.Infos.First.Value;
            var dimensions = GetDimensions();
            Vector2 center = dimensions.Center();

            float scale = 0.7f;
            float alpha = 1;
            if (Timer < TextTime / 6)
            {
                float f = Helper.BezierEase(Timer / (TextTime / 6));
                scale = f * scale;
                alpha = f;
            }
            else if (Timer > TextTime * 4 / 5)
            {
                float f = Helper.BezierEase((Timer - TextTime * 4 / 5) / (TextTime / 5));
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

            Vector2 linePos = center - new Vector2(0,  + 5);
            float lineFactor;
            if (Timer <= 10)
                lineFactor = Helper.SqrtEase(Timer / 10);
            else if (Timer < TextTime)
                lineFactor = 1 - (Timer - 10) / (TextTime-10);
            else
                lineFactor = 0;

            if (lineFactor != 0)
            {
                Effect e= ShaderLoader.GetShader("SinLine");
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
        }

        private void DrawState1(SpriteBatch spriteBatch)
        {
            NewKnowledgeInfo info = NewKnowledgeState.Infos.First.Value;
            var dimensions = GetDimensions();
            Vector2 center = dimensions.Center();

            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

            Effect e;

            Vector2 BackTop = center - new Vector2(0, dimensions.Height * 0.15f);
            float backFactor;
            if (Timer <= StartTime)
                backFactor = Timer / StartTime;
            else if (Timer < StartTime + ContiuneTime)
                backFactor = 1;
            else
                backFactor = 1 - (Timer - StartTime - ContiuneTime) / FadeOutTime;

            Texture2D tex = CoraliteNoteSystem.CoralBack.Value;

            spriteBatch.Draw(tex, BackTop + new Vector2(0, -35), null, Color.White * 0.8f * backFactor, 0, new Vector2(tex.Width / 2, 0), 0.25f, 0, 0);

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

            e = ShaderLoader.GetShader("Waterflow");
            e.Parameters["uFlowTex"].SetValue(CoraliteNoteSystem.Water1.Value);
            e.Parameters["uTime"].SetValue(-(float)Main.timeForVisualEffects * 0.02f);
            e.Parameters["addCount"].SetValue(0.1f);
            e.Parameters["addCount2"].SetValue(3.1f);
            e.Parameters["yScale2"].SetValue(0.3f);
            e.Parameters["uResolution"].SetValue(new Vector2(dimensions.Width, dimensions.Height * 0.66f));


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

            DrawBack(spriteBatch, BackTop, new Vector2(dimensions.Width, dimensions.Height * 0.66f), backFactor);

            #endregion

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);


            DrawName(spriteBatch, info, center + new Vector2(0, -dimensions.Height / 3), backFactor);

            DrawIcon(spriteBatch, info, center + new Vector2(0, -dimensions.Height / 6 - 5), backFactor);

            DrawText(spriteBatch, info, center + new Vector2(0, dimensions.Height / 6), backFactor);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 center,Color c, float factor,float width)
        {
            Texture2D tex = CoraliteNoteSystem.NoteConnectLine.Value;
            Vector2 left = center + new Vector2(-width * factor, 0);
            Vector2 Right = center + new Vector2(width * factor, 0);
            Vector2 dir = Right - left;

            spriteBatch.Draw(tex, left, null, c, dir.ToRotation(), new Vector2(0, tex.Height / 2), new Vector2(dir.Length() / tex.Width, 84f / tex.Height), 0, 0);
        }

        public static void DrawBack(SpriteBatch spriteBatch,Vector2 top,Vector2 size, float factor)
        {
            Texture2D tex = CoraliteNoteSystem.NewTextBarBack.Value;
            Vector2 origin = new Vector2(tex.Width / 2, 0);

            spriteBatch.Draw(tex, top, null, new Color(21, 140, 206)*0.6f * factor, 0, origin, size / tex.Size(), 0, 0);
        }

        public void DrawIcon(SpriteBatch spriteBatch, NewKnowledgeInfo info, Vector2 center, float factor)
        {
            Texture2D tex = info.knowledge.Texture2D.Value;
            spriteBatch.Draw(tex, center, null, Color.White * factor, 0, tex.Size() / 2, 1, 0, 0);
        }

        public void DrawText(SpriteBatch spriteBatch, NewKnowledgeInfo info, Vector2 center, float factor)
        {
            Utils.DrawBorderString(spriteBatch, info.knowledge.Description.Value, center, Color.White*factor, 1, 0.5f, 0.5f);
        }

        public void DrawName(SpriteBatch spriteBatch, NewKnowledgeInfo info, Vector2 center, float factor)
        {
            Utils.DrawBorderStringBig(spriteBatch, info.knowledge.KnowledgeName.Value, center, info.color * factor, 0.6f, 0.5f, 0.5f);
        }

        #endregion
    }
}
