using Coralite.Content.Items.Glistent;
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
    public class ForestLens() : MagikeApparatusItem(TileType<ForestLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneForest;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient<GlistentBar>(3)
                .AddIngredient(ItemID.GrassSeeds)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ForestLensTile() : BaseLensTile
        ( Color.Green, DustID.Grass)
    {
        public override int DropItemType => ItemType<ForestLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Grass, TileID.HallowedGrass, TileID.GolfGrass, TileID.GolfGrassHallowed
            ];
        }

        public override MagikeTileEntity GetEntityInstance() => GetInstance<ForestLensTileEntity>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Glistent,
                MALevel.CrystallineMagike,
                MALevel.SplendorMagicore
            ];
        }
    }

    public class ForestLensTileEntity : BaseActiveProducerTileEntity<ForestLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new ForestLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new ForestLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new ForestProducer();
    }

    public class ForestLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Glistent:
                    MagikeMaxBase = 90;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.CrystallineMagike:
                    MagikeMaxBase = 483;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
                case MALevel.SplendorMagicore:
                    MagikeMaxBase = 4500;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class ForestLensSender : UpgradeableLinerSender
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
                    SendDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ConnectLengthBase = 0;
                    break;
                case MALevel.Glistent:
                    UnitDeliveryBase = 10;
                    SendDelayBase = 30;
                    break;
                case MALevel.CrystallineMagike:
                    UnitDeliveryBase = 129;
                    SendDelayBase = 8;
                    break;
                case MALevel.SplendorMagicore:
                    UnitDeliveryBase = 900;
                    SendDelayBase = 6;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class ForestProducer : UpgradeableProducerByBiome
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.ForestLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.ForestCondition;

        public override bool CheckTile(Tile tile)
            => TileID.Sets.Grass[tile.TileType];

        public override bool CheckWall(Tile tile)
            => tile.WallType is WallID.Grass or WallID.GrassUnsafe or WallID.Flower or WallID.FlowerUnsafe;

        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 1;
                    break;
                case MALevel.Glistent:
                    ProductionDelayBase = 10;
                    ThroughputBase = 10;
                    break;
                case MALevel.CrystallineMagike:
                    ProductionDelayBase = 8;
                    ThroughputBase = 43;
                    break;
                case MALevel.SplendorMagicore:
                    ProductionDelayBase = 6;
                    ThroughputBase = 300;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
