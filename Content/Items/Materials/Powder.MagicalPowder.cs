using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class MagicalPowder : BaseMaterial, IMagikeCraftable
    {
        public MagicalPowder() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe(ModContent.ItemType<MagicalPowder>(), ItemID.EnchantedSword, 150, MainItenStack: 40)
                .RegisterNew(ItemID.FallenStar, 25).SetMainStack(5)
                .RegisterNew(ItemID.ManaCrystal, 50).SetMainStack(25)
                .Register();
        }
    }
}
