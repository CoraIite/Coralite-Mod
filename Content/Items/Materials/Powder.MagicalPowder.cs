using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class MagicalPowder : BaseMaterial, IMagikeRemodelable
    {
        public MagicalPowder() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<MagicalPowder>(150, ItemID.EnchantedSword, selfStack: 40);
            MagikeSystem.AddRemodelRecipe<MagicalPowder>(0f, ItemID.FallenStar, 25, 5);
            MagikeSystem.AddRemodelRecipe<MagicalPowder>(0f, ItemID.ManaCrystal, 50, 25);
        }
    }
}
