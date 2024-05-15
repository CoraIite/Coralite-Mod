using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    [AutoloadEquip(EquipType.Head)]
    public class ThunderveinDragonMask() : BaseBossMask(ItemRarityID.Yellow, Item.sellPrice(0, 0, 75))
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;
    }
}
