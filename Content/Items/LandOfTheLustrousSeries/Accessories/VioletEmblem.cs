using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries.Accessories
{
    [AutoloadEquip(EquipType.HandsOn)]
    [PlayerEffect]
    public class VioletEmblem() : BaseAccessory(ItemRarityID.Lime, Item.sellPrice(0, 6))
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(EightsquareHand));
                cp.AddEffect(nameof(VioletEmblem));
            }

            player.goldRing = true;
            player.hasLuckyCoin = true;
            player.hasLuck_LuckyCoin = true;
            player.discountEquipped = true;

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GreedyRing)
                .AddIngredient<EightsquareHand>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
