using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Coralite.Content.UI.Animations
{
    public abstract class UIAnimationComponent(Vector2 center) : UIElement
    {
        public int StartTime;
        public int EndTime;

        public Vector2 Offset { get; protected set; } = new Vector2(0, -20);
        public int FadeTime { get; protected set; } = 10;
        public float Rotation;{ get; protected set; }
        public Color DrawColor { get; protected set; } = Color.White;

        /// <summary>
        /// 根据当前时间轴设置结束时间
        /// </summary>
        /// <param name="ani"></param>
        public void SetEnd(UIAnimation ani)
        {
            EndTime = ani.TempTimer;
        }

        /// <summary>
        /// 设置渐入渐出的小动画时间和位移
        /// </summary>
        /// <param name="time"></param>
        /// <param name="fadeoffset"></param>
        public void SetFadeValues(int time, Vector2 fadeoffset)
        {
            FadeTime = time;
            Offset = fadeoffset;
        }

        public void SetColor(Color c) => DrawColor = c;
        public void SetRotation(float rot) => Rotation = rot;

        public override void Recalculate()
        {
            RecalculateOthers();
            this.SetCenter(center);

            base.Recalculate();
        }

        public virtual void RecalculateOthers() { }

        public virtual void UpdateAnimation(int timer)
        {

        }

        public void DrawAnimationInner(SpriteBatch spriteBatch, int timer)
        {
            if (timer < StartTime || timer > EndTime)
                return;

            Vector2 pos = GetDimensions().Center();

            float f = 1;
            if (timer < StartTime + FadeTime)//根据时间渐变消失，从0到1
                f = (float)(timer - StartTime) / FadeTime;
            if (timer > EndTime - FadeTime)//从1到0
                f = 1 - (float)(timer - (EndTime - FadeTime)) / FadeTime;

            DrawAnimation(spriteBatch, timer, pos,f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="timer"></param>
        /// <param name="center"></param>
        /// <param name="fadeFactor">根据渐变时间从0到1再到0，使用时请注意</param>
        public virtual void DrawAnimation(SpriteBatch spriteBatch, int timer, Vector2 center, float fadeFactor)
        {

        }
    }
}
