using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class DukeFishronSkin : BaseMaterial
    {
        public DukeFishronSkin() : base(9999, Item.sellPrice(0, 10, 0, 0), ItemRarityID.Yellow, AssetDirectory.Materials) { }
    }
}