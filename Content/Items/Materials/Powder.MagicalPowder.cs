using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class MagicalPowder : BaseMaterial, IMagikeCraftable
    {
        public MagicalPowder() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateRecipe(ModContent.ItemType<MagicalPowder>(), ItemID.EnchantedSword, MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 6, 120)
                , MainItenStack: 40)
                .RegisterNew(ItemID.FallenStar, MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 6, 20)).SetMainStack(5)
                .RegisterNew(ItemID.ManaCrystal, MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 6, 40)).SetMainStack(25)
                .Register();
        }
    }
}
