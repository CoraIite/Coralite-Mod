using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class ExplodeCircle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "ExplosionCircle";

        /// <summary> 目标颜色，要透明 </summary>
        public Color? targetColor;
        /// <summary> 缩放时间 </summary>
        public int scaleTime = 8;
        /// <summary> 颜色消失的开始时间 </summary>
        public int fadeStartTime=8;
        public float recordScale;
        /// <summary> 目标大小 </summary>
        public float targetScale;
        /// <summary> 白色高亮的透明度 </summary>
        public float highlightAlpha = 1;

        public override void SetProperty()
        {
            recordScale = Scale;
        }

        public override void AI()
        {
            Opacity++;
            if (Opacity < scaleTime)
            {
                Scale = Helper.Lerp(recordScale, targetScale, Helper.SqrtEase(Opacity / scaleTime));
            }

            if (Opacity > fadeStartTime)
            {
                Color = Color.Lerp(Color, targetColor ?? Color.Transparent, 0.2f);
            }

            if (Color.A < 10)
            {
                active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            TexValue.QuickCenteredDraw(spriteBatch, Position - Main.screenPosition, Color, Rotation, Scale);

            Color c = new Color(255, 255, 255, 0) * highlightAlpha * (Color.A / 255f);

            TexValue.QuickCenteredDraw(spriteBatch, Position - Main.screenPosition, c, Rotation, Scale);

            return false;
        }
    }
}
