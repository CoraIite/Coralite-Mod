using Coralite.Core;
using System.Collections.Generic;
using Terraria.UI;

namespace Coralite.Content.UI.NewKnowledgeUnlock
{
    public class NewKnowledgeState : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));


    }
}
