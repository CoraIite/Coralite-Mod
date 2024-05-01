using Coralite.Core;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class SkyRing : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = RarityType<EpicRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 7f;
            player.GetDamage(DamageClass.Melee) += 0.085f;
        }
    }
}
