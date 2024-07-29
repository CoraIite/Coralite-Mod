using Coralite.Content.Items.Glistent;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class BotanicusPowder : BaseMaterial, IMagikePolymerizable
    {
        public BotanicusPowder() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikePolymerizeRecipe()
        {
            PolymerizeRecipe.CreateRecipe<EmpyrosPowder>(15)
                .SetMainItem<MagicalPowder>()
                .AddIngredient<LeafStone>()
                .Register();
        }
    }
}
