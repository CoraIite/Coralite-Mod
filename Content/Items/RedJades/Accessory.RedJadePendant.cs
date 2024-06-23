﻿using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.RedJades
{
    [AutoloadEquip(EquipType.Waist)]
    public class RedJadePendant : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1);
            Item.accessory = true;
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(RedJadePendant));
        }
    }
}
