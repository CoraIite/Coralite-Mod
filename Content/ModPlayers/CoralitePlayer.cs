using Terraria.ModLoader;

namespace Coralite.Content.ModPlayers
{
    public class CoralitePlayer : ModPlayer
    {
        public byte cosmosFractureRightClickTimer = 0;

        public override void PostUpdate()
        {
            if (cosmosFractureRightClickTimer != 0)
                cosmosFractureRightClickTimer--;
        }
    }
}
