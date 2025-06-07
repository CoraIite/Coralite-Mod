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

            //夜视头盔
            MagikeRecipe.CreateCraftRecipe(ItemID.GraniteBlock, ItemID.NightVisionHelmet, CalculateMagikeCost(MagicCrystal, 3, 60), 14)
                .AddIngredient(ItemID.UltrabrightTorch)
                .Register();

            //雨衣套装
            MagikeRecipe.CreateCraftRecipe(ItemID.Gel, ItemID.RainHat, CalculateMagikeCost(MagicCrystal, 3, 60), 10)
                .AddIngredient(ItemID.YellowDye)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Gel, ItemID.RainCoat, CalculateMagikeCost(MagicCrystal, 3, 60), 20)
                .AddIngredient(ItemID.YellowDye)
                .Register();

            //防雪套装
            MagikeRecipe.CreateCraftRecipe(ItemID.FlinxFur, ItemID.EskimoHood, CalculateMagikeCost(MagicCrystal, 3, 60))
                .AddIngredient(ItemID.BlueString)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.FlinxFur, ItemID.EskimoCoat, CalculateMagikeCost(MagicCrystal, 3, 60), 2)
                .AddIngredient(ItemID.BlueString)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.FlinxFur, ItemID.EskimoPants, CalculateMagikeCost(MagicCrystal, 3, 60))
                .AddIngredient(ItemID.BlueString)
                .Register();

            //渔夫套装
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.AnglerHat, CalculateMagikeCost(MagicCrystal, 6, 60), 5)
                .AddIngredient(ItemID.ApprenticeBait)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.AnglerVest, CalculateMagikeCost(MagicCrystal, 6, 60), 10)
                .AddIngredient(ItemID.ApprenticeBait)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.AnglerPants, CalculateMagikeCost(MagicCrystal, 6, 60), 5)
                .AddIngredient(ItemID.ApprenticeBait)
                .Register();
        }
    }
}
