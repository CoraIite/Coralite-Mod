using Coralite.Content.Items.LandOfTheLustrousSeries.Accessories;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class SeniorRoughGemstone : ModItem
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ExtractinatorMode[Type] = Type;
        }

        public override void SetDefaults()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightRed4, Item.sellPrice(0, 0, 10));
            Item.maxStack = Item.CommonMaxStack;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 15;
            Item.consumable = true;
            Item.autoReuse = true;
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            resultStack = 1;

            var wr = new WeightedRandom<int>(Main.rand);

            wr.Add(ModContent.ItemType<Zumurud>());
            wr.Add(ItemID.Ruby, 0.5f);
            wr.Add(ModContent.ItemType<Peridot>());
            wr.Add(ItemID.Sapphire, 0.5f);
            wr.Add(ModContent.ItemType<Tourmaline>());
            wr.Add(ItemID.Topaz, 0.5f);
            wr.Add(ModContent.ItemType<Zircon>());

            wr.Add(ModContent.ItemType<Pyrope>(), 0.5f);
            wr.Add(ModContent.ItemType<Aquamarine>(), 0.5f);
            wr.Add(ModContent.ItemType<PinkDiamond>(), 0.5f);
            wr.Add(ItemID.Diamond, 0.3f);
            wr.Add(ItemID.Amethyst, 0.3f);

            wr.Add(ModContent.ItemType<SilkAgate>(), 0.1f);

            wr.Add(ItemID.StoneBlock, 0.5f);

            resultType = wr.Get();
        }
    }
}
