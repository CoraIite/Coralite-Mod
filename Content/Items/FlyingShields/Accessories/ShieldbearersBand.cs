using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    public class ShieldbearersBand : BaseFlyingShieldAccessory
    {
        public ShieldbearersBand() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
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
                if (cp.ExtraShield1)
                    return false;

                cp.ExtraShield1 = true;

                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Leather)
                .AddIngredient(ItemID.GoldBar, 12)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddIngredient(ItemID.PlatinumBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
