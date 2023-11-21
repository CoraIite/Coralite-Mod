using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class FragmentsOfLight : BaseMaterial
    {
        public FragmentsOfLight() : base(9999, Item.sellPrice(0, 2, 0, 0), ItemRarityID.Yellow, AssetDirectory.Materials) { }
    }
}
