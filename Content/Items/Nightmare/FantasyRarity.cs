using System;
using Terraria;

namespace Coralite.Content.Items.Nightmare
{
    public class FantasyRarity : ModRarity
    {
        public override Color RarityColor
        {
            get
            {
                float factor = Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly / 2)) * 2;
                if (factor < 1)
                    return Color.Lerp(new Color(158, 166, 176), new Color(218, 205, 212), factor);
                return Color.Lerp(new Color(218, 205, 212), new Color(254, 252, 194), factor - 1);
            }
        }
    }
}
