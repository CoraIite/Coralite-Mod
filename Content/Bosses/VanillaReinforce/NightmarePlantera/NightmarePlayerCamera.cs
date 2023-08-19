using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmarePlayerCamera:ModPlayer
    {
        public float factor;
        public bool useScreenMove;
        public bool useShake;
        public float shakeLevel;
        public int ShakeDelay;
        private int ShakeTimer;
        public Vector2 ShakeVec2;

        public override void ResetEffects()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out _))
            {
                useScreenMove = false;
            }
        }

        public override void ModifyScreenPosition()
        {
            if (useScreenMove && factor > 0 && NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Vector2 pos = Vector2.Lerp(Player.Center, np.Center, factor) - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

                if (useShake)
                {
                    ShakeTimer--;
                    ShakeVec2 *= 0.8f;
                    if (ShakeTimer<=0)
                    {
                        ShakeTimer = ShakeDelay;
                        ShakeVec2 = Helpers.Helper.NextVec2Dir() * shakeLevel;
                    }

                    pos += ShakeVec2;
                }
                
                Main.screenPosition = pos;
            }
        }
    }
}
