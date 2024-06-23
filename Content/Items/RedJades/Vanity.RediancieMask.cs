using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    [AutoloadEquip(EquipType.Head)]
    public class RediancieMask() : BaseBossMask(ItemRarityID.Blue, Item.sellPrice(0, 0, 75))
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;
    }
}
