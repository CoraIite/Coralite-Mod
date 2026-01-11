using Coralite.Content.Items.BossSummons;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.RedJades;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class BossSummons : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //赤玉晶核
            AddRemodelRecipe<RedJade, RedJadeCore>(CalculateMagikeCost<RedJadeLevel>(3), mainStack: 35);

            //冰龙心
            AddRemodelRecipe<IcicleCrystal, IcicleHeart>(CalculateMagikeCost<IcicleLevel>(3), mainStack: 8);

            //血泪
            //肉前高难度配方
            MagikeRecipe.CreateCraftRecipe(ItemID.DeathweedSeeds, ItemID.BloodMoonStarter, CalculateMagikeCost<CrystalLevel>(6), 5)
                .AddIngredient(ItemID.ViciousPowder, 30)
                .AddIngredient(ItemID.Vertebrae, 15)
                .AddIngredient(ItemID.CrimstoneBlock, 20)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.DeathweedSeeds, ItemID.BloodMoonStarter, CalculateMagikeCost<CrystalLevel>(6), 5)
                .AddIngredient(ItemID.VilePowder, 30)
                .AddIngredient(ItemID.RottenChunk, 15)
                .AddIngredient(ItemID.EbonstoneBlock, 20)
                .Register();

            //肉后简易配方
            MagikeRecipe.CreateCraftRecipe(ItemID.SoulofNight, ItemID.BloodMoonStarter, CalculateMagikeCost<BrilliantLevel>(6))
                .AddIngredient(ItemID.BloodWater)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.SoulofNight, ItemID.BloodMoonStarter, CalculateMagikeCost<BrilliantLevel>(6))
                .AddIngredient(ItemID.UnholyWater)
                .Register();
        }
    }
}
