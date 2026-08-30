using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class BeetleLimbStrap : BaseFlyingShieldAccessory<FlyingShieldAccessoryPage2>, ISpecialDrawBackpacks
    {
        public BeetleLimbStrap() : base(ItemRarityID.Yellow, Item.sellPrice(0, 5))
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
        //        if (cp.ExtraShield3)
        //            return false;

        //        cp.ExtraShield3 = true;

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
                    Helper.PlayPitched(CoraliteSoundID.MinecartTrack_Item52, player.Center, pitch: -1f);
                    Helper.PlayPitched(CoraliteSoundID.Swing_Item1, player.Center);
                    cp.ExtraShield3 = false;
                }
                else
                {
                    Helper.PlayPitched(CoraliteSoundID.Knock_Item37, player.Center);
                    cp.ExtraShield3 = true;
                }

                return true;
            }

            return base.UseItem(player);
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
