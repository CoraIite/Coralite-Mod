using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Terraria.ModLoader.ModContent;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class Ore : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //木头
            AddRemodelRecipe(ItemID.Wood, ItemID.Coal, CalculateMagikeCost(MALevel.MagicCrystal, 4,15), mainStack: 18);
            AddRemodelRecipe(ItemID.AshWood, ItemID.Coal, CalculateMagikeCost(MALevel.MagicCrystal, 4, 15), mainStack: 18);
            AddRemodelRecipe(ItemID.RichMahogany, ItemID.Coal, CalculateMagikeCost(MALevel.MagicCrystal, 4, 15), mainStack: 18);
            AddRemodelRecipe(ItemID.Ebonwood, ItemID.Coal, CalculateMagikeCost(MALevel.MagicCrystal, 4, 15), mainStack: 18);
            AddRemodelRecipe(ItemID.Shadewood, ItemID.Coal, CalculateMagikeCost(MALevel.MagicCrystal, 4, 15), mainStack: 18);
            AddRemodelRecipe(ItemID.Pearlwood, ItemID.Coal, CalculateMagikeCost(MALevel.MagicCrystal, 4, 15), mainStack: 18);
            AddRemodelRecipe(ItemID.BorealWood, ItemID.Coal, CalculateMagikeCost(MALevel.MagicCrystal, 4, 15), mainStack: 18);
            AddRemodelRecipe(ItemID.PalmWood, ItemID.Coal, CalculateMagikeCost(MALevel.MagicCrystal, 4, 15), mainStack: 18);

            //煤
            AddRemodelRecipe(ItemID.Coal, ItemID.Diamond, CalculateMagikeCost(RedJade, 6), mainStack: 6);

            //沙漠化石
            AddRemodelRecipe(ItemID.DesertFossil, ItemID.Amber, CalculateMagikeCost(MALevel.MagicCrystal, 4, 15), mainStack: 10);

            //黄玉
            AddRemodelRecipe(ItemID.Topaz, ItemID.GemSquirrelTopaz, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Topaz, ItemID.GemBunnyTopaz, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));

            //紫晶
            AddRemodelRecipe(ItemID.Amethyst, ItemID.GemSquirrelAmethyst, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Amethyst, ItemID.GemBunnyAmethyst, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));

            //蓝宝石
            AddRemodelRecipe(ItemID.Sapphire, ItemID.GemSquirrelSapphire, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Sapphire, ItemID.GemBunnySapphire, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));

            //绿宝石
            AddRemodelRecipe(ItemID.Emerald, ItemID.GemSquirrelEmerald, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Emerald, ItemID.GemBunnyEmerald, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));

            //红宝石
            AddRemodelRecipe(ItemID.Ruby, ItemID.GemSquirrelRuby, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Ruby, ItemID.GemBunnyRuby, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));

            //钻石
            AddRemodelRecipe(ItemID.Diamond, ItemID.GemSquirrelDiamond, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Diamond, ItemID.GemBunnyDiamond, CalculateMagikeCost(MALevel.MagicCrystal, 5, 15));

            //石头            
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Sapphire, 100, mainStack: 200);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Emerald, 100, mainStack: 200);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Ruby, 200, mainStack: 300);

            //石头变泥土，虽然微光转化也可以做到，但是使用魔能的话可以翻倍
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.DirtBlock, CalculateMagikeCost(MALevel.MagicCrystal, 4, 5), resultStack:2);
            //石头变各种乱七八糟的石头
            //AddRemodelRecipe(ItemID.StoneBlock, ItemID.Granite, 10, mainStack: 2);
            //AddRemodelRecipe(ItemID.StoneBlock, ItemID.Marble, 10, mainStack: 2);
            //AddRemodelRecipe(ItemID.StoneBlock, ItemID.AshBlock, 10, mainStack: 2);
            //AddRemodelRecipe(ItemID.StoneBlock, ItemID.MudBlock, 10, mainStack: 2);
            //AddRemodelRecipe(ItemID.StoneBlock, ItemID.SandBlock, 10, mainStack: 2);
            //AddRemodelRecipe(ItemID.StoneBlock, ItemType<Basalt>(), 15, mainStack: 4);

            #region 铜和锡
            //均为2矿变一个锭，使用泥土就可以变成铜和锡
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.CopperOre, CalculateMagikeCost(MALevel.MagicCrystal, 6, 15), 4);
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.TinOre, CalculateMagikeCost(MALevel.MagicCrystal, 6, 15), 4);

            //AddRemodelRecipe(ItemID.CopperOre, ItemID.IronOre, 1);
            //AddRemodelRecipe(ItemID.CopperOre, ItemID.LeadOre, 1);
            AddRemodelRecipe(ItemID.CopperOre, ItemID.CopperBar, CalculateMagikeCost(MALevel.MagicCrystal, 6, 15),2);
            AddRemodelRecipe(ItemID.CopperOre, ItemID.TinBar, CalculateMagikeCost(MALevel.MagicCrystal, 6, 15),2);

            //AddRemodelRecipe(ItemID.TinOre, ItemID.IronOre, 1);
            //AddRemodelRecipe(ItemID.TinOre, ItemID.LeadOre, 1);
            AddRemodelRecipe(ItemID.TinOre, ItemID.CopperBar, CalculateMagikeCost(MALevel.MagicCrystal, 6, 15),2);
            AddRemodelRecipe(ItemID.TinOre, ItemID.TinBar, CalculateMagikeCost(MALevel.MagicCrystal, 6, 15), 2);

            //AddRemodelRecipe(ItemID.CopperBar, ItemID.IronBar, 2);
            //AddRemodelRecipe(ItemID.CopperBar, ItemID.LeadBar, 2);
            //AddRemodelRecipe(ItemID.TinBar, ItemID.IronBar, 2);
            //AddRemodelRecipe(ItemID.TinBar, ItemID.LeadBar, 2);

            #endregion

            #region 铁和铅
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.IronOre, CalculateMagikeCost(RedJade, 6, 15), 4);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.TinOre, CalculateMagikeCost(RedJade, 6, 15), 4);

            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Amethyst, CalculateMagikeCost(RedJade, 4, 10), mainStack: 18);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Topaz, CalculateMagikeCost(RedJade, 4, 10), mainStack: 18);

            //AddRemodelRecipe(ItemID.IronOre, ItemID.SilverOre, 3);
            //AddRemodelRecipe(ItemID.IronOre, ItemID.TungstenOre, 3);
            AddRemodelRecipe(ItemID.IronOre, ItemID.IronBar, CalculateMagikeCost(RedJade, 6, 15), 2);
            AddRemodelRecipe(ItemID.IronOre, ItemID.LeadBar, CalculateMagikeCost(RedJade, 6, 15), 2);

            //AddRemodelRecipe(ItemID.LeadOre, ItemID.SilverOre, 3);
            //AddRemodelRecipe(ItemID.LeadOre, ItemID.TungstenOre, 3);
            AddRemodelRecipe(ItemID.LeadOre, ItemID.IronBar, CalculateMagikeCost(RedJade, 6, 15), 2);
            AddRemodelRecipe(ItemID.LeadOre, ItemID.LeadBar, CalculateMagikeCost(RedJade, 6, 15), 2);

            //AddRemodelRecipe(ItemID.IronBar, ItemID.SilverBar, 6);
            //AddRemodelRecipe(ItemID.IronBar, ItemID.TungstenBar, 6);
            AddRemodelRecipe(ItemID.IronBar, ItemID.AncientIronHelmet, CalculateMagikeCost(RedJade, 6, 120), mainStack: 10);
            //AddRemodelRecipe(ItemID.LeadBar, ItemID.SilverBar, 6);
            //AddRemodelRecipe(ItemID.LeadBar, ItemID.TungstenBar, 6);
            AddRemodelRecipe(ItemID.LeadBar, ItemID.AncientIronHelmet, CalculateMagikeCost(RedJade, 6, 120), mainStack: 10);

            #endregion

            #region 银和钨
            AddRemodelRecipe(ItemID.Granite, ItemID.SilverOre, CalculateMagikeCost(RedJade, 6, 20), 4);
            AddRemodelRecipe(ItemID.Granite, ItemID.TungstenOre, CalculateMagikeCost(RedJade, 6, 20), 4);

            AddRemodelRecipe(ItemID.Granite, ItemID.Sapphire, CalculateMagikeCost(RedJade, 6, 20), mainStack: 18);
            AddRemodelRecipe(ItemID.Granite, ItemID.Emerald, CalculateMagikeCost(RedJade, 6, 20), mainStack: 18);

            //AddRemodelRecipe(ItemID.SilverOre, ItemID.GoldOre, 5);
            //AddRemodelRecipe(ItemID.SilverOre, ItemID.PlatinumOre, 5);
            AddRemodelRecipe(ItemID.SilverOre, ItemID.SilverBar, CalculateMagikeCost(RedJade, 6, 20), 2);
            AddRemodelRecipe(ItemID.SilverOre, ItemID.TungstenBar, CalculateMagikeCost(RedJade, 6, 20), 2);

            //AddRemodelRecipe(ItemID.TungstenOre, ItemID.GoldOre, 5);
            //AddRemodelRecipe(ItemID.TungstenOre, ItemID.PlatinumOre, 5);
            AddRemodelRecipe(ItemID.TungstenOre, ItemID.SilverBar, CalculateMagikeCost(RedJade, 6, 20), 2);
            AddRemodelRecipe(ItemID.TungstenOre, ItemID.TungstenBar, CalculateMagikeCost(RedJade, 6, 20), 2);

            //AddRemodelRecipe(ItemID.SilverBar, ItemID.GoldBar, 10);
            //AddRemodelRecipe(ItemID.SilverBar, ItemID.PlatinumBar, 10);
            //AddRemodelRecipe(ItemID.TungstenBar, ItemID.GoldBar, 10);
            //AddRemodelRecipe(ItemID.TungstenBar, ItemID.PlatinumBar, 10);

            #endregion

            #region 金和铂金
            int cost = CalculateMagikeCost(Glistent, 6, 15);

            AddRemodelRecipe(ItemID.Marble, ItemID.GoldOre, cost, 4);
            AddRemodelRecipe(ItemID.Marble, ItemID.PlatinumOre, cost, 4);

            AddRemodelRecipe(ItemID.Marble, ItemID.Ruby, cost, mainStack: 18);

            AddRemodelRecipe(ItemID.GoldOre, ItemID.CrimtaneOre, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.DemoniteOre, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.Meteorite, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.GoldBar, cost, 2);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.PlatinumBar, cost, 2);

            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.CrimtaneOre, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.DemoniteOre, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.Meteorite, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.GoldBar, cost, 2);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.PlatinumBar, cost, 2);

            AddRemodelRecipe(ItemID.GoldBar, ItemID.CrimtaneBar, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.DemoniteBar, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.MeteoriteBar, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.AncientGoldHelmet, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.GoldChest, CalculateMagikeCost(Glistent, 6, 120), mainStack: 15);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.GoldenKey, CalculateMagikeCost(Bone, 6, 75), mainStack: 20, conditions: Condition.DownedSkeletron);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.CrimtaneBar, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.DemoniteBar, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.MeteoriteBar, cost, 2, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.AncientGoldHelmet, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.GoldChest, CalculateMagikeCost(Glistent, 6, 120), mainStack: 15);

            #endregion

            #region 猩红矿和魔矿
            cost = CalculateMagikeCost(Corruption, 6, 15);

            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.TissueSample, cost, mainStack: 3, conditions: Condition.DownedEowOrBoc);
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.ShadowScale, cost, mainStack: 3, conditions: Condition.DownedEowOrBoc);
            //AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.Hellstone, 15);
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.CrimtaneBar, cost, 2);
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.DemoniteBar, cost, 2);

            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.TissueSample, 15, mainStack: 3, conditions: Condition.DownedEowOrBoc);
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.ShadowScale, 15, mainStack: 3, conditions: Condition.DownedEowOrBoc);
            //AddRemodelRecipe(ItemID.DemoniteOre, ItemID.Hellstone, 15);
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.CrimtaneBar, cost, 2);
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.DemoniteBar, cost, 2);

            cost = CalculateMagikeCost(Corruption, 6, 120);

            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.AncientShadowHelmet, cost, mainStack: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.AncientShadowScalemail, cost, mainStack: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.AncientShadowGreaves, cost, mainStack: 10);
            //AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.HellstoneBar, 25);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.AncientShadowHelmet, cost, mainStack: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.AncientShadowScalemail, cost, mainStack: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.AncientShadowGreaves, cost, mainStack: 10);
            //AddRemodelRecipe(ItemID.DemoniteBar, ItemID.HellstoneBar, 25);
            #endregion

            #region 陨石矿
            cost = CalculateMagikeCost(Glistent, 6, 20);

            AddRemodelRecipe(ItemID.EbonstoneBlock, ItemID.Meteorite, cost, 4, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.CrimstoneBlock, ItemID.Meteorite, cost, 4, conditions: Condition.DownedEyeOfCthulhu);

            //AddRemodelRecipe(ItemID.Meteorite, ItemID.Hellstone, 15);
            //AddRemodelRecipe(ItemID.Meteorite, ItemID.MeteoriteBar, 20);
            AddRemodelRecipe(ItemID.Meteorite, ItemID.MeteoriteBar, cost, 2);

            //AddRemodelRecipe(ItemID.MeteoriteBar, ItemID.HellstoneBar, 25);
            #endregion

            #region 狱岩
            cost = CalculateMagikeCost(Hellstone, 6, 20);

            AddRemodelRecipe(ItemID.AshBlock, ItemID.Hellstone, cost, 4, conditions: Condition.DownedEowOrBoc);
            AddRemodelRecipe(ItemID.Hellstone, ItemID.HellstoneBar, cost);
            //AddRemodelRecipe(ItemID.Hellstone, ItemID.LivingFireBlock, 25, 3);

            //AddRemodelRecipe(ItemID.Hellstone, ItemID.CobaltOre, 50, conditions: Condition.Hardmode);
            //AddRemodelRecipe(ItemID.Hellstone, ItemID.PalladiumOre, 50, conditions: Condition.Hardmode);

            //AddRemodelRecipe(ItemID.HellstoneBar, ItemID.CobaltBar, 65, conditions: Condition.Hardmode);
            //AddRemodelRecipe(ItemID.HellstoneBar, ItemID.PalladiumBar, 65, conditions: Condition.Hardmode);
            #endregion

            #region 钴矿和钯金矿
            cost = CalculateMagikeCost(CrystallineMagike, 6, 20);

            AddRemodelRecipe(ItemID.CobaltOre, ItemID.MythrilOre, cost,3);
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.OrichalcumOre, cost, 3);
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.CobaltBar, cost, 2);
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.PalladiumBar, cost, 2);

            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.MythrilOre, cost, 3);
            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.OrichalcumOre, cost, 3);
            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.CobaltBar, cost, 2);
            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.PalladiumBar, cost, 2);

            AddRemodelRecipe(ItemID.CobaltBar, ItemID.MythrilBar, cost, 3);
            AddRemodelRecipe(ItemID.CobaltBar, ItemID.OrichalcumBar, cost, 3);
            AddRemodelRecipe(ItemID.PalladiumBar, ItemID.MythrilBar, cost, 3);
            AddRemodelRecipe(ItemID.PalladiumBar, ItemID.OrichalcumBar, cost, 3);
            #endregion

            #region 秘银矿和山铜矿
            cost = CalculateMagikeCost(Pelagic, 6, 20);

            AddRemodelRecipe(ItemID.MythrilOre, ItemID.TitaniumOre, cost, 3);
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.AdamantiteOre, cost, 3);
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.MythrilBar, cost, 2);
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.OrichalcumBar, cost, 2);

            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.TitaniumOre, cost, 3);
            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.AdamantiteOre, cost, 3);
            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.MythrilBar, cost, 2);
            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.OrichalcumBar, cost, 2);

            AddRemodelRecipe(ItemID.MythrilBar, ItemID.TitaniumBar, cost, 3);
            AddRemodelRecipe(ItemID.MythrilBar, ItemID.AdamantiteBar, cost, 3);
            AddRemodelRecipe(ItemID.OrichalcumBar, ItemID.TitaniumBar, cost, 3);
            AddRemodelRecipe(ItemID.OrichalcumBar, ItemID.AdamantiteBar, cost, 3);
            #endregion

            #region 钛金和精金
            cost = CalculateMagikeCost(Pelagic, 6, 25);

            AddRemodelRecipe(ItemID.TitaniumOre, ItemID.TitaniumBar, cost, 2);
            AddRemodelRecipe(ItemID.TitaniumOre, ItemID.AdamantiteBar, cost, 2);
            AddRemodelRecipe(ItemID.AdamantiteOre, ItemID.TitaniumBar, cost, 2);
            AddRemodelRecipe(ItemID.AdamantiteOre, ItemID.AdamantiteBar, cost, 2);

            //AddRemodelRecipe(ItemID.TitaniumBar, ItemID.HallowedBar, 180, conditions: Condition.DownedMechBossAny);
            //AddRemodelRecipe(ItemID.AdamantiteBar, ItemID.HallowedBar, 180, conditions: Condition.DownedMechBossAny);
            #endregion

            //神圣锭
            cost = CalculateMagikeCost(Hallow, 6, 20);
            AddRemodelRecipe(ItemID.SoulofMight, ItemID.HallowedBar, cost, conditions: Condition.DownedMechBossAny);
            AddRemodelRecipe(ItemID.SoulofSight, ItemID.HallowedBar, cost, conditions: Condition.DownedMechBossAny);
            AddRemodelRecipe(ItemID.SoulofFright, ItemID.HallowedBar, cost, conditions: Condition.DownedMechBossAny);

            cost = CalculateMagikeCost(Hallow, 6, 30);
            MagikeCraftRecipe.CreateRecipe(ItemID.HallowedBar, ItemID.ChlorophyteBar, cost, 3)
                .AddIngredient(ItemID.JungleGrassSeeds)
                .AddIngredient(ItemID.MudBlock, 8)
                .AddCondition(Condition.DownedMechBossAll)
                .Register();

            //叶绿矿
            cost = CalculateMagikeCost(Soul, 6, 30);

            MagikeCraftRecipe.CreateRecipe(ItemID.ChlorophyteBar, ItemID.ShroomiteBar, cost)
                .AddIngredient(ItemID.GlowingMushroom, 4)
                .Register();

            MagikeCraftRecipe.CreateRecipe(ItemID.ChlorophyteBar, ItemID.SpectreBar, cost)
                .AddIngredient(ItemID.Ectoplasm, 2)
                .Register();

            cost = CalculateMagikeCost(SplendorMagicore, 6, 30);

            AddRemodelRecipe(ItemID.ChlorophyteOre, ItemID.LunarOre, cost, conditions: Condition.DownedMoonLord);
            AddRemodelRecipe(ItemID.ChlorophyteBar, ItemID.LunarBar, cost, conditions: Condition.DownedMoonLord);
        }
    }
}
