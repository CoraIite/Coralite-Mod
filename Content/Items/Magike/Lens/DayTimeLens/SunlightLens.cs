using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.DayTimeLens
{
    public class SunlightLens() : MagikeApparatusItem(TileType<SunlightLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.Sunflower)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SunlightLensTile() : BaseLensTile
        (Color.SaddleBrown, DustID.HallowedTorch)
    {
        public override int DropItemType => ItemType<SunlightLens>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                GlistentLevel.ID,
                HallowLevel.ID,
                HolyLightLevel.ID,
            ];
        }
    }

    public class SunlightLensTileEntity : BaseActiveProducerTileEntity<SunlightLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new SunlightLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new SunlightLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new SunlightProducer();
    }

    public class SunlightLensContainer : UpgradeableContainer<SunlightLensTile>
    {
        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    switch (incomeLevel)
        //    {
        //        default:
        //            MagikeMaxBase = 0;
        //            //AntiMagikeMaxBase = 0;
        //            break;
        //        case MALevel.Glistent:
        //            MagikeMaxBase = 100;
        //            //AntiMagikeMaxBase = MagikeMaxBase * 3;
        //            break;
        //        case MALevel.Hallow:
        //            MagikeMaxBase = 1000;
        //            //AntiMagikeMaxBase = MagikeMaxBase * 2;
        //            break;
        //        case MALevel.HolyLight:
        //            MagikeMaxBase = 1928;
        //            //AntiMagikeMaxBase = MagikeMaxBase * 2;
        //            break;
        //    }

        //    LimitMagikeAmount();
        //    //LimitAntiMagikeAmount();
        //}
    }

    public class SunlightLensSender : UpgradeableLinerSender<SunlightLensTile>
    {
    }

    public class SunlightProducer : UpgradeableProducerByTime<SunlightLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.SunlightLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.SunlightCondition;

        public override bool CheckDayTime()
        {
            return Main.dayTime && !Main.raining && !Main.eclipse;
        }
    }
}
