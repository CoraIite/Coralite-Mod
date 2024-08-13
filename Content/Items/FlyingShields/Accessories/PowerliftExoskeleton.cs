using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class PowerliftExoskeleton : BaseFlyingShieldAccessory, ISpecialDrawBackpacks
    {
        public PowerliftExoskeleton() : base(ItemRarityID.Pink, Item.sellPrice(0, 1, 30))
        { }

        public Vector2 ExtraOffset => new(0, 10);

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<ShieldbearersBand>()//下位
                || equippedItem.type == ModContent.ItemType<BeetleLimbStrap>())//上位

                && incomingItem.type == ModContent.ItemType<PowerliftExoskeleton>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.MaxFlyingShield = 3;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShieldbearersBand>()
                .AddIngredient(ItemID.HallowedBar, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
