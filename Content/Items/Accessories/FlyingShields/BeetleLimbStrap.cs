using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    [AutoloadEquip(EquipType.Back)]
    public class BeetleLimbStrap : BaseAccessory,ISpecialDrawBackPacks
    {
        public BeetleLimbStrap() : base(ItemRarityID.Yellow, Item.sellPrice(0, 5))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<ShieldbearersBand>()//下位
                || equippedItem.type == ModContent.ItemType<PowerliftExoskeleton>())//下位

                && incomingItem.type == ModContent.ItemType<BeetleLimbStrap>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.MaxFlyingShield = 4;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PowerliftExoskeleton>()
                .AddIngredient(ItemID.BeetleHusk, 4)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
