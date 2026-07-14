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

        public override void OnInitialize()
        {
            UIAnimation animation = GetAnimation();
            anmi = animation;
            animation.SetSize(Vector2.Zero, 1, 1);
            animation.SetCenter(Vector2.Zero, Vector2.One / 2);
            Append(animation);

            //播放按钮
            UIAnimationPauseButton button1 = new UIAnimationPauseButton(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimePauseButton",ReLogic.Content.AssetRequestMode.ImmediateLoad), animation);
            button1.SetCenter(Bottom + new Vector2(0, -40));
            Append(button1);

            //时间轴
            UITimeLine line = new UITimeLine(animation,
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLine", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLineArrow", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLineTag", ReLogic.Content.AssetRequestMode.ImmediateLoad)
                );
            line.SetCenter(Bottom + new Vector2(0, -80));
            Append(line);

        }

        public override void Recalculate()
        {
            base.Recalculate();

#if DEBUG
            RemoveAllChildren();


            //DEBUG模式都要重新加载动画

            RemoveChild(anmi);
            UIAnimation animation = GetAnimation();
            anmi = animation;
            animation.SetSize(Vector2.Zero, 1, 1);
            animation.SetCenter(Vector2.Zero, Vector2.One / 2);
            Append(animation);

            //播放按钮
            UIAnimationPauseButton button1 = new UIAnimationPauseButton(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimePauseButton", ReLogic.Content.AssetRequestMode.ImmediateLoad), animation);
            button1.SetCenter(new Vector2(0, -40), new Vector2(0.5f, 1f));
            Append(button1);

            //时间轴
            UITimeLine line = new UITimeLine(animation,
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLine", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLineArrow", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "NoteTimeLineTag", ReLogic.Content.AssetRequestMode.ImmediateLoad)
                );

            line.SetCenter(new Vector2(0,-80),new Vector2(0.5f,1) );
            Append(line);


#endif

        }
    }
}
