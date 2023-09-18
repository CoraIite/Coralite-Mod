using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class IceyDust : BaseMaterial,IMagikePolymerizable
    {
        public IceyDust() : base( 9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikePolymerizeRecipe()
        {
            PolymerizeRecipe.CreateRecipe<IceyDust>(15)
                .SetMainItem<MagicalDust>()
                .AddIngredient(ItemID.IceBlock, 6)
                .Register();
        }
    }
}
