using Coralite.Core;
using Coralite.Core.Prefabs.Misc;

namespace Coralite.Content.NPCs.Crystalline
{
    public class CrystallineSentinelBossBar : BaseBossHealthBar
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public override Point BarSize => new(410, 20);

        public override Vector2 IconOffset => new Vector2(24,8);
    }
}
