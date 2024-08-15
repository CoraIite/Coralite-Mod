using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.BiomeLens
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
                .AddIngredient(ItemID.Wood, 10)
                .AddIngredient(ItemID.GrassSeeds)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ForestLensTile() : BaseLensTile
        (2, 3, Color.Green, DustID.Grass, 8)
    {
        public override string Texture => AssetDirectory.MagikeLensTiles + Name;
        public override int DropItemType => ItemType<ForestLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Grass, TileID.HallowedGrass, TileID.GolfGrass, TileID.GolfGrassHallowed
            ];
        }

        public override MagikeTileEntity GetEntityInstance() => GetInstance<ForestLensTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return
            [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.Glistent,
                MagikeApparatusLevel.CrystallineMagike,
                MagikeApparatusLevel.SplendorMagicore
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
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MagikeApparatusLevel.Glistent => 45,
                MagikeApparatusLevel.CrystallineMagike => 250,
                MagikeApparatusLevel.SplendorMagicore => 1125,
                _ => 0,
            };
            LimitMagikeAmount();

            AntiMagikeMaxBase = MagikeMaxBase * 2;
            LimitAntiMagikeAmount();
        }
    }

    public class ForestLensSender : UpgradeableLinerSender
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
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
                case MagikeApparatusLevel.Glistent:
                    UnitDeliveryBase = 15;
                    SendDelayBase = 10;
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    UnitDeliveryBase = 75;
                    SendDelayBase = 9;
                    break;
                case MagikeApparatusLevel.SplendorMagicore:
                    UnitDeliveryBase = 300;
                    SendDelayBase = 8;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class ForestProducer : UpgradeableBiomeProducer
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.ForestLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.ForestCondition;

        public override bool CheckTile(Tile tile)
            => TileID.Sets.Grass[tile.TileType];

        public override bool CheckWall(Tile tile)
            => tile.WallType is WallID.Grass or WallID.GrassUnsafe or WallID.Flower or WallID.FlowerUnsafe;

        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 5;
                    break;
                case MagikeApparatusLevel.Glistent:
                    ProductionDelayBase = 10;
                    ThroughputBase = 5;
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    ProductionDelayBase = 9;
                    ThroughputBase = 25;
                    break;
                case MagikeApparatusLevel.SplendorMagicore:
                    ProductionDelayBase = 8;
                    ThroughputBase = 100;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
