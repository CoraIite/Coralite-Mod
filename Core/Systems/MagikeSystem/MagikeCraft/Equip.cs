using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria.ID;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class Equip : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //高顶礼帽
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.TopHat, CalculateMagikeCost<CrystalLevel>( 6, 60 * 2), 8)
                .AddIngredient(ItemID.BlackThread, 4)
                .Register();

            //死人毛衣
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.DeadMansSweater, CalculateMagikeCost<CrystalLevel>( 6, 60 * 2), 8)
                .AddIngredient(ItemID.PinkThread, 4)
                .Register();

            //夜视头盔
            MagikeRecipe.CreateCraftRecipe(ItemID.GraniteBlock, ItemID.NightVisionHelmet, CalculateMagikeCost<CrystalLevel>( 3, 60), 14)
                .AddIngredient(ItemID.UltrabrightTorch)
                .Register();

            //雨衣套装
            MagikeRecipe.CreateCraftRecipe(ItemID.Gel, ItemID.RainHat, CalculateMagikeCost<CrystalLevel>( 3, 60), 10)
                .AddIngredient(ItemID.YellowDye)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Gel, ItemID.RainCoat, CalculateMagikeCost<CrystalLevel>( 3, 60), 20)
                .AddIngredient(ItemID.YellowDye)
                .Register();

            //防雪套装
            MagikeRecipe.CreateCraftRecipe(ItemID.FlinxFur, ItemID.EskimoHood, CalculateMagikeCost<CrystalLevel>( 3, 60))
                .AddIngredient(ItemID.BlueString)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.FlinxFur, ItemID.EskimoCoat, CalculateMagikeCost<CrystalLevel>( 3, 60), 2)
                .AddIngredient(ItemID.BlueString)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.FlinxFur, ItemID.EskimoPants, CalculateMagikeCost<CrystalLevel>( 3, 60))
                .AddIngredient(ItemID.BlueString)
                .Register();

            //渔夫套装
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.AnglerHat, CalculateMagikeCost<CrystalLevel>( 6, 60), 5)
                .AddIngredient(ItemID.ApprenticeBait)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.AnglerVest, CalculateMagikeCost<CrystalLevel>( 6, 60), 10)
                .AddIngredient(ItemID.ApprenticeBait)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Silk, ItemID.AnglerPants, CalculateMagikeCost<CrystalLevel>( 6, 60), 5)
                .AddIngredient(ItemID.ApprenticeBait)
                .Register();
        }
    }
}
