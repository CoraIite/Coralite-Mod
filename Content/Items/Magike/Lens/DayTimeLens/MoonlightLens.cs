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
using Terraria.Enums;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.DayTimeLens
{
    public class MoonlightLens() : MagikeApparatusItem(TileType<MoonlightLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class MoonlightLensTile() : BaseLensTile
        (Color.Purple, DustID.PurpleTorch)
    {
        public override int DropItemType => ItemType<MoonlightLens>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                CrystalLevel.ID,
                BloodJadeLevel.ID,
            ];
        }
    }

    public class MoonlightLensTileEntity : BaseActiveProducerTileEntity<MoonlightLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new MoonlightLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new MoonlightLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new MoonlightProducer();
    }

    public class MoonlightLensContainer : UpgradeableContainer<MoonlightLensTile>
    {
    }

    public class MoonlightLensSender : UpgradeableLinerSender<MoonlightLensTile>
    {
    }

    public class MoonlightProducer : UpgradeableProducerByTime<MoonlightLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.MoonlightLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.MoonlightCondition;

        public override int Throughput
        {
            get
            {
                int i = base.Throughput;
                if (Main.bloodMoon)
                    i = (int)(i * 2f);
                if (Main.moonType == (int)MoonPhase.Full)
                    i = (int)(i * 2f);

                return i;
            }
        }

        public override bool CheckDayTime()
        {
            return !Main.dayTime && !Main.raining && Main.moonType != (int)MoonPhase.Empty;
        }
    }
}