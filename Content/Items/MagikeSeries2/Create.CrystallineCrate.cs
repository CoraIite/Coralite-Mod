using Coralite.Content.Raritys;
using Coralite.Core;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineCrate : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsFishingCrate[Type] = true;

            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystallineCrateTile>());
            Item.width = 12;
            Item.height = 12;
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
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
            //必掉卷轴，作为空岛中获取卷轴的方法
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Reel_MagikeAdvance>(), 1, 1, 1));
            // 掉矽卡岩
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Skarn>(), 3, 10, 35));
            // 掉蕴魔水晶
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 5, 20));

            // 钱币
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 8, 1, 2));
            itemLoot.Add(ItemDropRule.Common(ItemID.SilverCoin, 2, 1, 75));
            itemLoot.Add(ItemDropRule.Common(ItemID.CopperCoin, 1, 1, 99));

            // Drop an "exploration utility" potion, with the addition of one from ExampleMod
            IItemDropRule[] explorationPotions = [
                ItemDropRule.Common(ItemID.InvisibilityPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.GravitationPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 5),
            ];
            itemLoot.Add(new OneFromRulesRule(4, explorationPotions));
        }
    }

    public class CrystallineCrateTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.StyleHorizontal = true; // Optional, if you add more placeStyles for the item 
            TileObjectData.addTile(Type);

            // Etc
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(Coralite.CrystallinePurple, name);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }
    }
}
