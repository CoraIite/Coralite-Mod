using Coralite.Content.ModPlayers;
using Coralite.Content.Raritys;
using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries1
{
    [AutoloadEquip(EquipType.Face)]
    public class MagikeMonoclastic : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();

            Item.value = Item.sellPrice(0, 0, 10, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (hideVisual)
                return;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(MagikeMonoclastic));
        }
    }
}
