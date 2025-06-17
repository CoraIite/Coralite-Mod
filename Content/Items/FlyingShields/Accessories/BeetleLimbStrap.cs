using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class BeetleLimbStrap : BaseFlyingShieldAccessory, ISpecialDrawBackpacks
    {
        public BeetleLimbStrap() : base(ItemRarityID.Yellow, Item.sellPrice(0, 5))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.vanity = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 20;
            Item.UseSound = CoraliteSoundID.Knock_Item37;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.ExtraShield3)
                    return false;

                cp.ExtraShield3 = true;

                return true;
            }

            return false;
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
