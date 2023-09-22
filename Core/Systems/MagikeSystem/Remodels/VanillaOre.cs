using Coralite.Content.Items.Magike.OtherPlaceables;
using Coralite.Core.Systems.MagikeSystem.CraftConditions;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class VanillaOre : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //木头
            AddRemodelRecipe(ItemID.Wood, 5, ItemID.Coal, selfStack: 20);
            AddRemodelRecipe(ItemID.AshWood, 5, ItemID.Coal, selfStack: 20);
            AddRemodelRecipe(ItemID.RichMahogany, 5, ItemID.Coal, selfStack: 20);
            AddRemodelRecipe(ItemID.Ebonwood, 5, ItemID.Coal, selfStack: 20);
            AddRemodelRecipe(ItemID.Shadewood, 5, ItemID.Coal, selfStack: 20);
            AddRemodelRecipe(ItemID.Pearlwood, 5, ItemID.Coal, selfStack: 20);
            AddRemodelRecipe(ItemID.BorealWood, 5, ItemID.Coal, selfStack: 20);
            AddRemodelRecipe(ItemID.PalmWood, 5, ItemID.Coal, selfStack: 20);

            //煤
            AddRemodelRecipe(ItemID.Coal, 200, ItemID.Diamond, selfStack: 20);

            //沙漠化石
            AddRemodelRecipe(ItemID.DesertFossil, 1, ItemID.Amber, selfStack: 10);

            //黄玉
            AddRemodelRecipe(ItemID.Topaz, 25, ItemID.GemSquirrelTopaz);
            AddRemodelRecipe(ItemID.Topaz, 25, ItemID.GemBunnyTopaz);

            //紫晶
            AddRemodelRecipe(ItemID.Amethyst, 25, ItemID.GemSquirrelAmethyst);
            AddRemodelRecipe(ItemID.Amethyst, 25, ItemID.GemBunnyAmethyst);

            //蓝宝石
            AddRemodelRecipe(ItemID.Sapphire, 25, ItemID.GemSquirrelSapphire);
            AddRemodelRecipe(ItemID.Sapphire, 25, ItemID.GemBunnySapphire);

            //绿宝石
            AddRemodelRecipe(ItemID.Emerald, 25, ItemID.GemSquirrelEmerald);
            AddRemodelRecipe(ItemID.Emerald, 25, ItemID.GemBunnyEmerald);

            //红宝石
            AddRemodelRecipe(ItemID.Ruby, 25, ItemID.GemSquirrelRuby);
            AddRemodelRecipe(ItemID.Ruby, 25, ItemID.GemBunnyRuby);

            //钻石
            AddRemodelRecipe(ItemID.Diamond, 25, ItemID.GemSquirrelDiamond);
            AddRemodelRecipe(ItemID.Diamond, 25, ItemID.GemBunnyDiamond);

            //石头
            AddRemodelRecipe(ItemID.StoneBlock, 1, ItemID.CopperOre, selfStack: 20);
            AddRemodelRecipe(ItemID.StoneBlock, 1, ItemID.TinOre, selfStack: 20);
            AddRemodelRecipe(ItemID.StoneBlock, 50, ItemID.Amethyst, selfStack: 100);
            AddRemodelRecipe(ItemID.StoneBlock, 50, ItemID.Topaz, selfStack: 100);
            AddRemodelRecipe(ItemID.StoneBlock, 100, ItemID.Sapphire, selfStack: 200);
            AddRemodelRecipe(ItemID.StoneBlock, 100, ItemID.Emerald, selfStack: 200);
            AddRemodelRecipe(ItemID.StoneBlock, 200, ItemID.Ruby, selfStack: 300);

            //石头变泥土，虽然微光转化也可以做到，但是使用魔能的话可以翻倍
            AddRemodelRecipe(ItemID.StoneBlock, 10, ItemID.DirtBlock, 2);
            //石头变各种乱七八糟的石头
            AddRemodelRecipe(ItemID.StoneBlock, 10, ItemID.Granite, selfStack: 2);
            AddRemodelRecipe(ItemID.StoneBlock, 10, ItemID.Marble, selfStack: 2);
            AddRemodelRecipe(ItemID.StoneBlock, 10, ItemID.AshBlock, selfStack: 2);
            AddRemodelRecipe(ItemID.StoneBlock, 10, ItemID.MudBlock, selfStack: 2);
            AddRemodelRecipe(ItemID.StoneBlock, 10, ItemID.SandBlock, selfStack: 2);
            AddRemodelRecipe<Basalt>(0f, ItemID.StoneBlock, 15, selfStack: 4);

            //铜和锡
            AddRemodelRecipe(ItemID.CopperOre, 1, ItemID.IronOre);
            AddRemodelRecipe(ItemID.CopperOre, 1, ItemID.LeadOre);
            AddRemodelRecipe(ItemID.CopperOre, 2, ItemID.CopperBar);
            AddRemodelRecipe(ItemID.CopperOre, 2, ItemID.TinBar);

            AddRemodelRecipe(ItemID.TinOre, 1, ItemID.IronOre);
            AddRemodelRecipe(ItemID.TinOre, 1, ItemID.LeadOre);
            AddRemodelRecipe(ItemID.TinOre, 2, ItemID.CopperBar);
            AddRemodelRecipe(ItemID.TinOre, 2, ItemID.TinBar);

            AddRemodelRecipe(ItemID.CopperBar, 2, ItemID.IronBar);
            AddRemodelRecipe(ItemID.CopperBar, 2, ItemID.LeadBar);
            AddRemodelRecipe(ItemID.TinBar, 2, ItemID.IronBar);
            AddRemodelRecipe(ItemID.TinBar, 2, ItemID.LeadBar);

            //铁和铅
            AddRemodelRecipe(ItemID.IronOre, 3, ItemID.SilverOre);
            AddRemodelRecipe(ItemID.IronOre, 3, ItemID.TungstenOre);
            AddRemodelRecipe(ItemID.IronOre, 6, ItemID.IronBar);
            AddRemodelRecipe(ItemID.IronOre, 6, ItemID.LeadBar);

            AddRemodelRecipe(ItemID.LeadOre, 3, ItemID.SilverOre);
            AddRemodelRecipe(ItemID.LeadOre, 3, ItemID.TungstenOre);
            AddRemodelRecipe(ItemID.LeadOre, 6, ItemID.IronBar);
            AddRemodelRecipe(ItemID.LeadOre, 6, ItemID.LeadBar);

            AddRemodelRecipe(ItemID.IronBar, 6, ItemID.SilverBar);
            AddRemodelRecipe(ItemID.IronBar, 6, ItemID.TungstenBar);
            AddRemodelRecipe(ItemID.IronBar, 150, ItemID.AncientIronHelmet,selfStack:10);
            AddRemodelRecipe(ItemID.LeadBar, 6, ItemID.SilverBar);
            AddRemodelRecipe(ItemID.LeadBar, 6, ItemID.TungstenBar);
            AddRemodelRecipe(ItemID.LeadBar, 150, ItemID.AncientIronHelmet, selfStack: 10);

            //银和钨
            AddRemodelRecipe(ItemID.SilverOre, 5, ItemID.GoldOre);
            AddRemodelRecipe(ItemID.SilverOre, 5, ItemID.PlatinumOre);
            AddRemodelRecipe(ItemID.SilverOre, 10, ItemID.SilverBar);
            AddRemodelRecipe(ItemID.SilverOre, 10, ItemID.TungstenBar);

            AddRemodelRecipe(ItemID.TungstenOre, 5, ItemID.GoldOre);
            AddRemodelRecipe(ItemID.TungstenOre, 5, ItemID.PlatinumOre);
            AddRemodelRecipe(ItemID.TungstenOre, 10, ItemID.SilverBar);
            AddRemodelRecipe(ItemID.TungstenOre, 10, ItemID.TungstenBar);

            AddRemodelRecipe(ItemID.SilverBar, 10, ItemID.GoldBar);
            AddRemodelRecipe(ItemID.SilverBar, 10, ItemID.PlatinumBar);
            AddRemodelRecipe(ItemID.TungstenBar, 10, ItemID.GoldBar);
            AddRemodelRecipe(ItemID.TungstenBar, 10, ItemID.PlatinumBar);

            //金和铂金
            AddRemodelRecipe(ItemID.GoldOre, 10, ItemID.CrimtaneOre);
            AddRemodelRecipe(ItemID.GoldOre, 10, ItemID.DemoniteOre);
            AddRemodelRecipe(ItemID.GoldOre, 10, ItemID.Meteorite);
            AddRemodelRecipe(ItemID.GoldOre, 15, ItemID.GoldBar);
            AddRemodelRecipe(ItemID.GoldOre, 15, ItemID.PlatinumBar);

            AddRemodelRecipe(ItemID.PlatinumOre, 10, ItemID.CrimtaneOre);
            AddRemodelRecipe(ItemID.PlatinumOre, 10, ItemID.DemoniteOre);
            AddRemodelRecipe(ItemID.PlatinumOre, 10, ItemID.Meteorite);
            AddRemodelRecipe(ItemID.PlatinumOre, 15, ItemID.GoldBar);
            AddRemodelRecipe(ItemID.PlatinumOre, 15, ItemID.PlatinumBar);

            AddRemodelRecipe(ItemID.GoldBar, 15, ItemID.CrimtaneBar);
            AddRemodelRecipe(ItemID.GoldBar, 15, ItemID.DemoniteBar);
            AddRemodelRecipe(ItemID.GoldBar, 15, ItemID.MeteoriteBar);
            AddRemodelRecipe(ItemID.GoldBar, 150, ItemID.AncientGoldHelmet,selfStack:10);
            AddRemodelRecipe(ItemID.GoldBar, 100, ItemID.GoldChest, selfStack: 15);
            AddRemodelRecipe(ItemID.GoldBar, 400, ItemID.GoldenKey, selfStack: 20, condition: DownedSkeletronCondition.Instance);
            AddRemodelRecipe(ItemID.PlatinumBar, 15, ItemID.CrimtaneBar);
            AddRemodelRecipe(ItemID.PlatinumBar, 15, ItemID.DemoniteBar);
            AddRemodelRecipe(ItemID.PlatinumBar, 15, ItemID.MeteoriteBar);
            AddRemodelRecipe(ItemID.PlatinumBar, 150, ItemID.AncientGoldHelmet, selfStack: 10);
            AddRemodelRecipe(ItemID.PlatinumBar, 100, ItemID.GoldChest, selfStack: 15);

            //猩红矿和魔矿
            AddRemodelRecipe(ItemID.CrimtaneOre, 15, ItemID.TissueSample, selfStack: 3, condition: DownedEvilBossCondition.Instance);
            AddRemodelRecipe(ItemID.CrimtaneOre, 15, ItemID.ShadowScale, selfStack: 3, condition: DownedEvilBossCondition.Instance);
            AddRemodelRecipe(ItemID.CrimtaneOre, 15, ItemID.Hellstone);
            AddRemodelRecipe(ItemID.CrimtaneOre, 20, ItemID.CrimtaneBar);
            AddRemodelRecipe(ItemID.CrimtaneOre, 20, ItemID.DemoniteBar);

            AddRemodelRecipe(ItemID.DemoniteOre, 15, ItemID.TissueSample, selfStack: 3, condition: DownedEvilBossCondition.Instance);
            AddRemodelRecipe(ItemID.DemoniteOre, 15, ItemID.ShadowScale, selfStack: 3, condition: DownedEvilBossCondition.Instance);
            AddRemodelRecipe(ItemID.DemoniteOre, 15, ItemID.Hellstone);
            AddRemodelRecipe(ItemID.DemoniteOre, 20, ItemID.CrimtaneBar);
            AddRemodelRecipe(ItemID.DemoniteOre, 20, ItemID.DemoniteBar);

            AddRemodelRecipe(ItemID.CrimtaneBar, 150, ItemID.AncientShadowHelmet, selfStack: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, 150, ItemID.AncientShadowScalemail, selfStack: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, 150, ItemID.AncientShadowGreaves, selfStack: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, 25, ItemID.HellstoneBar);
            AddRemodelRecipe(ItemID.DemoniteBar, 150, ItemID.AncientShadowHelmet, selfStack: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, 150, ItemID.AncientShadowScalemail, selfStack: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, 150, ItemID.AncientShadowGreaves, selfStack: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, 25, ItemID.HellstoneBar);

            //陨石矿
            AddRemodelRecipe(ItemID.Meteorite, 15, ItemID.Hellstone);
            AddRemodelRecipe(ItemID.Meteorite, 20, ItemID.MeteoriteBar);

            AddRemodelRecipe(ItemID.MeteoriteBar, 25, ItemID.HellstoneBar);

            //狱岩
            AddRemodelRecipe(ItemID.Hellstone, 25, ItemID.HellstoneBar);
            AddRemodelRecipe(ItemID.Hellstone, 25, ItemID.LivingFireBlock,3);

            AddRemodelRecipe(ItemID.Hellstone, 50, ItemID.CobaltOre, condition: HardModeCondition.Instance);
            AddRemodelRecipe(ItemID.Hellstone, 50, ItemID.PalladiumOre, condition: HardModeCondition.Instance);

            AddRemodelRecipe(ItemID.HellstoneBar, 65, ItemID.CobaltBar, condition: HardModeCondition.Instance);
            AddRemodelRecipe(ItemID.HellstoneBar, 65, ItemID.PalladiumBar, condition: HardModeCondition.Instance);

            //钴矿和钯金矿
            AddRemodelRecipe(ItemID.CobaltOre, 70, ItemID.MythrilOre);
            AddRemodelRecipe(ItemID.CobaltOre, 70, ItemID.OrichalcumOre);
            AddRemodelRecipe(ItemID.CobaltOre, 60, ItemID.CobaltBar);
            AddRemodelRecipe(ItemID.CobaltOre, 60, ItemID.PalladiumBar);

            AddRemodelRecipe(ItemID.PalladiumOre, 70, ItemID.MythrilOre);
            AddRemodelRecipe(ItemID.PalladiumOre, 70, ItemID.OrichalcumOre);
            AddRemodelRecipe(ItemID.PalladiumOre, 60, ItemID.CobaltBar);
            AddRemodelRecipe(ItemID.PalladiumOre, 60, ItemID.PalladiumBar);

            AddRemodelRecipe(ItemID.CobaltBar, 100, ItemID.MythrilBar);
            AddRemodelRecipe(ItemID.CobaltBar, 100, ItemID.OrichalcumBar);
            AddRemodelRecipe(ItemID.PalladiumBar, 100, ItemID.MythrilBar);
            AddRemodelRecipe(ItemID.PalladiumBar, 100, ItemID.OrichalcumBar);

            //秘银矿和山铜矿
            AddRemodelRecipe(ItemID.MythrilOre, 100, ItemID.TitaniumOre);
            AddRemodelRecipe(ItemID.MythrilOre, 100, ItemID.AdamantiteOre);
            AddRemodelRecipe(ItemID.MythrilOre, 80, ItemID.MythrilBar);
            AddRemodelRecipe(ItemID.MythrilOre, 80, ItemID.OrichalcumBar);

            AddRemodelRecipe(ItemID.OrichalcumOre, 100, ItemID.TitaniumOre);
            AddRemodelRecipe(ItemID.OrichalcumOre, 100, ItemID.AdamantiteOre);
            AddRemodelRecipe(ItemID.OrichalcumOre, 80, ItemID.MythrilBar);
            AddRemodelRecipe(ItemID.OrichalcumOre, 80, ItemID.OrichalcumBar);

            AddRemodelRecipe(ItemID.MythrilBar, 150, ItemID.TitaniumBar);
            AddRemodelRecipe(ItemID.MythrilBar, 150, ItemID.AdamantiteBar);
            AddRemodelRecipe(ItemID.OrichalcumBar, 150, ItemID.TitaniumBar);
            AddRemodelRecipe(ItemID.OrichalcumBar, 150, ItemID.AdamantiteBar);

            //钛金和精金
            AddRemodelRecipe(ItemID.TitaniumOre, 120, ItemID.TitaniumBar);
            AddRemodelRecipe(ItemID.TitaniumOre, 120, ItemID.AdamantiteBar);
            AddRemodelRecipe(ItemID.AdamantiteOre, 120, ItemID.TitaniumBar);
            AddRemodelRecipe(ItemID.AdamantiteOre, 120, ItemID.AdamantiteBar);

            AddRemodelRecipe(ItemID.TitaniumBar, 180, ItemID.HallowedBar,condition:DownedAnyMachineBossCondition.Instance);
            AddRemodelRecipe(ItemID.AdamantiteBar, 180, ItemID.HallowedBar, condition: DownedAnyMachineBossCondition.Instance);

            //神圣锭
            AddRemodelRecipe(ItemID.HallowedBar, 230, ItemID.ChlorophyteBar, condition: DownedAllMachineBossCondition.Instance);

            //叶绿矿
            AddRemodelRecipe(ItemID.ChlorophyteOre, 500, ItemID.LunarOre, condition: DownedMoonlordCondition.Instance);
            AddRemodelRecipe(ItemID.ChlorophyteBar, 500, ItemID.LunarBar, condition: DownedMoonlordCondition.Instance);
        }
    }
}
