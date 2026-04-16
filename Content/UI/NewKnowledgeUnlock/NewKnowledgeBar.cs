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
        public static int ContiuneTime = 60*4;
        public static int FadeOutTime= 10;


        public NewKnowledgeBar()
        {
            this.SetSize(new Vector2(200,120));
            OverflowHidden = false;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);


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
        }

        #region Draw

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            NewKnowledgeInfo info = NewKnowledgeState.Infos.First.Value;
            var dimensions = GetDimensions();

            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

            Effect e;

            #region 绘制线条

            Vector2 linePos = dimensions.Center() - new Vector2(0, dimensions.Height / 6-5);
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

                DrawLine(spriteBatch, linePos, 0);
            }

            #endregion

            #region 绘制背景

            e = ShaderLoader.GetShader("Waterflow");
            e.Parameters["uFlowTex"].SetValue(CoraliteAssets.Laser.WaterFlow.Value);
            e.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects);
            e.Parameters["addCount"].SetValue(0.2f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

            Vector2 BackTop = dimensions.Center() - new Vector2(0, dimensions.Height / 6);
            float backFactor;
            if (Timer <= StartTime)
                backFactor = Timer / StartTime;
            else if (Timer < StartTime + ContiuneTime)
                backFactor = 1;
            else
                backFactor = (Timer - StartTime - ContiuneTime)/ FadeOutTime;

            DrawBack(spriteBatch, BackTop, new Vector2(dimensions.Width, dimensions.Height * 2 / 3), backFactor);

            #endregion

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

            group?.DrawInUI(spriteBatch);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 center, float factor)
        {
            Texture2D tex = CoraliteNoteSystem.NoteConnectLine.Value;
            Vector2 left = center + new Vector2(-80 * factor, 0);
            Vector2 Right = center + new Vector2(80 * factor, 0);
            Vector2 dir = Right - left;

            spriteBatch.Draw(tex, left, null, Color.White, dir.ToRotation(), new Vector2(0, tex.Height / 2), new Vector2(dir.Length() / tex.Width, 64f / tex.Height), 0, 0);
        }

        public static void DrawBack(SpriteBatch spriteBatch,Vector2 top,Vector2 size,float factor)
        {
            Texture2D tex = CoraliteNoteSystem.NewTextBarBack.Value;
            Vector2 origin = new Vector2(tex.Width / 2, 0);

            spriteBatch.Draw(tex, top, null, new Color(21, 140, 176)* factor, 0, origin, size / tex.Size(), 0, 0);
        }

        public void DrawIcon(SpriteBatch spriteBatch, NewKnowledgeInfo info)
        {

        }

        public void DrawText(SpriteBatch spriteBatch, NewKnowledgeInfo info)
        {

        }

        #endregion
    }
}
