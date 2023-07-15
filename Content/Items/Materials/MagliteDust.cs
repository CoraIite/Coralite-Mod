using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class MagliteDust : BaseMaterial,IMagikeRemodelable
    {
        public MagliteDust() : base( 9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<MagliteDust>(150, ItemID.EnchantedSword, selfRequiredNumber: 40);
        }
    }
}
