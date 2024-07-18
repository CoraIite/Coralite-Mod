using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class Night_luminescentPearl : BaseMaterial
    {
        public Night_luminescentPearl() : base(Item.CommonMaxStack, Item.sellPrice(0, 1), ItemRarityID.Red, AssetDirectory.Materials)
        {
        }
    }
}
