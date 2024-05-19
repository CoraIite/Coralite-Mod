using Coralite.Core;
using Coralite.Core.Prefabs.Items;

namespace Coralite.Content.Items.FairyCatcher.Accessories
{
    public abstract class BaseFairyAccessory(int rare, int value) : BaseAccessory(rare, value)
    {
        public override string Texture => AssetDirectory.FairyCatcherAccessories + Name;
    }
}
