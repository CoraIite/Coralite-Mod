using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PrimaryRoughGemstone : ModItem
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ExtractinatorMode[Type] = Type;
        }

        public override void SetDefaults()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 0, 10));
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 15;
            Item.consumable = true;
            Item.autoReuse = true;
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            resultType = Main.rand.NextFromList(
                ModContent.ItemType<Pyrope>(),
                ModContent.ItemType<Aquamarine>(),
                ItemID.Amethyst
                );

            if (Main.rand.NextBool(3))
            {
                resultType = ItemID.StoneBlock;
            }

            resultStack = 1;
        }
    }
}
