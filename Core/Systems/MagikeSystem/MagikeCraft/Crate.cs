using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class Crate : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //木板条箱
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.SailfishBoots);//航鱼靴
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.TsunamiInABottle);//海啸瓶
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.Extractinator);//提取机
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.Aglet);//金属代扣
            AddRemodelRecipe(ItemID.WoodenCrate, 50, ItemID.PortableStool);//踢蹬
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.ClimbingClaws);//攀爬抓
            AddRemodelRecipe(ItemID.WoodenCrate, 50, ItemID.CordageGuide);//植物纤维宝典
            AddRemodelRecipe(ItemID.WoodenCrate, 150, ItemID.Radar);//
            AddRemodelRecipe(ItemID.WoodenCrate, 50, ItemID.ApprenticeBait, 3);
            AddRemodelRecipe(ItemID.WoodenCrate, 100, ItemID.JourneymanBait, 3);

        }
    }
}
