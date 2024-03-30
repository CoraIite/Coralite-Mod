using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class RegrowthTentacle : BaseMaterial
    {
        public RegrowthTentacle() : base(9999
            , Item.sellPrice(0, 0, 50), ItemRarityID.Lime, AssetDirectory.Materials) { }
    }
}
