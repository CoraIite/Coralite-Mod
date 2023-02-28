using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.IcicleItems
{
    public class IcicleCrystal : BaseMaterial
    {
        public IcicleCrystal() : base("冰晶", "冰之结晶", 9999, Item.sellPrice(0,0,50,0), ItemRarityID.Orange, AssetDirectory.IcicleItems) { }
    }
}
