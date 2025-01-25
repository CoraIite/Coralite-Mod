using Coralite.Content.Items.Materials;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class Misc : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //泥土
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.Acorn, 5, mainStack: 6);

            //石头
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.SandBlock, 8, mainStack: 1, resultStack: 2);

            //玻璃
            AddRemodelRecipe(ItemID.SandBlock, ItemID.Glass, 10, mainStack: 1, resultStack: 2);
            AddRemodelRecipe(ItemID.Glass, ItemID.Bottle, 10, mainStack: 1, resultStack: 4);

            //橡实
            MagikeRecipe.CreateRecipe(ItemID.Acorn, ItemID.Wood, CalculateMagikeCost(MagicCrystal, 2, 30), resultItemStack: 25)
                .RegisterNew(ItemID.AshWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.RichMahogany, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.Ebonwood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.Shadewood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.BorealWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.PalmWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNew(ItemID.Pearlwood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .AddCondition(Condition.Hardmode)
                .Register();

            //生命果
            AddRemodelRecipe(ItemType<RegrowthTentacle>(), ItemID.LifeFruit, CalculateMagikeCost(Soul, 6, 60 * 2), 5);
        }
    }
}
