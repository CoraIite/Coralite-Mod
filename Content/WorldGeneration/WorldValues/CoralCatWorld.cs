using Coralite.Core.Systems.WorldValueSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.WorldGeneration.WorldValues
{
    public class CoralCatWorld : WorldFlag
    {
        public override bool NeedResetPostWoldGen => false;

        public override void OnEnterWorld(Player player)
        {
            if (Value)
                player.QuickSpawnItem(player.GetSource_FromThis(), ItemID.Meowmere);
        }
    }
}
