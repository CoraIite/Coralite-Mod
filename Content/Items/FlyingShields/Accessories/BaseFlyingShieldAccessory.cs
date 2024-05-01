using Coralite.Core;
using Coralite.Core.Prefabs.Items;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public abstract class BaseFlyingShieldAccessory : BaseAccessory
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        protected BaseFlyingShieldAccessory(int rare, int value) : base(rare, value)
        {
        }
    }
}
