using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class CondensedCrystalBall : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = Item.useTime = 20;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.mana = 25;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.UseSound = CoraliteSoundID.ManaCrystal_Item29;
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magikeAmount = 50;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();

            if (MagikeHelper.TryGetEntityWithComponent<MagikeContainer>(pos.X, pos.Y, MagikeComponentID.MagikeContainer, out MagikeTP entity))
            {
                if (player.statMana > 20)
                {
                    entity.GetMagikeContainer().AddMagike(1);
                    return true;
                }

                return false;
            }

            PopupText.NewText(new AdvancedPopupRequest()
            {
                Color = Coralite.MagicCrystalPink,
                Text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ChargeNotFound),
                DurationInFrames = 60,
                Velocity = -Vector2.UnitY
            }, Main.MouseWorld - (Vector2.UnitY * 32));

            return false;
        }
    }
}
