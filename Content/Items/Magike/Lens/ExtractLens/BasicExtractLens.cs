using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.ExtractLens
{
    public class BasicExtractLens() : MagikeApparatusItem(TileType<BasicExtractLensTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicExtractLensTile() : BaseLensTile
        ( Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override int DropItemType => ItemType<BasicExtractLens>();

        public override MagikeTileEntity GetEntityInstance() => GetInstance<BasicExtractLensTileEntity>();

        public override MALevel[] GetAllLevels()
        {
            return [
                MALevel.None,
                MALevel.MagicCrystal,
                MALevel.Crimson,
                MALevel.Corruption,
                MALevel.Icicle,
                MALevel.CrystallineMagike,
                MALevel.Soul,
                MALevel.Feather,
                MALevel.SplendorMagicore
                ];
        }
    }

    public class BasicExtractLensTileEntity : BaseCostItemProducerTileEntity<BasicExtractLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicExtractLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicExtractLensSender();

        public override MagikeCostItemProducer GetStartProducer()
            => new BasicExtractProducer();

        public override ItemContainer GetStartItemContainer()
            => new()
            {
                CapacityBase = 1
            };
    }

    public class BasicExtractLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 120,
                MALevel.Crimson
                or MALevel.Corruption
                or MALevel.Icicle => 600,
                MALevel.CrystallineMagike => 1800,
                MALevel.Soul
                or MALevel.Feather => 7500,
                MALevel.SplendorMagicore => 18000,
                _ => 0,
            };
            LimitMagikeAmount();

            AntiMagikeMaxBase = MagikeMaxBase * 2;
            LimitAntiMagikeAmount();
        }
    }

    public class BasicExtractLensSender : UpgradeableLinerSender
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MaxConnectBase = 1;
            ConnectLengthBase = 4 * 16;
            switch (incomeLevel)
            {
                default:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = 1_0000_0000/60;//随便填个大数
                    ConnectLengthBase = 0;
                    break;
                case MALevel.MagicCrystal:
                    UnitDeliveryBase = 10;
                    SendDelayBase =   5;
                    break;
                case MALevel.Crimson:
                case MALevel.Corruption:
                case MALevel.Icicle:
                    UnitDeliveryBase = 50;
                    SendDelayBase =  5;
                    break;
                case MALevel.CrystallineMagike:
                    UnitDeliveryBase = 120;
                    SendDelayBase =   4;
                    break;
                case MALevel.Soul:
                case MALevel.Feather:
                    UnitDeliveryBase = 500;
                    SendDelayBase =   4;
                    break;
                case MALevel.SplendorMagicore:
                    UnitDeliveryBase = 900;
                    SendDelayBase =   3;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class BasicExtractProducer : UpgradeableExtractProducer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            ProductionDelayBase = incomeLevel switch
            {
                MALevel.MagicCrystal
                or MALevel.Crimson
                or MALevel.Corruption
                or MALevel.Icicle
                or MALevel.CrystallineMagike
                or MALevel.Soul
                or MALevel.Feather
                or MALevel.SplendorMagicore => 10,
                _ => 1_0000_0000 / 60,//随便填个大数
            } * 60;

            Timer = ProductionDelayBase;
        }
    }
}
