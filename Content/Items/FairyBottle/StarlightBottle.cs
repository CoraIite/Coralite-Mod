using Coralite.Content.Items.Materials;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyBottle
{
    public class StarlightBottle : BaseFairyBottle
    {
        public override int FightCapacity => 3;
        public override int ContainCapacity => 10;

        public override void SetDefaults()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 0, 0, 20));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StarinaBottle)
                .AddIngredient<MagicalPowder>()
                .Register();
        }
    }
}
