using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
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

            //点击拖拽
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

            ArrowTex.Value.QuickCenteredDraw(spriteBatch, pos + new Vector2(d.Width * ((float)animation.Timer/animation.MaxTime), 0));

            //Helper.DrawDebugFrame(this, spriteBatch);
        }
    }
}
