using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    [AutoloadEquip(EquipType.Head)]
    public class SlimeEmperorMask() : BaseBossMask(ItemRarityID.Orange, Item.sellPrice(0, 0, 75))
    {
        public override string Texture => AssetDirectory.GelItems + Name;
    }
}
