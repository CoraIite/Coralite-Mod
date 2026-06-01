using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class Hecatolite() : BaseGemItem(Item.sellPrice(0, 1), ItemRarityID.Pink, AssetDirectory.LandOfTheLustrousSeriesItems)
    {
        public static Color highlightC = new(196, 242, 255);
        public static Color brightC = new(112, 128, 186);
        public static Color darkC = new(39, 37, 82);
    }
}
