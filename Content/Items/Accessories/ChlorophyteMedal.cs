using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories
{
    public class ChlorophyteMedal : BaseAccessory, IFlyingShieldAccessory
    {
        public ChlorophyteMedal() : base(ItemRarityID.Lime, Item.sellPrice(0, 1, 50))
        { }

        public void OnInitialize(BaseFlyingShield projectile)
        {
            projectile.canChase = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar,10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
