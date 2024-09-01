using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class BotanicusPowder : BaseMaterial, IMagikeCraftable
    {
        public BotanicusPowder() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            //MagikeCraftRecipe.CreateRecipe<EmpyrosPowder>(15)
            //    .SetMainItem<MagicalPowder>()
            //    .AddIngredient<LeafStone>()
            //    .Register();
        }
    }
}
