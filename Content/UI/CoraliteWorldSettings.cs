using Coralite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
