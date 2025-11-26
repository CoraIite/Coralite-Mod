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

namespace Coralite.Content.Items.Magike.Lens.BiomeLens
{
    public class SkyLens() : MagikeApparatusItem(TileType<SkyLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneSkyHeight;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.Cloud, 5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SkyLensTile() : BaseLensTile
        (Color.SandyBrown, DustID.Cloud)
    {
        public override int DropItemType => ItemType<SkyLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Cloud,TileID.RainCloud,TileID.SnowCloud
            ];
        }

                public override List<ushort> GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Eiderdown,
                MALevel.Flight,
                MALevel.Feather,
            ];
        }
    }

    public class SkyLensTileEntity : BaseActiveProducerTileEntity<SkyLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new SkyLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new SkyLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new SkyProducer();
    }

    public class SkyLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    //AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Eiderdown:
                    MagikeMaxBase = 30;
                    //AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.Flight:
                    MagikeMaxBase = 412;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
                case MALevel.Feather:
                    MagikeMaxBase = 714;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            //LimitAntiMagikeAmount();
        }
    }

    public class SkyLensSender : UpgradeableLinerSender
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MaxConnectBase = 1;
            ConnectLengthBase = 6 * 16;

            switch (incomeLevel)
            {
                default:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = -1;
                    ConnectLengthBase = 0;
                    break;
                case MALevel.Eiderdown:
                    UnitDeliveryBase = 15;
                    SendDelayBase = 5 * 60;
                    break;
                case MALevel.Flight:
                    UnitDeliveryBase = 165;
                    SendDelayBase = 4 * 60;
                    break;
                case MALevel.Feather:
                    UnitDeliveryBase = 250;
                    SendDelayBase = 3 * 60 + 30;
                    break;
            }

            RecheckConnect();
        }
    }

    public class SkyProducer : UpgradeableProducerByBiome
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.SkyLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.SkyCondition;

        public override bool CheckTile(Tile tile)
            => TileID.Sets.Clouds[tile.TileType];

        public override bool CheckWall(Tile tile)
            => true;

        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = -1;
                    ThroughputBase = 0;
                    break;
                case MALevel.Eiderdown:
                    ProductionDelayBase = 5 * 60;
                    ThroughputBase = 3;
                    break;
                case MALevel.Flight:
                    ProductionDelayBase = 4 * 60;
                    ThroughputBase = 33;
                    break;
                case MALevel.Feather:
                    ProductionDelayBase = 3 * 60 + 30;
                    ThroughputBase = 50;
                    break;
            }

            Timer = ProductionDelayBase;
        }
    }
}
