using Terraria.ModLoader;

namespace Coralite.Content.ModPlayers
{
    public class MagikePlayer:ModPlayer
    {
        public bool equippedMagikeMonoclastic;

        public override void ResetEffects()
        {
            equippedMagikeMonoclastic = false;
        }
    }
}
