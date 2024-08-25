using Coralite.Content.Items.MagikeSeries1;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class Ore : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //木头
            AddRemodelRecipe(ItemID.Wood, ItemID.Coal, 5, mainStack: 20);
            AddRemodelRecipe(ItemID.AshWood, ItemID.Coal, 5, mainStack: 20);
            AddRemodelRecipe(ItemID.RichMahogany, ItemID.Coal, 5, mainStack: 20);
            AddRemodelRecipe(ItemID.Ebonwood, ItemID.Coal, 5, mainStack: 20);
            AddRemodelRecipe(ItemID.Shadewood, ItemID.Coal, 5, mainStack: 20);
            AddRemodelRecipe(ItemID.Pearlwood, ItemID.Coal, 5, mainStack: 20);
            AddRemodelRecipe(ItemID.BorealWood, ItemID.Coal, 5, mainStack: 20);
            AddRemodelRecipe(ItemID.PalmWood, ItemID.Coal, 5, mainStack: 20);

            //煤
            AddRemodelRecipe(ItemID.Coal, ItemID.Diamond, 150, mainStack: 20);

            //沙漠化石
            AddRemodelRecipe(ItemID.DesertFossil, ItemID.Amber, 1, mainStack: 10);

            //黄玉
            AddRemodelRecipe(ItemID.Topaz, ItemID.GemSquirrelTopaz, 25);
            AddRemodelRecipe(ItemID.Topaz, ItemID.GemBunnyTopaz, 25);

            //紫晶
            AddRemodelRecipe(ItemID.Amethyst, ItemID.GemSquirrelAmethyst, 25);
            AddRemodelRecipe(ItemID.Amethyst, ItemID.GemBunnyAmethyst, 25);

            //蓝宝石
            AddRemodelRecipe(ItemID.Sapphire, ItemID.GemSquirrelSapphire, 25);
            AddRemodelRecipe(ItemID.Sapphire, ItemID.GemBunnySapphire, 25);

            //绿宝石
            AddRemodelRecipe(ItemID.Emerald, ItemID.GemSquirrelEmerald, 25);
            AddRemodelRecipe(ItemID.Emerald, ItemID.GemBunnyEmerald, 25);

            //红宝石
            AddRemodelRecipe(ItemID.Ruby, ItemID.GemSquirrelRuby, 25);
            AddRemodelRecipe(ItemID.Ruby, ItemID.GemBunnyRuby, 25);

            //钻石
            AddRemodelRecipe(ItemID.Diamond, ItemID.GemSquirrelDiamond, 25);
            AddRemodelRecipe(ItemID.Diamond, ItemID.GemBunnyDiamond, 25);

            //石头
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.CopperOre, 1, mainStack: 20);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.TinOre, 1, mainStack: 20);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Amethyst, 50, mainStack: 100);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Topaz, 50, mainStack: 100);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Sapphire, 100, mainStack: 200);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Emerald, 100, mainStack: 200);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Ruby, 200, mainStack: 300);

            //石头变泥土，虽然微光转化也可以做到，但是使用魔能的话可以翻倍
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.DirtBlock, 10, 2);
            //石头变各种乱七八糟的石头
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Granite, 10, mainStack: 2);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.Marble, 10, mainStack: 2);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.AshBlock, 10, mainStack: 2);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.MudBlock, 10, mainStack: 2);
            AddRemodelRecipe(ItemID.StoneBlock, ItemID.SandBlock, 10, mainStack: 2);
            AddRemodelRecipe(ItemID.StoneBlock, ItemType<Basalt>(), 15, mainStack: 4);

            //铜和锡
            AddRemodelRecipe(ItemID.CopperOre, ItemID.IronOre, 1);
            AddRemodelRecipe(ItemID.CopperOre, ItemID.LeadOre, 1);
            AddRemodelRecipe(ItemID.CopperOre, ItemID.CopperBar, 2);
            AddRemodelRecipe(ItemID.CopperOre, ItemID.TinBar, 2);

            AddRemodelRecipe(ItemID.TinOre, ItemID.IronOre, 1);
            AddRemodelRecipe(ItemID.TinOre, ItemID.LeadOre, 1);
            AddRemodelRecipe(ItemID.TinOre, ItemID.CopperBar, 2);
            AddRemodelRecipe(ItemID.TinOre, ItemID.TinBar, 2);

            AddRemodelRecipe(ItemID.CopperBar, ItemID.IronBar, 2);
            AddRemodelRecipe(ItemID.CopperBar, ItemID.LeadBar, 2);
            AddRemodelRecipe(ItemID.TinBar, ItemID.IronBar, 2);
            AddRemodelRecipe(ItemID.TinBar, ItemID.LeadBar, 2);

            //铁和铅
            AddRemodelRecipe(ItemID.IronOre, ItemID.SilverOre, 3);
            AddRemodelRecipe(ItemID.IronOre, ItemID.TungstenOre, 3);
            AddRemodelRecipe(ItemID.IronOre, ItemID.IronBar, 6);
            AddRemodelRecipe(ItemID.IronOre, ItemID.LeadBar, 6);

            AddRemodelRecipe(ItemID.LeadOre, ItemID.SilverOre, 3);
            AddRemodelRecipe(ItemID.LeadOre, ItemID.TungstenOre, 3);
            AddRemodelRecipe(ItemID.LeadOre, ItemID.IronBar, 6);
            AddRemodelRecipe(ItemID.LeadOre, ItemID.LeadBar, 6);

            AddRemodelRecipe(ItemID.IronBar, ItemID.SilverBar, 6);
            AddRemodelRecipe(ItemID.IronBar, ItemID.TungstenBar, 6);
            AddRemodelRecipe(ItemID.IronBar, ItemID.AncientIronHelmet, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.LeadBar, ItemID.SilverBar, 6);
            AddRemodelRecipe(ItemID.LeadBar, ItemID.TungstenBar, 6);
            AddRemodelRecipe(ItemID.LeadBar, ItemID.AncientIronHelmet, 150, mainStack: 10);

            //银和钨
            AddRemodelRecipe(ItemID.SilverOre, ItemID.GoldOre, 5);
            AddRemodelRecipe(ItemID.SilverOre, ItemID.PlatinumOre, 5);
            AddRemodelRecipe(ItemID.SilverOre, ItemID.SilverBar, 10);
            AddRemodelRecipe(ItemID.SilverOre, ItemID.TungstenBar, 10);

            AddRemodelRecipe(ItemID.TungstenOre, ItemID.GoldOre, 5);
            AddRemodelRecipe(ItemID.TungstenOre, ItemID.PlatinumOre, 5);
            AddRemodelRecipe(ItemID.TungstenOre, ItemID.SilverBar, 10);
            AddRemodelRecipe(ItemID.TungstenOre, ItemID.TungstenBar, 10);

            AddRemodelRecipe(ItemID.SilverBar, ItemID.GoldBar, 10);
            AddRemodelRecipe(ItemID.SilverBar, ItemID.PlatinumBar, 10);
            AddRemodelRecipe(ItemID.TungstenBar, ItemID.GoldBar, 10);
            AddRemodelRecipe(ItemID.TungstenBar, ItemID.PlatinumBar, 10);

            //金和铂金
            AddRemodelRecipe(ItemID.GoldOre, ItemID.CrimtaneOre, 10);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.DemoniteOre, 10);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.Meteorite, 10);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.GoldBar, 15);
            AddRemodelRecipe(ItemID.GoldOre, ItemID.PlatinumBar, 15);

            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.CrimtaneOre, 10);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.DemoniteOre, 10);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.Meteorite, 10);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.GoldBar, 15);
            AddRemodelRecipe(ItemID.PlatinumOre, ItemID.PlatinumBar, 15);

            AddRemodelRecipe(ItemID.GoldBar, ItemID.CrimtaneBar, 15);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.DemoniteBar, 15);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.MeteoriteBar, 15);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.AncientGoldHelmet, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.GoldChest, 100, mainStack: 15);
            AddRemodelRecipe(ItemID.GoldBar, ItemID.GoldenKey, 400, mainStack: 20, conditions:Condition.DownedSkeletron);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.CrimtaneBar, 15);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.DemoniteBar, 15);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.MeteoriteBar, 15);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.AncientGoldHelmet, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.PlatinumBar, ItemID.GoldChest, 100, mainStack: 15);

            //猩红矿和魔矿
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.TissueSample, 15, mainStack: 3, conditions: Condition.DownedEowOrBoc);
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.ShadowScale, 15, mainStack: 3, conditions: Condition.DownedEowOrBoc );
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.Hellstone, 15);
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.CrimtaneBar, 20);
            AddRemodelRecipe(ItemID.CrimtaneOre, ItemID.DemoniteBar, 20);

            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.TissueSample, 15, mainStack: 3, conditions: Condition.DownedEowOrBoc );
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.ShadowScale, 15, mainStack: 3, conditions: Condition.DownedEowOrBoc);
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.Hellstone, 15);
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.CrimtaneBar, 20);
            AddRemodelRecipe(ItemID.DemoniteOre, ItemID.DemoniteBar, 20);

            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.AncientShadowHelmet, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.AncientShadowScalemail, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.AncientShadowGreaves, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, ItemID.HellstoneBar, 25);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.AncientShadowHelmet, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.AncientShadowScalemail, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.AncientShadowGreaves, 150, mainStack: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, ItemID.HellstoneBar, 25);

            //陨石矿
            AddRemodelRecipe(ItemID.Meteorite, ItemID.Hellstone, 15);
            AddRemodelRecipe(ItemID.Meteorite, ItemID.MeteoriteBar, 20);

            AddRemodelRecipe(ItemID.MeteoriteBar, ItemID.HellstoneBar, 25);

            //狱岩
            AddRemodelRecipe(ItemID.Hellstone, ItemID.HellstoneBar, 25);
            AddRemodelRecipe(ItemID.Hellstone, ItemID.LivingFireBlock, 25, 3);

            AddRemodelRecipe(ItemID.Hellstone, ItemID.CobaltOre, 50, conditions: Condition.Hardmode);
            AddRemodelRecipe(ItemID.Hellstone, ItemID.PalladiumOre, 50, conditions: Condition.Hardmode);

            AddRemodelRecipe(ItemID.HellstoneBar, ItemID.CobaltBar, 65, conditions: Condition.Hardmode);
            AddRemodelRecipe(ItemID.HellstoneBar, ItemID.PalladiumBar, 65, conditions: Condition.Hardmode);

            //钴矿和钯金矿
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.MythrilOre, 70);
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.OrichalcumOre, 70);
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.CobaltBar, 60);
            AddRemodelRecipe(ItemID.CobaltOre, ItemID.PalladiumBar, 60);

            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.MythrilOre, 70);
            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.OrichalcumOre, 70);
            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.CobaltBar, 60);
            AddRemodelRecipe(ItemID.PalladiumOre, ItemID.PalladiumBar, 60);

            AddRemodelRecipe(ItemID.CobaltBar, ItemID.MythrilBar, 100);
            AddRemodelRecipe(ItemID.CobaltBar, ItemID.OrichalcumBar, 100);
            AddRemodelRecipe(ItemID.PalladiumBar, ItemID.MythrilBar, 100);
            AddRemodelRecipe(ItemID.PalladiumBar, ItemID.OrichalcumBar, 100);

            //秘银矿和山铜矿
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.TitaniumOre, 100);
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.AdamantiteOre, 100);
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.MythrilBar, 80);
            AddRemodelRecipe(ItemID.MythrilOre, ItemID.OrichalcumBar, 80);

            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.TitaniumOre, 100);
            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.AdamantiteOre, 100);
            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.MythrilBar, 80);
            AddRemodelRecipe(ItemID.OrichalcumOre, ItemID.OrichalcumBar, 80);

            AddRemodelRecipe(ItemID.MythrilBar, ItemID.TitaniumBar, 150);
            AddRemodelRecipe(ItemID.MythrilBar, ItemID.AdamantiteBar, 150);
            AddRemodelRecipe(ItemID.OrichalcumBar, ItemID.TitaniumBar, 150);
            AddRemodelRecipe(ItemID.OrichalcumBar, ItemID.AdamantiteBar, 150);

            //钛金和精金
            AddRemodelRecipe(ItemID.TitaniumOre, ItemID.TitaniumBar, 120);
            AddRemodelRecipe(ItemID.TitaniumOre, ItemID.AdamantiteBar, 120);
            AddRemodelRecipe(ItemID.AdamantiteOre, ItemID.TitaniumBar, 120);
            AddRemodelRecipe(ItemID.AdamantiteOre, ItemID.AdamantiteBar, 120);

            AddRemodelRecipe(ItemID.TitaniumBar, ItemID.HallowedBar, 180, conditions: Condition.DownedMechBossAny);
            AddRemodelRecipe(ItemID.AdamantiteBar, ItemID.HallowedBar, 180, conditions: Condition.DownedMechBossAny);

            //神圣锭
            AddRemodelRecipe(ItemID.HallowedBar, ItemID.ChlorophyteBar, 230, conditions: Condition.DownedMechBossAll );

            //叶绿矿
            AddRemodelRecipe(ItemID.ChlorophyteOre, ItemID.LunarOre, 500, conditions: Condition.DownedMoonLord);
            AddRemodelRecipe(ItemID.ChlorophyteBar, ItemID.LunarBar, 500, conditions: Condition.DownedMoonLord);
        }
    }
}
