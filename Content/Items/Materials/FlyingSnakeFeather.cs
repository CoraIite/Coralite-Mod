using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class FlyingSnakeFeather : BaseMaterial
    {
        public FlyingSnakeFeather() : base(9999, Item.sellPrice(0, 0, 5, 0), ItemRarityID.Lime, AssetDirectory.Materials) { }

    }
}
