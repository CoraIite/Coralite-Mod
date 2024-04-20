using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Terraria.ID;
using Terraria;

namespace Coralite.Content.Items.Accessories
{
    public class LifePulseDevice:BaseAccessory
    {
        public LifePulseDevice() : base(ItemRarityID.LightRed, Item.sellPrice(0, 2, 0, 0))
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.equippedLifePulseDevice = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SteelBar>(8)
                .AddIngredient(ItemID.DeathweedSeeds)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
