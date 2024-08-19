using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class IceyPowder : BaseMaterial, IMagikeCraftable
    {
        public IceyPowder() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe<IceyPowder>(15)
                .SetMainItem<MagicalPowder>()
                .AddIngredient(ItemID.IceBlock, 6)
                .Register();
        }
    }
}
