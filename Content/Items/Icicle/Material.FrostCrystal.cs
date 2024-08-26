using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class FrostCrystal : BaseMaterial, IMagikeCraftable
    {
        public FrostCrystal() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 10, 0), ItemRarityID.Green, AssetDirectory.IcicleItems)
        { }

        public void AddMagikeCraftRecipe()
        {
            MagikeSystem.AddRemodelRecipe<IcicleCrystal, FrostCrystal>(500);
            MagikeSystem.AddRemodelRecipe<FrostCrystal, IcicleCrystal>(100);
        }
    }
}
