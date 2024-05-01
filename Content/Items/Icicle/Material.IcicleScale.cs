using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    internal class IcicleScale:BaseMaterial
    {
        public IcicleScale() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 40), ItemRarityID.Green, AssetDirectory.IcicleItems)
        {
        }
    }
}
