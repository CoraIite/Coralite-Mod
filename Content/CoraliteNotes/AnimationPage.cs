using Coralite.Content.UI.Animations;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Content.CoraliteNotes
{
    public abstract class AnimationPage() : KnowledgePage
    {
        public abstract UIAnimation GetAnimation();

        private UIAnimation anmi;

        public sealed override void OnInitialize()
        {
            InitOthers();

            InitElements();
        }

        private void InitElements()
        {
            UIAnimation animation = GetAnimation();
            anmi = animation;
            animation.SetSize(Vector2.Zero, 1, 1);
            animation.SetCenter(Vector2.Zero, Vector2.Zero);
            Append(animation);

            //播放按钮
            UIAnimationPauseButton button1 = new UIAnimationPauseButton(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimePauseButton", ReLogic.Content.AssetRequestMode.ImmediateLoad), animation);
            button1.SetCenter(new Vector2(0, -40), new Vector2(0.5f, 1f));
            Append(button1);

            //快进按钮
            UIKeyFrameButton button2 = new UIKeyFrameButton(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteKeyFrameButton", ReLogic.Content.AssetRequestMode.ImmediateLoad), animation, UIKeyFrameButton.KeyFrameSwitchType.Left);
            button2.SetCenter(new Vector2(-80, -40), new Vector2(0.5f, 1f));
            Append(button2);
            UIKeyFrameButton button3 = new UIKeyFrameButton(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteKeyFrameButton", ReLogic.Content.AssetRequestMode.ImmediateLoad), animation, UIKeyFrameButton.KeyFrameSwitchType.Right);
            button3.SetCenter(new Vector2(80, -40), new Vector2(0.5f, 1f));
            Append(button3);

            //时间轴
            UITimeLine line = new UITimeLine(animation,
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLine", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLineArrow", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLineTag", ReLogic.Content.AssetRequestMode.ImmediateLoad)
                );
            line.SetCenter(new Vector2(0, -90), new Vector2(0.5f, 1));
            Append(line);
        }

        public virtual void InitOthers()
        {

        }

        public override void Recalculate()
        {
            base.Recalculate();

#if DEBUG
            RemoveAllChildren();

            //DEBUG模式都要重新加载动画
            InitElements();

#endif

        }
    }
}
