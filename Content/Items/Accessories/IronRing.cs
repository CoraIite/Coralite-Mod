using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories
{
    public class IronRing : BaseAccessory, IMagikeRemodelable
    {
        public IronRing() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 0, 20)) { }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<IronRing>(0f, ItemID.IronBar, 150, selfStack: 100);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance += 0.04f;
            player.moveSpeed -= 0.1f;
        }
    }
}
