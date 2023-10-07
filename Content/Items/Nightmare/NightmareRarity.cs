using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Nightmare
{
    public class NightmareRarity : ModRarity
    {
        public override Color RarityColor
            => Color.Lerp(new Color(255, 209, 252), new Color(158, 166, 176), Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly)));

    }
}
