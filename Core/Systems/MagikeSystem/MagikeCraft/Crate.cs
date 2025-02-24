using Coralite.Content.Items.FlyingShields.Accessories;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class Crate : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //木板条箱
            int woddenChestCost = CalculateMagikeCost(MagicCrystal, 4, 60 * 3);
            //航鱼靴
            MagikeRecipe.CreateCraftRecipe(ItemID.WoodenCrate, ItemID.SailfishBoots, woddenChestCost, 2)
                .RegisterNewCraft(ItemID.TsunamiInABottle, woddenChestCost)//海啸瓶
                .RegisterNewCraft(ItemID.Extractinator, woddenChestCost)//提取机
                .RegisterNewCraft(ItemID.Aglet, woddenChestCost)//金属代扣
                .RegisterNewCraft(ItemID.PortableStool, woddenChestCost)//踢蹬
                .RegisterNewCraft(ItemID.ClimbingClaws, woddenChestCost)//攀爬爪
                .RegisterNewCraft(ItemID.CordageGuide, woddenChestCost)//植物纤维宝典
                .RegisterNewCraft(ItemID.Radar, woddenChestCost)//雷达
                .RegisterNewCraft<FlyingShieldVarnish>(woddenChestCost)//飞盾光油
                .RegisterNewCraft<HeavyWedges>(woddenChestCost)//重型楔石
                .RegisterNewCraft(ItemID.ApprenticeBait, woddenChestCost, 3)
                .RegisterNewCraft(ItemID.JourneymanBait, woddenChestCost, 3)
                .Register();

            //珍珠木板条箱
            MagikeRecipe.CreateCraftRecipe(ItemID.WoodenCrateHard, ItemID.SailfishBoots, woddenChestCost, 2)
                .RegisterNewCraft(ItemID.TsunamiInABottle, woddenChestCost)//海啸瓶
                .RegisterNewCraft(ItemID.Anchor, CalculateMagikeCost(CrystallineMagike, 8, 180))//锚
                .RegisterNewCraft(ItemID.Aglet, woddenChestCost)//金属代扣
                .RegisterNewCraft(ItemID.PortableStool, woddenChestCost)//踢蹬
                .RegisterNewCraft(ItemID.ClimbingClaws, woddenChestCost)//攀爬爪
                .RegisterNewCraft(ItemID.CordageGuide, woddenChestCost)//植物纤维宝典
                .RegisterNewCraft(ItemID.Radar, woddenChestCost)//雷达
                .RegisterNewCraft<FlyingShieldVarnish>(woddenChestCost)//飞盾光油
                .RegisterNewCraft<HeavyWedges>(woddenChestCost)//重型楔石
                .RegisterNewCraft(ItemID.ApprenticeBait, woddenChestCost, 3)
                .RegisterNewCraft(ItemID.JourneymanBait, woddenChestCost, 3)
                .Register();


        }
    }
}
