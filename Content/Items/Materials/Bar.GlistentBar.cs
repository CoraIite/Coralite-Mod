using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class GlistentBar: BaseMaterial, IMagikePolymerizable
    {
        public GlistentBar() : base(9999, Item.sellPrice(0, 0, 5, 50), ItemRarityID.Orange, AssetDirectory.Materials) { }

        public void AddMagikePolymerizeRecipe()
        {
            PolymerizeRecipe.CreateRecipe<GlistentBar>(50)
                .SetMainItem<LeafStone>()
                .AddIngredient(ItemID.CrimtaneBar, 2)
                .AddIngredient(ItemID.DemoniteBar, 2)
                .AddIngredient(ItemID.Diamond)
                .Register();
        }
    }
}
