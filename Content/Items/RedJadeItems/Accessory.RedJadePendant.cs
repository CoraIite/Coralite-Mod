using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJadeItems
{
    [AutoloadEquip(EquipType.Waist)]
    public class RedJadePendant : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("受到攻击时有概率产生爆炸");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1);
            Item.accessory = true;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CoralitePlayer>().RedJadePendant = true;
        }
    }
}
