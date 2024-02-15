using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories
{
    public class LevitationBlossom : BaseAccessory, IFlyingShieldAccessory
    {
        public LevitationBlossom() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
        { }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.flyingTime = (int)(projectile.flyingTime * 1.3f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.VanityTreeSakuraSeed)
                .AddIngredient(ItemID.Feather)
                .AddIngredient(ItemID.TissueSample, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.VanityTreeSakuraSeed)
                .AddIngredient(ItemID.Feather)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
