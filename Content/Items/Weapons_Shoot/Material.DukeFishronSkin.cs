using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Weapons_Shoot
{
    public class DukeFishronSkin:BaseMaterial
    {
        public DukeFishronSkin() : base("远古鲨鱼皮", "巨型鲨鱼的粗糙韧皮，摸起来感觉不错", 9999, Item.sellPrice(0, 10, 0, 0), ItemRarityID.Yellow, AssetDirectory.Weapons_Shoot){}
    }
}