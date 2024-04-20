using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories
{
    public class MedalOfLife : BaseAccessory
    {
        public MedalOfLife() : base(ItemRarityID.LightRed, Item.sellPrice(0,2,0,0))
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.equippedMedalOfLife = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(8)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
