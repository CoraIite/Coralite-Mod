using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    [AutoloadEquip(EquipType.Waist)]
    public class ShieldbearersBand : BaseFlyingShieldAccessory<FlyingShieldAccessoryPage2>
    {
        public ShieldbearersBand() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 20))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.vanity = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 20;
            //Item.UseSound = CoraliteSoundID.Knock_Item37;
        }

        //public override bool CanUseItem(Player player)
        //{
        //    if (player.TryGetModPlayer(out CoralitePlayer cp))
        //    {
        //        if (cp.ExtraShield1)
        //            return false;

        //        cp.ExtraShield1 = true;

        //        return true;
        //    }

        //    return false;
        //}

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (player.altFunctionUse == 2)
                {
                    Helper.PlayPitched(CoraliteSoundID.MinecartTrack_Item52, player.Center,pitch:-1f);
                    Helper.PlayPitched(CoraliteSoundID.Swing_Item1, player.Center);
                    cp.ExtraShield1 = false;
                }
                else
                {
                    Helper.PlayPitched(CoraliteSoundID.Knock_Item37, player.Center);
                    cp.ExtraShield1 = true;
                }

                return true;
            }

            return base.UseItem(player);
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

            CreateRecipe()
                .AddIngredient(ItemID.Leather)
                .AddIngredient(ItemID.PlatinumBar, 12)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddIngredient(ItemID.GoldBar, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
