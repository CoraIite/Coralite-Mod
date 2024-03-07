using Coralite.Core;
using Microsoft.Xna.Framework;
using System;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ChainBall:BaseThunderProj
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "LightingBall";

        public ref float OtherOneIndex => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];

        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinYellowAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinOrangeAlpha, MathF.Sin(factor * MathHelper.Pi)) * ThunderAlpha;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                    break;
            }
        }
    }
}
