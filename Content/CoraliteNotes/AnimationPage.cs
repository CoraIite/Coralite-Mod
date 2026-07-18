using Coralite.Content.UI.Animations;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.CoraliteNotes
{
    public abstract class AnimationPage() : KnowledgePage
    {
        public abstract UIAnimation GetAnimation();

        /// <summary>
        /// 时间轴的颜色
        /// </summary>
        public virtual Color LineColor { get => new Color(115, 50, 75); }

        private UIAnimation anmi;

        public sealed override void OnInitialize()
        {
            InitOthers();

            InitElements(true);
        }

        private void InitElements(bool newAnimation)
        {
            if (newAnimation)
            {
                UIAnimation animation = GetAnimation();
                anmi = animation;
            }

            anmi.SetSize(Vector2.Zero, 1, 1);
            anmi.SetCenter(Vector2.Zero, Vector2.Zero);
            Append(anmi);

            //播放按钮
            UIAnimationPauseButton button1 = new UIAnimationPauseButton(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimePauseButton", ReLogic.Content.AssetRequestMode.ImmediateLoad), anmi);
            button1.SetCenter(new Vector2(0, -40), new Vector2(0.5f, 1f));
            Append(button1);

            //快进按钮
            UIKeyFrameButton button2 = new UIKeyFrameButton(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteKeyFrameButton", ReLogic.Content.AssetRequestMode.ImmediateLoad), anmi, UIKeyFrameButton.KeyFrameSwitchType.Left);
            button2.SetCenter(new Vector2(-80, -40), new Vector2(0.5f, 1f));
            Append(button2);
            UIKeyFrameButton button3 = new UIKeyFrameButton(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteKeyFrameButton", ReLogic.Content.AssetRequestMode.ImmediateLoad), anmi, UIKeyFrameButton.KeyFrameSwitchType.Right);
            button3.SetCenter(new Vector2(80, -40), new Vector2(0.5f, 1f));
            Append(button3);

            //时间轴
            UITimeLine line = new UITimeLine(anmi,
                new Vector2(550, 60),
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLineArrow", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLineTag", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                LineColor
                );
            line.SetCenter(new Vector2(0, -110), new Vector2(0.5f, 1));
            Append(line);
        }

        public virtual void InitOthers()
        {

        }

        public override void Recalculate()
        {
            base.Recalculate();

            RemoveAllChildren();
#if DEBUG
            InitElements(true);
#else
            InitElements(false);
#endif
        }
    }
}
