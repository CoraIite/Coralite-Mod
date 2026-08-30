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
    public class PowerliftExoskeleton : BaseFlyingShieldAccessory<FlyingShieldAccessoryPage2>, ISpecialDrawBackpacks
    {
        public PowerliftExoskeleton() : base(ItemRarityID.Pink, Item.sellPrice(0, 1, 30))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.vanity = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 20;
            //Item.UseSound = CoraliteSoundID.Knock_Item37;
        }

        public Vector2 ExtraOffset => new(0, 10);

        //public override bool CanUseItem(Player player)
        //{
        //    if (player.TryGetModPlayer(out CoralitePlayer cp))
        //    {
        //        if (cp.ExtraShield2)
        //            return false;

        //        cp.ExtraShield2 = true;

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
                    cp.ExtraShield2 = false;
                }
                else
                {
                    Helper.PlayPitched(CoraliteSoundID.Knock_Item37, player.Center);
                    cp.ExtraShield2 = true;
                }

                return true;
            }

            return base.UseItem(player);
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
