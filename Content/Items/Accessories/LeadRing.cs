using Coralite.Core.Systems.MagikeSystem;
using Terraria.ID;
using Terraria;
using Coralite.Core.Prefabs.Items;
using Coralite.Core;

namespace Coralite.Content.Items.Accessories
{
    internal class LeadRing : BaseAccessory, IMagikeRemodelable
    {
        public LeadRing() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 0, 20)) { }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<LeadRing>(0f, ItemID.LeadBar, 150, selfStack: 100);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance += 0.08f;
            player.moveSpeed -= 0.1f;
        }
    }
}
