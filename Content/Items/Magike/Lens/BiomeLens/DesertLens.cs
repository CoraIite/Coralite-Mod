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
using Terraria.GameContent.Events;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.BiomeLens
{
    public class DesertLens() : MagikeApparatusItem(TileType<DesertLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneDesert;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.SandBlock, 5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DesertLensTile() : BaseLensTile
        (Color.SandyBrown, DustID.Sand)
    {
        public override int DropItemType => ItemType<DesertLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Sand,TileID.Sandstone,TileID.SandstoneBrick
            ];
        }

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Quicksand,
                MALevel.Forbidden,
            ];
        }
    }

    public class DesertLensTileEntity : BaseActiveProducerTileEntity<DesertLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new DesertLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new DesertLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new DesertProducer();
    }

    public class DesertLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    //AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Quicksand:
                    MagikeMaxBase = 230;
                    //AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.Forbidden:
                    MagikeMaxBase = 730;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            //LimitAntiMagikeAmount();
        }
    }

    public class DesertLensSender : UpgradeableLinerSender
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
                    SendDelayBase = -1;
                    ConnectLengthBase = 0;
                    break;
                case MALevel.Quicksand:
                    UnitDeliveryBase = 69;
                    SendDelayBase = 9;
                    break;
                case MALevel.Forbidden:
                    UnitDeliveryBase = 195;
                    SendDelayBase = 8;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class DesertProducer : UpgradeableProducerByBiome
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.DesertLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.DesertCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType is TileID.Sand or TileID.Sandstone or TileID.SandstoneBrick;

        public override int Throughput => base.Throughput + (Sandstorm.Happening ? 5 : 0);

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
                case MALevel.Quicksand:
                    ProductionDelayBase = 9;
                    ThroughputBase = 23;
                    break;
                case MALevel.Forbidden:
                    ProductionDelayBase = 8;
                    ThroughputBase = 65;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
