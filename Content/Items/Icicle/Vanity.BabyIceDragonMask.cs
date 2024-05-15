using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    [AutoloadEquip(EquipType.Head)]
    public class BabyIceDragonMask() : BaseBossMask(ItemRarityID.Orange, Item.sellPrice(0, 0, 75))
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;
    }
}
