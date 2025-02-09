using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class Crate : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //木板条箱
            //航鱼靴
            MagikeRecipe.CreateCraftRecipe(ItemID.WoodenCrate, ItemID.SailfishBoots, CalculateMagikeCost(MagicCrystal, 8, 60 * 3), 2)
                .RegisterNewCraft(ItemID.TsunamiInABottle, CalculateMagikeCost(MagicCrystal, 8, 180))//海啸瓶
                .RegisterNewCraft(ItemID.Extractinator, CalculateMagikeCost(MagicCrystal, 8, 180))//提取机
                .RegisterNewCraft(ItemID.Aglet, CalculateMagikeCost(MagicCrystal, 8, 180))//金属代扣
                .RegisterNewCraft(ItemID.PortableStool, CalculateMagikeCost(MagicCrystal, 8, 180))//踢蹬
                .RegisterNewCraft(ItemID.ClimbingClaws, CalculateMagikeCost(MagicCrystal, 8, 180))//攀爬爪
                .RegisterNewCraft(ItemID.CordageGuide, CalculateMagikeCost(MagicCrystal, 8, 180))//植物纤维宝典
                .RegisterNewCraft(ItemID.Radar, CalculateMagikeCost(MagicCrystal, 8, 180))//雷达
                .RegisterNewCraft<FlyingShieldVarnish>(CalculateMagikeCost(MagicCrystal, 8, 180))//飞盾光油
                .RegisterNewCraft<HeavyWedges>(CalculateMagikeCost(MagicCrystal, 8, 180))//重型楔石
                .RegisterNewCraft(ItemID.ApprenticeBait, CalculateMagikeCost(MagicCrystal, 8, 180), 3)
                .RegisterNewCraft(ItemID.JourneymanBait, CalculateMagikeCost(MagicCrystal, 8, 180), 3)
                .Register();

            //珍珠木板条箱
            MagikeRecipe.CreateCraftRecipe(ItemID.WoodenCrateHard, ItemID.SailfishBoots, CalculateMagikeCost(MagicCrystal, 8, 60 * 3), 2)
                .RegisterNewCraft(ItemID.TsunamiInABottle, CalculateMagikeCost(MagicCrystal, 8, 180))//海啸瓶
                .RegisterNewCraft(ItemID.Anchor, CalculateMagikeCost(CrystallineMagike, 8, 180))//锚
                .RegisterNewCraft(ItemID.Aglet, CalculateMagikeCost(MagicCrystal, 8, 180))//金属代扣
                .RegisterNewCraft(ItemID.PortableStool, CalculateMagikeCost(MagicCrystal, 8, 180))//踢蹬
                .RegisterNewCraft(ItemID.ClimbingClaws, CalculateMagikeCost(MagicCrystal, 8, 180))//攀爬爪
                .RegisterNewCraft(ItemID.CordageGuide, CalculateMagikeCost(MagicCrystal, 8, 180))//植物纤维宝典
                .RegisterNewCraft(ItemID.Radar, CalculateMagikeCost(MagicCrystal, 8, 180))//雷达
                .RegisterNewCraft<FlyingShieldVarnish>(CalculateMagikeCost(MagicCrystal, 8, 180))//飞盾光油
                .RegisterNewCraft<HeavyWedges>(CalculateMagikeCost(MagicCrystal, 8, 180))//重型楔石
                .RegisterNewCraft(ItemID.ApprenticeBait, CalculateMagikeCost(MagicCrystal, 8, 180), 3)
                .RegisterNewCraft(ItemID.JourneymanBait, CalculateMagikeCost(MagicCrystal, 8, 180), 3)
                .Register();


        }
    }
}
