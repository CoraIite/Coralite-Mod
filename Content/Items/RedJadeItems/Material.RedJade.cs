using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJadeItems
{
    public class RedJade:BaseMaterial
    {
        public RedJade() : base("赤玉", " ", 9999, Item.sellPrice(0, 0, 2, 50), ItemRarityID.Blue, AssetDirectory.RedJadeItems) { }
    }
}
