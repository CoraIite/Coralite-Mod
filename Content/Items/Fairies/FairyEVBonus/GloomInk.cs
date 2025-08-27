using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies.FairyEVBonus
{
    public class GloomInk : ModItem
    {
        public override string Texture => AssetDirectory.FairyEVBonus + Name;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
