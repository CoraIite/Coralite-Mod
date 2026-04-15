using Coralite.Content.CoraliteNotes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.KeySystem;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.NewKnowledgeUnlock
{
    public class NewKnowledgeBar : UIElement
    {
        public PRTGroup group;
        public int Timer;

        public Vector2 size;


        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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

            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

            spriteBatch.End();
            Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);

            Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, spriteBatch.GraphicsDevice.ScissorRectangle);
            spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = EffectLoader.OverflowHiddenRasterizerState;
            
            Effect e = ShaderLoader.GetShader("SinLine");
            e.Parameters["flowPercent"].SetValue(0.06f);
            float time = (float)Main.timeForVisualEffects * 0.02f;
            float flowTime = -(float)Main.timeForVisualEffects * 0.003f;
            e.Parameters["uTime"].SetValue(time);
            e.Parameters["uFlowTime"].SetValue(flowTime);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

            //绘制线条
            DrawLine(spriteBatch,Vector2.Zero,0);

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

            group?.DrawInUI(spriteBatch);
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 center, float factor)
        {
            Texture2D tex = CoraliteNoteSystem.NoteConnectLine.Value;
            Vector2 left = center + new Vector2(-80 * factor, 0);
            Vector2 Right = center + new Vector2(80 * factor, 0);
            Vector2 dir = Right - left;

            spriteBatch.Draw(tex, left, null, Color.White, dir.ToRotation(), new Vector2(0, tex.Height / 2), new Vector2(dir.Length() / tex.Width, 64f / tex.Height), 0, 0);
        }

        public void DrawBack(SpriteBatch spriteBatch)
        {

        }

        public void DrawIcon(SpriteBatch spriteBatch)
        {

        }

        public void DrawText(SpriteBatch spriteBatch)
        {

        }

        #endregion
    }
}
