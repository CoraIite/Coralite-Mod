using Terraria;

namespace Coralite.Content.ModPlayers
{
    public class MagikePlayer : ModPlayer
    {
        public int SpecialEnchantCD;

        public override void ResetEffects()
        {
            if (SpecialEnchantCD > 0)
                SpecialEnchantCD--;
        }
    }
}
