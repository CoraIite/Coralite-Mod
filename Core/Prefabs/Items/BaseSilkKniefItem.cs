using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseSilkKniefItem:ModItem
    {
        public int combo;
        public override bool AltFunctionUse(Player player) => true;
    }
}
