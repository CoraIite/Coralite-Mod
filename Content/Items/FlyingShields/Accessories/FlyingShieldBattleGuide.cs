using Coralite.Content.ModPlayers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class FlyingShieldBattleGuide : BaseFlyingShieldAccessory
    {
        public FlyingShieldBattleGuide() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
        { }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.FlyingShieldLRMeantime = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .AddIngredient(ItemID.Bone, 8)
                .AddTile(TileID.Bookcases)
                .DisableDecraft()
                .Register();
        }
    }
}