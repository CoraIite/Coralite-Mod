using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.ShadowCastle
{
    public class ShadowMagneticCard:ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems+Name;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 99;
            Item.value = Item.sellPrice(0, 0, 0, 10);
        }
    }
}
