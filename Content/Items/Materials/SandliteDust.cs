using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class SandliteDust : BaseMaterial
    {
        public SandliteDust() : base("沙魔粉", "蕴含沙漠的魔力", 9999, Item.sellPrice(0, 0, 2, 50), ItemRarityID.Green, AssetDirectory.Materials) { }
    }
}
