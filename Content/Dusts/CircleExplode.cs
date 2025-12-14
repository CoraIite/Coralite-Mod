using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Dusts
{
    public class CircleExplode : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            if (dust.fadeIn < 10)
            {
                dust.scale += 0.05f;
            }
            else
            {
                dust.color *= 0.9f;
                dust.scale += 0.02f;
            }

            if (dust.color.A < 10)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.position - Main.screenPosition
                , dust.color, 0, dust.scale);

            return false;
        }
    }

    public class CircleExplodeParticle : Particle
    {
        public override string Texture => AssetDirectory.Dusts + "CircleExplode";

        /// <summary>
        /// 变大的时间
        /// </summary>
        public int addTime=10;
        /// <summary>
        /// 变大的时候每帧尺寸增加多少
        /// </summary>
        public float  scaleAdd=0.05f;
        public float  scaleAddSlow=0.02f;
        public float  colorFade=0.9f;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            Opacity++;
            if (Opacity < addTime)
            {
                Scale += scaleAdd;
            }
            else
            {
                Color *= colorFade;
                Scale += scaleAddSlow;
            }

            if (Color.A < 10)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            TexValue.QuickCenteredDraw(spriteBatch, Position - Main.screenPosition
                , Color, 0, Scale);

            return false;
        }
    }
}
