using Coralite.Content.Raritys;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnKey : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(3, 17));
        }

        public override void SetDefaults()
        {
            Item.maxStack = 99;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 1));
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }
    }
}
