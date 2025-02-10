using Coralite.Content.Items.Gels;
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

            AddRemodelRecipe(ItemID.Gel, ItemID.Lens, 10, mainStack: 5);
            MagikeRecipe.CreateCraftRecipe(ItemID.Lens, ItemID.BlackLens, CalculateMagikeCost(Corruption, 6, 60 * 3), 4)
                .AddIngredient(ItemID.BlackInk)
                .Register();

            //橡实
            MagikeRecipe.CreateCraftRecipe(ItemID.Acorn, ItemID.Wood, CalculateMagikeCost(MagicCrystal, 2, 30), resultItemStack: 25)
                .RegisterNewCraft(ItemID.AshWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.RichMahogany, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.Ebonwood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.Shadewood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.BorealWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.PalmWood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .RegisterNewCraft(ItemID.Pearlwood, CalculateMagikeCost(MagicCrystal, 2, 30), 25)
                .AddCondition(Condition.Hardmode)
                .Register();

            //生命果
            AddRemodelRecipe(ItemType<RegrowthTentacle>(), ItemID.LifeFruit, CalculateMagikeCost(Soul, 6, 60 * 2));

            //丛林玫瑰与大自然的恩惠
            MagikeRecipe.CreateCraftRecipe(ItemID.JungleGrassSeeds, ItemID.JungleRose, CalculateMagikeCost(MagicCrystal, 6, 60), 3)
                .RegisterNewCraft(ItemID.NaturesGift, CalculateMagikeCost(MagicCrystal, 6, 60 * 3))
                .Register();

            #region 各种水果

            int fruitCost = CalculateMagikeCost(MagicCrystal, 6);
            MagikeRecipe.CreateCraftRecipe(ItemID.Wood, ItemID.Apple, fruitCost, 24)
                .RegisterNewCraft(ItemID.Apricot, fruitCost)
                .RegisterNewCraft(ItemID.Grapefruit, fruitCost)
                .RegisterNewCraft(ItemID.Lemon, fruitCost)
                .RegisterNewCraft(ItemID.Peach, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.BorealWood, ItemID.Cherry, fruitCost, 24)
                .RegisterNewCraft(ItemID.Plum, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.Ebonwood, ItemID.BlackCurrant, fruitCost, 24)
                .RegisterNewCraft(ItemID.Elderberry, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.Shadewood, ItemID.BloodOrange, fruitCost, 24)
                .RegisterNewCraft(ItemID.Rambutan, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.RichMahogany, ItemID.Mango, fruitCost, 24)
                .RegisterNewCraft(ItemID.Pineapple, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.PalmWood, ItemID.Banana, fruitCost, 24)
                .RegisterNewCraft(ItemID.Coconut, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.Pearlwood, ItemID.Dragonfruit, CalculateMagikeCost(Hellstone, 6), 24)
                .RegisterNewCraft(ItemID.Starfruit, CalculateMagikeCost(Hellstone, 6))
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.AshWood, ItemID.Pomegranate, fruitCost, 24)
                .RegisterNewCraft(ItemID.SpicyPepper, fruitCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe<GelFiber, Princesstrawberry>(CalculateMagikeCost(Glistent, 6), 24)
                .RegisterNewCraft<Woodbine>(CalculateMagikeCost(Glistent, 6))
                .Register();

            #endregion
        }
    }
}
