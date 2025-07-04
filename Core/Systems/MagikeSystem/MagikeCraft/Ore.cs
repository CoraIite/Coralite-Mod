﻿using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class Ore : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //木头
            AddRemodelRecipe(ItemID.Wood, ItemID.Coal, CalculateMagikeCost(MagicCrystal, 4, 15), mainStack: 4);
            AddRemodelRecipe(ItemID.AshWood, ItemID.Coal, CalculateMagikeCost(MagicCrystal, 4, 15), mainStack: 4);
            AddRemodelRecipe(ItemID.RichMahogany, ItemID.Coal, CalculateMagikeCost(MagicCrystal, 4, 15), mainStack: 4);
            AddRemodelRecipe(ItemID.Ebonwood, ItemID.Coal, CalculateMagikeCost(MagicCrystal, 4, 15), mainStack: 4);
            AddRemodelRecipe(ItemID.Shadewood, ItemID.Coal, CalculateMagikeCost(MagicCrystal, 4, 15), mainStack: 4);
            AddRemodelRecipe(ItemID.Pearlwood, ItemID.Coal, CalculateMagikeCost(MagicCrystal, 4, 15), mainStack: 4);
            AddRemodelRecipe(ItemID.BorealWood, ItemID.Coal, CalculateMagikeCost(MagicCrystal, 4, 15), mainStack: 4);
            AddRemodelRecipe(ItemID.PalmWood, ItemID.Coal, CalculateMagikeCost(MagicCrystal, 4, 15), mainStack: 4);

            //煤
            AddRemodelRecipe(ItemID.Coal, ItemID.Diamond, CalculateMagikeCost(RedJade, 4), mainStack: 4);

            //沙漠化石
            AddRemodelRecipe(ItemID.DesertFossil, ItemID.Amber, CalculateMagikeCost(MagicCrystal, 4, 15), mainStack: 10);

            //黄玉
            AddRemodelRecipe(ItemID.Topaz, ItemID.GemSquirrelTopaz, CalculateMagikeCost(MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Topaz, ItemID.GemBunnyTopaz, CalculateMagikeCost(MagicCrystal, 5, 15));

            //紫晶
            AddRemodelRecipe(ItemID.Amethyst, ItemID.GemSquirrelAmethyst, CalculateMagikeCost(MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Amethyst, ItemID.GemBunnyAmethyst, CalculateMagikeCost(MagicCrystal, 5, 15));

            //蓝宝石
            AddRemodelRecipe(ItemID.Sapphire, ItemID.GemSquirrelSapphire, CalculateMagikeCost(MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Sapphire, ItemID.GemBunnySapphire, CalculateMagikeCost(MagicCrystal, 5, 15));

            //绿宝石
            AddRemodelRecipe(ItemID.Emerald, ItemID.GemSquirrelEmerald, CalculateMagikeCost(MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Emerald, ItemID.GemBunnyEmerald, CalculateMagikeCost(MagicCrystal, 5, 15));

            //红宝石
            AddRemodelRecipe(ItemID.Ruby, ItemID.GemSquirrelRuby, CalculateMagikeCost(MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Ruby, ItemID.GemBunnyRuby, CalculateMagikeCost(MagicCrystal, 5, 15));

            //钻石
            AddRemodelRecipe(ItemID.Diamond, ItemID.GemSquirrelDiamond, CalculateMagikeCost(MagicCrystal, 5, 15));
            AddRemodelRecipe(ItemID.Diamond, ItemID.GemBunnyDiamond, CalculateMagikeCost(MagicCrystal, 5, 15));

            //琥珀
            AddRemodelRecipe(ItemID.Amber, ItemID.AmberMosquito, CalculateMagikeCost(MagicCrystal, 12, 120), mainStack: 50);

            //石头变泥土，虽然微光转化也可以做到，但是使用魔能的话可以翻倍
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.DirtBlock, CalculateMagikeCost(MagicCrystal, 4, 5), resultStack: 2);

            #region 铜和锡
            //均为2矿变一个锭，使用泥土就可以变成铜和锡
            int copperMagikeCost = CalculateMagikeCost(MagicCrystal, 2, 5);
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.CopperOre, copperMagikeCost, 3);
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.TinOre, copperMagikeCost, 3);

            //AddRemodelRecipe(ItemID.CopperOre, ItemID.IronOre, 1);
            //AddRemodelRecipe(ItemID.CopperOre, ItemID.LeadOre, 1);
            AddRemodelRecipe(ItemID.CopperOre, ItemID.CopperBar, copperMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.CopperOre, ItemID.TinBar, copperMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            //AddRemodelRecipe(ItemID.TinOre, ItemID.IronOre, 1);
            //AddRemodelRecipe(ItemID.TinOre, ItemID.LeadOre, 1);
            AddRemodelRecipe(ItemID.TinOre, ItemID.CopperBar, copperMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.TinOre, ItemID.TinBar, copperMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            //AddRemodelRecipe(ItemID.CopperBar, ItemID.IronBar, 2);
            //AddRemodelRecipe(ItemID.CopperBar, ItemID.LeadBar, 2);
            //AddRemodelRecipe(ItemID.TinBar, ItemID.IronBar, 2);
            //AddRemodelRecipe(ItemID.TinBar, ItemID.LeadBar, 2);

            #endregion

            #region 铁和铅
            int ironMagikeCost = CalculateMagikeCost(RedJade, 3, 10);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.IronOre, ironMagikeCost, 3);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.TinOre, ironMagikeCost, 3);

            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Amethyst, CalculateMagikeCost(RedJade, 4, 10), mainStack: 8);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Topaz, CalculateMagikeCost(RedJade, 4, 10), mainStack: 8);

            //AddRemodelRecipe(ItemID.IronOre, ItemID.SilverOre, 3);
            //AddRemodelRecipe(ItemID.IronOre, ItemID.TungstenOre, 3);
            AddRemodelRecipe(ItemID.IronOre, ItemID.IronBar, ironMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.IronOre, ItemID.LeadBar, ironMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            //AddRemodelRecipe(ItemID.LeadOre, ItemID.SilverOre, 3);
            //AddRemodelRecipe(ItemID.LeadOre, ItemID.TungstenOre, 3);
            AddRemodelRecipe(ItemID.LeadOre, ItemID.IronBar, ironMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.LeadOre, ItemID.LeadBar, ironMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            //AddRemodelRecipe(ItemID.IronBar, ItemID.SilverBar, 6);
            //AddRemodelRecipe(ItemID.IronBar, ItemID.TungstenBar, 6);
            AddRemodelRecipe(ItemID.IronBar, ItemID.AncientIronHelmet, CalculateMagikeCost(RedJade, 6, 120), mainStack: 10);
            //AddRemodelRecipe(ItemID.LeadBar, ItemID.SilverBar, 6);
            //AddRemodelRecipe(ItemID.LeadBar, ItemID.TungstenBar, 6);
            AddRemodelRecipe(ItemID.LeadBar, ItemID.AncientIronHelmet, CalculateMagikeCost(RedJade, 6, 120), mainStack: 10);

            #endregion

            #region 银和钨
            int silverMagikeCost = CalculateMagikeCost(RedJade, 4, 10);
            AddRemodelRecipe(ItemID.Granite, ItemID.SilverOre, silverMagikeCost, 3);
            AddRemodelRecipe(ItemID.Granite, ItemID.TungstenOre, silverMagikeCost, 3);

            AddRemodelRecipe(ItemID.Granite, ItemID.Sapphire, silverMagikeCost, mainStack: 8);
            AddRemodelRecipe(ItemID.Granite, ItemID.Emerald, silverMagikeCost, mainStack: 8);

            //AddRemodelRecipe(ItemID.SilverOre, ItemID.GoldOre, 5);
            //AddRemodelRecipe(ItemID.SilverOre, ItemID.PlatinumOre, 5);
            AddRemodelRecipe(ItemID.SilverOre, ItemID.SilverBar, silverMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.SilverOre, ItemID.TungstenBar, silverMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            //AddRemodelRecipe(ItemID.TungstenOre, ItemID.GoldOre, 5);
            //AddRemodelRecipe(ItemID.TungstenOre, ItemID.PlatinumOre, 5);
            AddRemodelRecipe(ItemID.TungstenOre, ItemID.SilverBar, silverMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.TungstenOre, ItemID.TungstenBar, silverMagikeCost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            //AddRemodelRecipe(ItemID.SilverBar, ItemID.GoldBar, 10);
            //AddRemodelRecipe(ItemID.SilverBar, ItemID.PlatinumBar, 10);
            //AddRemodelRecipe(ItemID.TungstenBar, ItemID.GoldBar, 10);
            //AddRemodelRecipe(ItemID.TungstenBar, ItemID.PlatinumBar, 10);

            #endregion

            #region 金和铂金
            int cost = CalculateMagikeCost(Glistent, 4, 10);

            AddRemodelRecipe(ItemID.Marble, ItemID.GoldOre, cost, 4);
            AddRemodelRecipe(ItemID.Marble, ItemID.PlatinumOre, cost, 4);

            AddRemodelRecipe(ItemID.Marble, ItemID.Ruby, cost, mainStack: 8);

            AddRemodelRecipe(ItemID.GoldOre, ItemID.CrimtaneOre, cost, 2, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.DemoniteOre, cost, 2, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.Meteorite, cost, 12, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.GoldBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.PlatinumBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.CrimtaneOre, cost, 2, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.DemoniteOre, cost, 2, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.Meteorite, cost, 1, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.GoldBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.PlatinumBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            AddRemodelRecipe(ItemID.GoldBar, ItemID.CrimtaneBar, cost, 1, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.DemoniteBar, cost, 1, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.MeteoriteBar, cost, 1, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.AncientGoldHelmet, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.GoldChest, CalculateMagikeCost(Glistent, 6, 120), mainStack: 8, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.GoldenKey, CalculateMagikeCost(Bone, 6, 75), mainStack: 12, conditions: Condition.DownedSkeletron);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.CrimtaneBar, cost, 2, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.DemoniteBar, cost, 2, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.MeteoriteBar, cost, 1, conditions: [Condition.DownedEyeOfCthulhu, CoraliteConditions.NotInDigDigDig]);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.AncientGoldHelmet, 150, mainStack: 8);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.GoldChest, CalculateMagikeCost(Glistent, 6, 120), mainStack: 12, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.GoldenKey, CalculateMagikeCost(Bone, 6, 75), mainStack: 12, conditions: Condition.DownedSkeletron);
            #endregion

            #region 猩红矿和魔矿
            cost = CalculateMagikeCost(Corruption, 4, 15);

            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.TissueSample, cost, mainStack: 2, conditions: Condition.DownedEowOrBoc);
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.ShadowScale, cost, mainStack: 2, conditions: Condition.DownedEowOrBoc);
            //AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.Hellstone, 15);
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.CrimtaneBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.DemoniteBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.TissueSample, 15, mainStack: 2, conditions: Condition.DownedEowOrBoc);
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.ShadowScale, 15, mainStack: 2, conditions: Condition.DownedEowOrBoc);
            //AddRemodelRecipe(ItemID.DemoniteOre, ItemID.Hellstone, 15);
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.CrimtaneBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.DemoniteBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            cost = CalculateMagikeCost(Corruption, 6, 120);

            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.AncientShadowHelmet, cost, mainStack: 15);
            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.AncientShadowScalemail, cost, mainStack: 15);
            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.AncientShadowGreaves, cost, mainStack: 15);
            //AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.HellstoneBar, 25);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.AncientShadowHelmet, cost, mainStack: 15);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.AncientShadowScalemail, cost, mainStack: 15);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.AncientShadowGreaves, cost, mainStack: 15);
            //AddRemodelRecipe(ItemID.DemoniteBar, ItemID.HellstoneBar, 25);
            #endregion

            #region 陨石矿
            cost = CalculateMagikeCost(Glistent, 4, 15);

            AddRemodelRecipe(ItemID.EbonstoneBlock, ItemID.Meteorite, cost, 3, conditions: Condition.DownedEyeOfCthulhu);
            AddRemodelRecipe(ItemID.CrimstoneBlock, ItemID.Meteorite, cost, 3, conditions: Condition.DownedEyeOfCthulhu);

            //AddRemodelRecipe(ItemID.Meteorite, ItemID.Hellstone, 15);
            //AddRemodelRecipe(ItemID.Meteorite, ItemID.MeteoriteBar, 20);
            AddRemodelRecipe(ItemID.Meteorite, ItemID.MeteoriteBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            //AddRemodelRecipe(ItemID.MeteoriteBar, ItemID.HellstoneBar, 25);
            #endregion

            #region 狱岩
            cost = CalculateMagikeCost(Hellstone, 4, 15);

            AddRemodelRecipe(ItemID.AshBlock, ItemID.Hellstone, cost, 3, conditions: Condition.DownedEowOrBoc);
            AddRemodelRecipe(ItemID.Hellstone, ItemID.HellstoneBar, cost, conditions: CoraliteConditions.NotInDigDigDig);
            //AddRemodelRecipe(ItemID.Hellstone, ItemID.LivingFireBlock, 25, 3);

            //AddRemodelRecipe(ItemID.Hellstone, ItemID.CobaltOre, 50, conditions: Condition.Hardmode);
            //AddRemodelRecipe(ItemID.Hellstone, ItemID.PalladiumOre, 50, conditions: Condition.Hardmode);

            //AddRemodelRecipe(ItemID.HellstoneBar, ItemID.CobaltBar, 65, conditions: Condition.Hardmode);
            //AddRemodelRecipe(ItemID.HellstoneBar, ItemID.PalladiumBar, 65, conditions: Condition.Hardmode);
            #endregion

            #region 钴矿和钯金矿
            cost = CalculateMagikeCost(CrystallineMagike, 6, 15);

            AddRemodelRecipe(ItemID.PearlstoneBlock, ItemID.CobaltOre, cost, 3, conditions: Condition.Hardmode);
            AddRemodelRecipe(ItemID.PearlstoneBlock, ItemID.PalladiumOre, cost, 3, conditions: Condition.Hardmode);

            AddRemodelRecipe(ItemID.CobaltOre, ItemID.MythrilOre, cost, 2);
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.OrichalcumOre, cost, 2);
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.CobaltBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.PalladiumBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.MythrilOre, cost, 2);
            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.OrichalcumOre, cost, 2);
            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.CobaltBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.PalladiumBar, cost, 1, conditions: CoraliteConditions.NotInDigDigDig);

            AddRemodelRecipe(ItemID.CobaltBar, ItemID.MythrilBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.CobaltBar, ItemID.OrichalcumBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.PalladiumBar, ItemID.MythrilBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.PalladiumBar, ItemID.OrichalcumBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);
            #endregion

            #region 秘银矿和山铜矿
            cost = CalculateMagikeCost(Pelagic, 6, 15);

            AddRemodelRecipe(ItemID.MythrilOre, ItemID.TitaniumOre, cost, 3);
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.AdamantiteOre, cost, 3);
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.MythrilBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.OrichalcumBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);

            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.TitaniumOre, cost, 3);
            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.AdamantiteOre, cost, 3);
            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.MythrilBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.OrichalcumBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);

            AddRemodelRecipe(ItemID.MythrilBar, ItemID.TitaniumBar, cost, 3, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.MythrilBar, ItemID.AdamantiteBar, cost, 3, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.OrichalcumBar, ItemID.TitaniumBar, cost, 3, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.OrichalcumBar, ItemID.AdamantiteBar, cost, 3, conditions: CoraliteConditions.NotInDigDigDig);
            #endregion

            #region 钛金和精金
            cost = CalculateMagikeCost(Pelagic, 6, 15);

            AddRemodelRecipe(ItemID.TitaniumOre, ItemID.TitaniumBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.TitaniumOre, ItemID.AdamantiteBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.AdamantiteOre, ItemID.TitaniumBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.AdamantiteOre, ItemID.AdamantiteBar, cost, 2, conditions: CoraliteConditions.NotInDigDigDig);

            //AddRemodelRecipe(ItemID.TitaniumBar, ItemID.HallowedBar, 180, conditions: Condition.DownedMechBossAny);
            //AddRemodelRecipe(ItemID.AdamantiteBar, ItemID.HallowedBar, 180, conditions: Condition.DownedMechBossAny);
            #endregion

            //神圣锭
            cost = CalculateMagikeCost(Hallow, 6, 15);
            AddRemodelRecipe(ItemID.SoulofMight, ItemID.HallowedBar, cost, conditions: Condition.DownedMechBossAny);
            AddRemodelRecipe(ItemID.SoulofSight, ItemID.HallowedBar, cost, conditions: Condition.DownedMechBossAny);
            AddRemodelRecipe(ItemID.SoulofFright, ItemID.HallowedBar, cost, conditions: Condition.DownedMechBossAny);

            AddRemodelRecipe(ItemID.TitaniumBar, ItemID.HallowedBar, cost, 2, conditions: Condition.DownedMechBossAny);
            AddRemodelRecipe(ItemID.AdamantiteBar, ItemID.HallowedBar, cost, 2, conditions: Condition.DownedMechBossAny);

            cost = CalculateMagikeCost(Hallow, 6, 15);
            MagikeRecipe.CreateCraftRecipe(ItemID.HallowedBar, ItemID.ChlorophyteBar, cost, 2)
                .AddIngredient(ItemID.JungleGrassSeeds)
                .AddIngredient(ItemID.MudBlock, 2)
                .AddCondition(Condition.DownedMechBossAll)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            //叶绿矿
            cost = CalculateMagikeCost(Soul, 6, 15);

            MagikeRecipe.CreateCraftRecipe(ItemID.ChlorophyteOre, ItemID.ShroomiteBar, cost, 2)
                .AddIngredient(ItemID.GlowingMushroom, 2)
                .AddCondition(Condition.DownedPlantera)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.ChlorophyteOre, ItemID.SpectreBar, cost, 2)
                .AddIngredient(ItemID.Ectoplasm, 1)
                .AddCondition(Condition.DownedPlantera)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            cost = CalculateMagikeCost(SplendorMagicore, 6, 15);

            //四柱碎片


            AddRemodelRecipe(ItemID.ChlorophyteOre, ItemID.LunarOre, cost, 2, conditions: Condition.DownedMoonLord);
            AddRemodelRecipe(ItemID.LunarOre, ItemID.LunarBar, cost, 2, conditions: Condition.DownedMoonLord);
            AddRemodelRecipe(ItemID.ChlorophyteBar, ItemID.LunarBar, cost, 2, conditions: Condition.DownedMoonLord);
        }
    }
}
