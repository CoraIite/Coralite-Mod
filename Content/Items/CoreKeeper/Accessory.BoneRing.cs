using Coralite.Core;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class CoreBoneRing : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 40;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = RarityType<UncommonRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 10;
            player.GetCritChance(DamageClass.Generic) += 0.04f;
        }
    }
}
