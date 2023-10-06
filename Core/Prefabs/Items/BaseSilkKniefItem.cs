using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseSilkKnifeItem:ModItem
    {
        public int combo;
        public override bool AltFunctionUse(Player player) => true;
    }
}
