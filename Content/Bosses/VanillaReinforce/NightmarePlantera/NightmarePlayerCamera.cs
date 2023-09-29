using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmarePlayerCamera : ModPlayer
    {
        public float factor;
        public bool useScreenMove;
        public bool useShake;
        public float shakeLevel;
        public int shakeDelay;
        private int shakeTimer;
        public Vector2 ShakeVec2;

        public void Reset()
        {
            useScreenMove = false;
            useShake = false;
            factor = 0;
            shakeLevel = 0;
            shakeDelay = 0;
            shakeTimer = 0;
            ShakeVec2 = Vector2.Zero;
        }

        public override void ResetEffects()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out _))
                Reset();
        }

        public override void ModifyScreenPosition()
        {
            if (useScreenMove && factor > 0 && NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Vector2 pos = Vector2.Lerp(Player.Center, np.Center, factor) - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

                if (useShake)
                {
                    shakeTimer--;
                    ShakeVec2 *= 0.8f;
                    if (shakeTimer <= 0)
                    {
                        shakeTimer = shakeDelay;
                        ShakeVec2 = Helpers.Helper.NextVec2Dir() * shakeLevel;
                    }

                    pos += ShakeVec2;
                }

                Main.screenPosition = pos;
            }
        }
    }
}
