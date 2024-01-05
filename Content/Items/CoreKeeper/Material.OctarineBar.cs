using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.CoreKeeper
{
    public class OctarineBar : BaseMaterial
    {
        public OctarineBar() : base(999, Item.sellPrice(0, 0, 25)
            , ModContent.RarityType<RareRarity>(), AssetDirectory.CoreKeeperItems)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OctarineOre>(5)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}
