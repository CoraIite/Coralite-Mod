using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class FrostCrystal : BaseMaterial, IMagikeCraftable
    {
        public FrostCrystal() : base(Item.CommonMaxStack, Item.sellPrice(0, 1), ItemRarityID.LightPurple, AssetDirectory.IcicleItems)
        { }

        public void AddMagikeCraftRecipe()
        {
            MagikeSystem.AddRemodelRecipe<IcicleCrystal, FrostCrystal>(MagikeHelper.CalculateMagikeCost(MALevel.Frost, 12, 60 * 10));
            //MagikeSystem.AddRemodelRecipe<FrostCrystal, IcicleCrystal>(100);
        }
    }
}
