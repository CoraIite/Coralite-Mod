using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.Animations
{
    public class UITimeLine : UIElement
    {
        private ATex LineTex;
        private ATex ArrowTex;
        private ATex TagTex;
        private UIAnimation animation;

        private bool LeftHold = false;

        public UITimeLine(UIAnimation animation, ATex LineTex, ATex ArrowTex, ATex tagTex)
        {
            this.animation = animation;
            this.LineTex = LineTex;
            this.ArrowTex = ArrowTex;
            Vector2 size = LineTex.Size();
            size.Y += 20;
            this.SetSize(size);
            TagTex = tagTex;
        }

        public override void Recalculate()
        {
            Vector2 size = LineTex.Size();
            size.Y += 20;
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

            Vector2 center = d.Center();
            Vector2 pos = d.Position() + new Vector2(0, d.Height / 2);

            LineTex.Value.QuickCenteredDraw(spriteBatch, center, Color.White, 0);

            foreach (var keyFrame in animation.KeyFrames)
            {
                float f = (float)keyFrame / animation.MaxTime;
                TagTex.Value.QuickCenteredDraw(spriteBatch, pos + new Vector2(d.Width * f, 0));
            }

            ArrowTex.Value.QuickCenteredDraw(spriteBatch, pos + new Vector2(d.Width * ((float)animation.Timer / animation.MaxTime), 0));

            //Helper.DrawDebugFrame(this, spriteBatch);
        }
    }
}
