using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    [AutoloadEquip(EquipType.Waist)]
    public class MedalOfLife : BaseAccessory
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public MedalOfLife() : base(ItemRarityID.LightRed, Item.sellPrice(0, 2, 0, 0))
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(MedalOfLife));
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
