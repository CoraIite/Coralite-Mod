using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class WatermelonSlices : BaseMaterial
    {
        public WatermelonSlices() : base("西瓜片", "", 9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Orange, AssetDirectory.Materials) { }
    }
}
