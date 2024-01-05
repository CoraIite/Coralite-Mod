using Coralite.Core;
using System.Collections.Generic;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class CoraliteWorldSettings : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers)
        {
            return 1;
        }
    }
}
