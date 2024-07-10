using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Magike
{
    public class CrystalCrate : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsFishingCrate[Type] = true;

            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystalCrateTile>());
            Item.width = 12;
            Item.height = 12;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 2);
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            //必掉书页，作为空岛中获取书页的方法
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Page_MagikeBase>(), 1, 1, 1));
            // 掉玄武岩
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Basalt>(), 3, 10, 35));
            // 掉魔力晶体
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagicCrystal>(), 1, 5, 20));

            // 钱币
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 8, 1, 2));
            itemLoot.Add(ItemDropRule.Common(ItemID.SilverCoin, 2, 1, 75));
            itemLoot.Add(ItemDropRule.Common(ItemID.CopperCoin, 1, 1, 99));

            // 矿物
            IItemDropRule[] oreTypes = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.CopperOre, 1, 4, 10),
                ItemDropRule.Common(ItemID.TinOre, 1, 4, 10),
                ItemDropRule.Common(ItemID.IronOre, 1, 3, 8),
                ItemDropRule.Common(ItemID.LeadOre, 1, 3, 8),
                ItemDropRule.Common(ItemID.SilverOre, 1, 2, 7),
                ItemDropRule.Common(ItemID.TungstenOre, 1, 2, 7),
                ItemDropRule.Common(ItemID.GoldOre, 1, 1, 6),
                ItemDropRule.Common(ItemID.PlatinumOre, 1, 1, 6),
            };
            itemLoot.Add(new OneFromRulesRule(7, oreTypes));

            // 矿物锭
            IItemDropRule[] oreBars = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.IronBar, 1, 2, 4),
                ItemDropRule.Common(ItemID.LeadBar, 1, 2, 4),
                ItemDropRule.Common(ItemID.SilverBar, 1, 1, 3),
                ItemDropRule.Common(ItemID.TungstenBar, 1, 1, 3),
                ItemDropRule.Common(ItemID.GoldBar, 1, 1, 3),
                ItemDropRule.Common(ItemID.PlatinumBar, 1, 1, 3),
            };
            itemLoot.Add(new OneFromRulesRule(4, oreBars));

            // Drop an "exploration utility" potion, with the addition of one from ExampleMod
            IItemDropRule[] explorationPotions = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.GravitationPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 5),
            };
            itemLoot.Add(new OneFromRulesRule(4, explorationPotions));

            // Drop (pre-hm) resource potion
            IItemDropRule[] resourcePotions = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 18),
                ItemDropRule.Common(ItemID.ManaPotion, 1, 5, 18),
            };
            itemLoot.Add(new OneFromRulesRule(2, resourcePotions));

            // Drop (high-end) bait
            IItemDropRule[] highendBait = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 7),
                ItemDropRule.Common(ItemID.MasterBait, 1, 2, 7),
            };
            itemLoot.Add(new OneFromRulesRule(2, highendBait));
        }

    }

    public class CrystalCrateTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true; // Optional, if you add more placeStyles for the item 
            TileObjectData.addTile(Type);

            // Etc
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(Coralite.Instance.MagicCrystalPink, name);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }
    }

}
