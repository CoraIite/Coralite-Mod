using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class BloodyOrb : BaseMaterial
    {
        public BloodyOrb() : base(9999, Item.sellPrice(0, 0, 50, 0), ItemRarityID.LightRed, AssetDirectory.Materials) { }
    }
}
