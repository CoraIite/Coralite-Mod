using Coralite.Core.Systems.MagikeSystem.RemodelConditions;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class VanillaOre : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
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
            AddRemodelRecipe(ItemID.IronBar, 150, ItemID.AncientIronHelmet,selfRequiredNumber:10);
            AddRemodelRecipe(ItemID.LeadBar, 6, ItemID.SilverBar);
            AddRemodelRecipe(ItemID.LeadBar, 6, ItemID.TungstenBar);
            AddRemodelRecipe(ItemID.LeadBar, 150, ItemID.AncientIronHelmet, selfRequiredNumber: 10);

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
            AddRemodelRecipe(ItemID.GoldBar, 150, ItemID.AncientGoldHelmet,selfRequiredNumber:10);
            AddRemodelRecipe(ItemID.PlatinumBar, 15, ItemID.CrimtaneBar);
            AddRemodelRecipe(ItemID.PlatinumBar, 15, ItemID.DemoniteBar);
            AddRemodelRecipe(ItemID.PlatinumBar, 15, ItemID.MeteoriteBar);
            AddRemodelRecipe(ItemID.PlatinumBar, 150, ItemID.AncientGoldHelmet, selfRequiredNumber: 10);

            //猩红矿和魔矿
            AddRemodelRecipe(ItemID.CrimtaneOre, 15, ItemID.TissueSample, selfRequiredNumber: 3, condition: DownedEvilBossCondition.Instance);
            AddRemodelRecipe(ItemID.CrimtaneOre, 15, ItemID.ShadowScale, selfRequiredNumber: 3, condition: DownedEvilBossCondition.Instance);
            AddRemodelRecipe(ItemID.CrimtaneOre, 15, ItemID.Hellstone);
            AddRemodelRecipe(ItemID.CrimtaneOre, 20, ItemID.CrimtaneBar);
            AddRemodelRecipe(ItemID.CrimtaneOre, 20, ItemID.DemoniteBar);

            AddRemodelRecipe(ItemID.DemoniteOre, 15, ItemID.TissueSample, selfRequiredNumber: 3, condition: DownedEvilBossCondition.Instance);
            AddRemodelRecipe(ItemID.DemoniteOre, 15, ItemID.ShadowScale, selfRequiredNumber: 3, condition: DownedEvilBossCondition.Instance);
            AddRemodelRecipe(ItemID.DemoniteOre, 15, ItemID.Hellstone);
            AddRemodelRecipe(ItemID.DemoniteOre, 20, ItemID.CrimtaneBar);
            AddRemodelRecipe(ItemID.DemoniteOre, 20, ItemID.DemoniteBar);

            AddRemodelRecipe(ItemID.CrimtaneBar, 150, ItemID.AncientShadowHelmet, selfRequiredNumber: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, 150, ItemID.AncientShadowScalemail, selfRequiredNumber: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, 150, ItemID.AncientShadowGreaves, selfRequiredNumber: 10);
            AddRemodelRecipe(ItemID.CrimtaneBar, 25, ItemID.HellstoneBar);
            AddRemodelRecipe(ItemID.DemoniteBar, 150, ItemID.AncientShadowHelmet, selfRequiredNumber: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, 150, ItemID.AncientShadowScalemail, selfRequiredNumber: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, 150, ItemID.AncientShadowGreaves, selfRequiredNumber: 10);
            AddRemodelRecipe(ItemID.DemoniteBar, 25, ItemID.HellstoneBar);

            //陨石矿
            AddRemodelRecipe(ItemID.Meteorite, 15, ItemID.Hellstone);
            AddRemodelRecipe(ItemID.Meteorite, 20, ItemID.MeteoriteBar);

            AddRemodelRecipe(ItemID.MeteoriteBar, 25, ItemID.HellstoneBar);

            //狱岩
            AddRemodelRecipe(ItemID.Hellstone, 25, ItemID.HellstoneBar);
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
        }
    }
}
