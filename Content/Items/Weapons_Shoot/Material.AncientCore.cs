using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Weapons_Shoot
{
    public class AncientCore : BaseMaterial
    {
        public AncientCore() : base("远古核心", "军火商压箱底的宝贝，来自远古文明的能量核心", 9999, Item.sellPrice(1, 0, 0, 0), ItemRarityID.Yellow, AssetDirectory.Weapons_Shoot)
        { }
    }
}