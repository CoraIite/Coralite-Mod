using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class EmpyrosPowder : BaseMaterial, IMagikeCraftable
    {
        public EmpyrosPowder() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            //MagikeCraftRecipe.CreateRecipe<EmpyrosPowder>(15)
            //    .SetMainItem<MagicalPowder>()
            //    .AddIngredient(ItemID.LivingFireBlock, 3)
            //    .Register();
        }
    }
}
