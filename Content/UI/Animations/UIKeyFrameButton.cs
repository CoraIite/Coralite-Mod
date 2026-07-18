using Coralite.Content.CoraliteNotes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.UI.Animations
{
    public class UIKeyFrameButton : UIElement
    {
        public ATex buttonTex;
        public UIAnimation animation;
        private float scale = 1;
        private readonly KeyFrameSwitchType switchType;

        public enum KeyFrameSwitchType
        {
            Left,
            Right
        }

        public UIKeyFrameButton(ATex buttonTex, UIAnimation animation, KeyFrameSwitchType switchType)
        {
            this.buttonTex = buttonTex;
            this.animation = animation;
            this.SetSize(new Vector2(buttonTex.Width() / 2, buttonTex.Height()));
            this.switchType = switchType;
        }

        public override void Recalculate()
        {
            this.SetSize(new Vector2(buttonTex.Width() / 2, buttonTex.Height()));
            base.Recalculate();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            animation.SetPause(true);

            if (animation.KeyFrames.Count > 1)
            {
                int time = animation.Timer;

                for (int i = 0; i < animation.KeyFrames.Count-1; i++)
                {
                    int less = animation.KeyFrames[i];
                    int more = animation.KeyFrames[i + 1];

                    if (time == more)
                    {
                        if (switchType == KeyFrameSwitchType.Left)
                            time = less;
                        else
                            time = animation.KeyFrames[Math.Clamp(i + 2, 0, animation.KeyFrames.Count - 1)];
                        animation.SetTimer(time);
                        return;
                    }

                    if (time >= less && time < more)
                    {
                        if (switchType == KeyFrameSwitchType.Left)
                            time = less;
                        else
                            time = more;

                        animation.SetTimer(time);
                        return;
                    }
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
            {
                if (switchType == KeyFrameSwitchType.Left)
                    UICommon.TooltipMouseText(CoraliteNoteSystem.ClickToSkipPreKeyFrame.Value);
                else
                    UICommon.TooltipMouseText(CoraliteNoteSystem.ClickToSkipNextKeyFrame.Value);
                scale = Helper.Lerp(scale, 1.3f, 0.2f);
            }
            else
                scale = Helper.Lerp(scale, 1f, 0.2f);

            Vector2 center = GetDimensions().Center();
            buttonTex.Value.QuickCenteredDraw(spriteBatch, new Rectangle((int)switchType, 0, 2, 1), center, scale: scale);

            //Helper.DrawDebugFrame(this, spriteBatch);
        }
    }
}
