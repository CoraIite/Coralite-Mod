using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class LeafStone : BaseMaterial, IMagikeRemodelable
    {
        public LeafStone() : base(9999, Item.sellPrice(0, 0, 0, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<LeafStone>(0f, ItemID.Wood, 20, selfStack: 15);
        }
    }
}
