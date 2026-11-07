using Coralite.Core;
using Coralite.Core.Systems.DotSystem.BasicDots;

namespace Coralite.Content.Items.DessertSeries.CommonDots
{
    public class RockSugarDot: BasicSweet
    {
        public override string Texture => AssetDirectory.DessertSeriesItem + Name;

        public override int BaseMaxDotLayer => 2;
        protected override float BasicOtherDotDamageBonus => 0.5f;
    }
}
