using Coralite.Content.CoraliteNotes;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.NewKnowledgeUnlock
{
    public class NewKnowledgeBar : UIElement
    {
        public PRTGroup group;
        public float Timer;

        public Vector2 size;

        public static int StartTime = 20;
        public static int ContiuneTime = 60 * 4;
        public static int FadeOutTime = 10;


        public NewKnowledgeBar()
        {
            this.SetSize(new Vector2(400,240));
            OverflowHidden = false;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (Timer < StartTime + ContiuneTime)
            {
                Timer = StartTime + ContiuneTime;
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
            if (Timer> StartTime + ContiuneTime+ FadeOutTime)
            {
                Timer = 0;

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

        #region Draw

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            NewKnowledgeInfo info = NewKnowledgeState.Infos.First.Value;
            var dimensions = GetDimensions();
            Vector2 center = dimensions.Center();

            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

            Effect e;

            #region 绘制线条

            Vector2 linePos = center - new Vector2(0, dimensions.Height / 6+ 5);
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
            e.Parameters["uFlowTex"].SetValue(CoraliteAssets.Laser.WaterFlow.Value);
            e.Parameters["uTime"].SetValue(-(float)Main.timeForVisualEffects * 0.02f);
            e.Parameters["addCount"].SetValue(0.7f);
            e.Parameters["yScale2"].SetValue(0.3f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

            Vector2 BackTop = center - new Vector2(0, dimensions.Height *0.15f);
            float backFactor;
            if (Timer <= StartTime)
                backFactor = Timer / StartTime;
            else if (Timer < StartTime + ContiuneTime)
                backFactor = 1;
            else
                backFactor = 1 - (Timer - StartTime - ContiuneTime) / FadeOutTime;

            DrawBack(spriteBatch, BackTop, new Vector2(dimensions.Width, dimensions.Height * 0.66f), backFactor);

            #endregion

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

            DrawName(spriteBatch, info, center + new Vector2(0, -dimensions.Height / 3), backFactor);

            DrawIcon(spriteBatch, info, center + new Vector2(0, -dimensions.Height / 6-5), backFactor);

            DrawText(spriteBatch, info, center + new Vector2(0, dimensions.Height / 6), backFactor);


            group?.DrawInUI(spriteBatch);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 center,Color c, float factor,float width)
        {
            Texture2D tex = CoraliteNoteSystem.NoteConnectLine.Value;
            Vector2 left = center + new Vector2(-width * factor, 0);
            Vector2 Right = center + new Vector2(width * factor, 0);
            Vector2 dir = Right - left;

            spriteBatch.Draw(tex, left, null, c, dir.ToRotation(), new Vector2(0, tex.Height / 2), new Vector2(dir.Length() / tex.Width, 64f / tex.Height), 0, 0);
        }

        public static void DrawBack(SpriteBatch spriteBatch,Vector2 top,Vector2 size, float factor)
        {
            Texture2D tex = CoraliteNoteSystem.NewTextBarBack.Value;
            Vector2 origin = new Vector2(tex.Width / 2, 0);

            spriteBatch.Draw(tex, top, null, new Color(21, 140, 206)*0.3f * factor, 0, origin, size / tex.Size(), 0, 0);
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
