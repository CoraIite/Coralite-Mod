using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Nightmare
{
    public class NightmareRarity : ModRarity
    {
        public override Color RarityColor
        {
            get
            {
                float factor = Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly / 2)) * 2;
                if (factor < 1)
                    return Color.Lerp(new Color(158, 166, 176), new Color(255, 209, 252), factor);
                return Color.Lerp(new Color(255, 209, 252), NightmarePlantera.nightmareRed, (factor - 1));
            }
        }

    }
}
