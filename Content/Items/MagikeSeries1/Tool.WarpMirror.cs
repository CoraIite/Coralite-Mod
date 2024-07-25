using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class WarpMirror : BaseMagikeChargeableItem
    {
        public WarpMirror() : base(150, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeSeries1Item)
        { }

        public override void SetDefs()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.UseSound = CoraliteSoundID.Teleport_Item6;
        }

        public override bool? UseItem(Player player)
        {
            return true;
        }

        public override void HoldItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 2 && (Item.TryCosumeMagike(50) || player.TryCosumeMagike(50)))
                player.TeleportationPotion();
        }
    }
}
