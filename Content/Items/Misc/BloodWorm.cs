using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Misc
{
    public class BloodWorm() : BaseMaterial(Item.CommonMaxStack
        , Item.sellPrice(0, 0, 10), ItemRarityID.LightRed, AssetDirectory.MiscItems), IMagikeCraftable
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.bait = 45;
        }

        public void AddMagikeCraftRecipe()
        {
            int magikeCost = MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 4, 30);
            int resultItemType = ModContent.ItemType<BloodWorm>();
            MagikeRecipe.CreateCraftRecipe(ItemID.EnchantedNightcrawler, resultItemType, magikeCost)
                .AddIngredient(ItemID.BloodWater)
                .RegisterNewCraft(resultItemType, magikeCost)
                .AddIngredient(ItemID.UnholyWater)
                .Register();
        }
    }
}
