using Coralite.Core;
using Coralite.Core.Systems.DotSystem.BasicDots;

namespace Coralite.Content.Items.DessertSeries.CommonDots
{
    public class SucroseDot : BasicSweet
    {
        public override string Texture => AssetDirectory.DessertSeriesItem + Name;
        public override int BaseMaxDotLayer => 1;
    }
}
