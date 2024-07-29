using Coralite.Core;
using Coralite.Core.Prefabs.Misc;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ThunderveinDragonBossBar : BaseBossHealthBar
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + Name;
    }
}
