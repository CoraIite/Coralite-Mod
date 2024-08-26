using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories
{
    internal class LeadRing : BaseAccessory, IMagikeCraftable
    {
        public LeadRing() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 0, 20)) { }

        public void AddMagikeCraftRecipe()
        {
            //MagikeSystem.AddRemodelRecipe<LeadRing>(0f, ItemID.LeadBar, 150, selfStack: 100);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance += 0.04f;
            player.moveSpeed -= 0.1f;
        }
    }
}
