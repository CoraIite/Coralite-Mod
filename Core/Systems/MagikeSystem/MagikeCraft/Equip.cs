using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class Equip : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //高顶礼帽
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.TopHat, CalculateMagikeCost(MagicCrystal, 6, 60 * 2), 8)
                .AddIngredient(ItemID.BlackThread, 4)
                .Register();

            //死人毛衣
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.DeadMansSweater, CalculateMagikeCost(MagicCrystal, 6, 60 * 2), 8)
                .AddIngredient(ItemID.PinkThread, 4)
                .Register();
        }
    }
}
