using Microsoft.Xna.Framework;
using System;

namespace Coralite.Core
{
    public class ColorGradient
    {
        public ColorGradient AddColor(Color c)
        {
            return this;
        }

        public Color GetColor(float factor)
        {
            factor = Math.Clamp(factor, 0f, 1f);
            return Color.White;
        }
    }
}
