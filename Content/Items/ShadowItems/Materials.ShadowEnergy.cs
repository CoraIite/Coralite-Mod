using Coralite.Core;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;

namespace Coralite.Content.Items.ShadowItems
{
    public class ShadowEnergy : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("影子能量");
            Tooltip.SetDefault("它是无形的");

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0,0,5,50);
            Item.maxStack = 999;
            Item.height = Item.width= 40;
        }
    }
}
