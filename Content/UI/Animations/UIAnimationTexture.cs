using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationTexture : UIAnimationComponent
    {
        private readonly ATex tex;

        private float rotation;
        public Vector2 center;
        private Vector2 offset=new Vector2(0,-20);

        private Color drawColor = Color.White;
        private int fadeTime = 10;

        public UIAnimationTexture(string texturePath,Vector2 center)
        {
            tex = ModContent.Request<Texture2D>(texturePath);
            this.center = center;
        }

        public override void Recalculate()
        {
            this.SetSize(tex.Size());
            this.SetCenter(center);

            base.Recalculate();
        }

        public void SetColor(Color c) => drawColor = c;
        public void SetRotation(float rot) => rotation = rot;
        public void SetFadeValues(int time, Vector2 fadeoffset)
        {
            fadeTime = time;
            offset = fadeoffset;
        }



        public override void DrawAnimation(SpriteBatch spriteBatch, int timer)
        {
            if (timer < StartTime || timer > EndTime)
                return;

            Vector2 pos = GetDimensions().Center();
            Color c = drawColor;

            if (timer < StartTime + fadeTime)
            {
                float f = (float)(timer - StartTime) / fadeTime;

                pos += offset * f;
                c *= f;
            }
            if (timer > EndTime - fadeTime)
            {
                float f = 1 - (float)(timer - (EndTime - fadeTime)) / fadeTime;

                pos += offset * f;
                c *= f;
            }

            tex.Value.QuickCenteredDraw(spriteBatch, pos, c, rotation);
        }
    }
}
