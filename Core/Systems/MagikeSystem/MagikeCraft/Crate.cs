using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class Crate : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //木板条箱
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.SailfishBoots, 150);//航鱼靴
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.TsunamiInABottle, 150);//海啸瓶
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.Extractinator, 150);//提取机
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.Aglet, 150);//金属代扣
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.PortableStool, 50);//踢蹬
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.ClimbingClaws, 150);//攀爬抓
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.CordageGuide, 50);//植物纤维宝典
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.Radar, 150);//
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.ApprenticeBait, 50, 3);
            AddRemodelRecipe(ItemID.WoodenCrate, ItemID.JourneymanBait, 100, 3);

        }
    }
}
