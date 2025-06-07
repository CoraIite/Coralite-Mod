using Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

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
            Item.useTime = Item.useAnimation = 8;
            Item.consumable = true;
            Item.autoReuse = true;
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            KnowledgeSystem.CheckForUnlock<LandOfTheLustrousKnowledge>(Main.LocalPlayer.Center, new Color(247, 239, 208));

            var wr = new WeightedRandom<int>(Main.rand);

            wr.Add(ModContent.ItemType<Pyrope>());
            wr.Add(ModContent.ItemType<Aquamarine>());
            wr.Add(ModContent.ItemType<PinkDiamond>());
            wr.Add(ItemID.Diamond, 0.8f);
            wr.Add(ItemID.Amethyst, 0.8f);

            wr.Add(ItemID.StoneBlock, 0.8f);

            resultType = wr.Get();

            resultStack = 1;
        }
    }
}
