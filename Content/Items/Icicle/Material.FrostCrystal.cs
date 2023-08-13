using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class FrostCrystal : BaseMaterial,IMagikeRemodelable
    {
        public FrostCrystal() : base(Item.CommonMaxStack, Item.sellPrice(0,0,10,0), ItemRarityID.Green, AssetDirectory.IcicleItems)
        {  }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<IcicleCrystal, FrostCrystal>(500);
            MagikeSystem.AddRemodelRecipe<FrostCrystal, IcicleCrystal>(100);
        }
    }
}
