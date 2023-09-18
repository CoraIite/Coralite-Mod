using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class SandliteDust : BaseMaterial, IMagikePolymerizable
    {
        public SandliteDust() : base( 9999, Item.sellPrice(0, 0, 2, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikePolymerizeRecipe()
        {
            PolymerizeRecipe.CreateRecipe<IceyDust>(15)
                .SetMainItem<MagicalDust>()
                .AddIngredient(ItemID.SandBlock, 6)
                .Register();
        }
    }
}
