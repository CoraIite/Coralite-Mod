﻿using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class OceanheartNecklace : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = RarityType<EpicRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.lifeReganBonus += 0.21f;
                cp.LifeMaxModifyer.Flat += 42;
            }
        }
    }
}
