namespace Coralite.Content.ModPlayers
{
    public class MagikePlayer : ModPlayer
    {
        public bool equippedMagikeMonoclastic;
        public int SpecialEnchantCD;

        public override void ResetEffects()
        {
            equippedMagikeMonoclastic = false;
            if (SpecialEnchantCD > 0)
                SpecialEnchantCD--;
        }
    }
}
