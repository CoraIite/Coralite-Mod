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
            MagikeRecipe.CreateRecipe(ItemID.WoodenCrate, ItemID.SailfishBoots, CalculateMagikeCost(MagicCrystal, 8, 60 * 3), 2)
                .RegisterNew(ItemID.TsunamiInABottle, CalculateMagikeCost(MagicCrystal, 8, 180))//海啸瓶
                .RegisterNew(ItemID.Extractinator, CalculateMagikeCost(MagicCrystal, 8, 180))//提取机
                .RegisterNew(ItemID.Aglet, CalculateMagikeCost(MagicCrystal, 8, 180))//金属代扣
                .RegisterNew(ItemID.PortableStool, CalculateMagikeCost(MagicCrystal, 8, 180))//踢蹬
                .RegisterNew(ItemID.ClimbingClaws, CalculateMagikeCost(MagicCrystal, 8, 180))//攀爬爪
                .RegisterNew(ItemID.CordageGuide, CalculateMagikeCost(MagicCrystal, 8, 180))//植物纤维宝典
                .RegisterNew(ItemID.Radar, CalculateMagikeCost(MagicCrystal, 8, 180))//雷达
                .RegisterNew<FlyingShieldVarnish>(CalculateMagikeCost(MagicCrystal, 8, 180))//飞盾光油
                .RegisterNew<HeavyWedges>(CalculateMagikeCost(MagicCrystal, 8, 180))//重型楔石
                .RegisterNew(ItemID.ApprenticeBait, CalculateMagikeCost(MagicCrystal, 8, 180), 3)
                .RegisterNew(ItemID.JourneymanBait, CalculateMagikeCost(MagicCrystal, 8, 180), 3)
                .Register();

            //珍珠木板条箱
            MagikeRecipe.CreateRecipe(ItemID.WoodenCrateHard, ItemID.SailfishBoots, CalculateMagikeCost(MagicCrystal, 8, 60 * 3), 2)
                .RegisterNew(ItemID.TsunamiInABottle, CalculateMagikeCost(MagicCrystal, 8, 180))//海啸瓶
                .RegisterNew(ItemID.Anchor, CalculateMagikeCost(CrystallineMagike, 8, 180))//锚
                .RegisterNew(ItemID.Aglet, CalculateMagikeCost(MagicCrystal, 8, 180))//金属代扣
                .RegisterNew(ItemID.PortableStool, CalculateMagikeCost(MagicCrystal, 8, 180))//踢蹬
                .RegisterNew(ItemID.ClimbingClaws, CalculateMagikeCost(MagicCrystal, 8, 180))//攀爬爪
                .RegisterNew(ItemID.CordageGuide, CalculateMagikeCost(MagicCrystal, 8, 180))//植物纤维宝典
                .RegisterNew(ItemID.Radar, CalculateMagikeCost(MagicCrystal, 8, 180))//雷达
                .RegisterNew<FlyingShieldVarnish>(CalculateMagikeCost(MagicCrystal, 8, 180))//飞盾光油
                .RegisterNew<HeavyWedges>(CalculateMagikeCost(MagicCrystal, 8, 180))//重型楔石
                .RegisterNew(ItemID.ApprenticeBait, CalculateMagikeCost(MagicCrystal, 8, 180), 3)
                .RegisterNew(ItemID.JourneymanBait, CalculateMagikeCost(MagicCrystal, 8, 180), 3)
                .Register();


        }
    }
}
