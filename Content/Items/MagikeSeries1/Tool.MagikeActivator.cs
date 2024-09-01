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
    public class MagikeActivator : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 15;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magikeAmount = 25;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();
            string text;

            if (MagikeHelper.TryGetEntityWithComponent(pos.X, pos.Y, MagikeComponentID.MagikeFactory, out MagikeTileEntity entity))
            {
                MagikeFactory factory = entity.GetSingleComponent<MagikeFactory>(MagikeComponentID.MagikeFactory);

                if (factory.Activation(out text))
                    Helper.PlayPitched("UI/Activation", 0.4f, 0, player.Center);
            }
            else //没找到
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0, player.Center);
                text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.FactoryNotFound);
            }

            PopupText.NewText(new AdvancedPopupRequest()
            {
                Color = Coralite.MagicCrystalPink,
                Text = text,
                DurationInFrames = 60,
                Velocity = -Vector2.UnitY
            }, Main.MouseWorld - (Vector2.UnitY * 32));

            return true;
        }
    }
}
