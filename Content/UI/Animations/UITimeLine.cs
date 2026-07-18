using Coralite.Content.CoraliteNotes;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.UI.Animations
{
    public class UITimeLine : UIElement
    {
        private ATex ArrowTex;
        private ATex TagTex;
        private UIAnimation animation;
        private Vector2 size;
        private Color lineColor;
        private float scale = 1;

        private bool LeftHold = false;

        public UITimeLine(UIAnimation animation, Vector2 size, ATex ArrowTex, ATex tagTex,Color lineColor)
        {
            this.animation = animation;
            this.ArrowTex = ArrowTex;
            this.size = size;
            this.lineColor = lineColor;
            this.SetSize(size);
            TagTex = tagTex;
        }

        public override void Recalculate()
        {
            this.SetSize(size);
            base.Recalculate();
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            LeftHold = true;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            LeftHold = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!LeftHold)
                return;

            //点击拖拽

            float x = Main.MouseScreen.X;
            var d = GetDimensions();

            float f = Math.Clamp((x - d.Position().X) / (d.Width - 4), 0, 1);
            int mouseTimer = (int)(f * animation.MaxTime);

            if (Main.keyState.PressingShift())//按住shift跳转到关键帧
            {
                if (animation.KeyFrames.Count > 1)
                {
                    for (int i = 0; i < animation.KeyFrames.Count - 1; i++)
                    {
                        int less = animation.KeyFrames[i];
                        int more = animation.KeyFrames[i + 1];

                        if (mouseTimer >= less && mouseTimer <= more)
                        {
                            int middle = (less + more) / 2;
                            if (mouseTimer <= middle)
                                mouseTimer = less;
                            else
                                mouseTimer = more;

                            animation.SetTimer(mouseTimer);
                        }
                    }
                }
            }
            else
                animation.SetTimer(mouseTimer);

            animation.SetPause(true);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var d = GetDimensions();

            Vector2 pos = d.Position() + new Vector2(0, d.Height / 2);

            //LineTex.Value.QuickCenteredDraw(spriteBatch, center, Color.White, 0);
            DrawLine(spriteBatch);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(CoraliteNoteSystem.HoldToDragTimeLine.Value);
                scale = Helper.Lerp(scale, 1.3f, 0.2f);
            }
            else
                scale = Helper.Lerp(scale, 1f, 0.2f);

            for (int i = 0; i < animation.KeyFrames.Count; i++)
            {
                int keyFrame = animation.KeyFrames[i];
                int keyFrameType = animation.KeyFrameTypes[i];

                float f = (float)keyFrame / animation.MaxTime;
                TagTex.Value.QuickCenteredDraw(spriteBatch, new Rectangle(keyFrameType, 0, 3, 1), pos + new Vector2(d.Width * f, 0));
            }

            ArrowTex.Value.QuickCenteredDraw(spriteBatch, pos + new Vector2(d.Width * ((float)animation.Timer / animation.MaxTime), 0), scale: scale);

            //Helper.DrawDebugFrame(this, spriteBatch);
        }

        private void DrawLine(SpriteBatch spriteBatch)
        {
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
            Texture2D tex = CoraliteNoteSystem.NoteConnectLine.Value;
            var d = GetDimensions();

            spriteBatch.Draw(tex, d.Position() + new Vector2(0, d.Height / 2), null, lineColor, 0, new Vector2(0, tex.Height / 2), new Vector2(d.Width, d.Height + 40) / tex.Size(), 0, 0);
            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
        }
    }
}
