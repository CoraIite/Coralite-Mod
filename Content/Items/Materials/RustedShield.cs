using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class RustedShield : BaseMaterial
    {
        public RustedShield() : base(9999, Item.sellPrice(0, 7, 50), ItemRarityID.Yellow, AssetDirectory.Materials) { }
    }
}
