using Coralite.Content.CoraliteNotes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationPauseButton : UIElement
    {
        public ATex buttonTex;
        public UIAnimation animation;
        private float scale = 1;

        public UIAnimationPauseButton(ATex buttonTex, UIAnimation animation)
        {
            this.buttonTex = buttonTex;
            this.animation = animation;
            this.SetSize(new Vector2(buttonTex.Width() / 2, buttonTex.Height()));
        }

        public override void Recalculate()
        {
            this.SetSize(new Vector2(buttonTex.Width() / 2, buttonTex.Height()));
            base.Recalculate();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            animation.SetPause();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
            {
                if (animation.Pause)
                    UICommon.TooltipMouseText(CoraliteNoteSystem.ClickToStart.Value);
                else
                    UICommon.TooltipMouseText(CoraliteNoteSystem.ClickToPause.Value);
                scale = Helper.Lerp(scale, 1.3f, 0.2f);
            }
            else
                scale = Helper.Lerp(scale, 1f, 0.2f);

            Vector2 center = GetDimensions().Center();
            buttonTex.Value.QuickCenteredDraw(spriteBatch, new Rectangle(animation.Pause ? 0 : 1, 0, 2, 1), center, scale: scale);

            //Helper.DrawDebugFrame(this, spriteBatch);
        }
    }
}
