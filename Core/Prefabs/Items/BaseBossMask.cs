using Terraria;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseBossMask(int rare, int value) : ModItem
    {
        public override void SetDefaults()
        {
            Item.vanity = true;
            Item.maxStack = 1;
            Item.rare = rare;
            Item.value = value;
        }
    }
}
