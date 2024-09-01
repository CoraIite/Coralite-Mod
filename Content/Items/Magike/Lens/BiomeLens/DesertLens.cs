using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
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
            return player.ZoneBeach;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.SandBlock, 5)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DesertLensTile() : BaseLensTile
        (2, 3, Color.SandyBrown, DustID.Sand)
    {
        public override string Texture => AssetDirectory.MagikeLensTiles + Name;
        public override int DropItemType => ItemType<DesertLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Sand,TileID.Sandstone,TileID.SandstoneBrick
            ];
        }

        public override MagikeTileEntity GetEntityInstance() => GetInstance<DesertLensTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return
            [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.Quicksand,
                MagikeApparatusLevel.Forbidden,
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
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    AntiMagikeMaxBase = 0;
                    break;
                case MagikeApparatusLevel.Quicksand:
                    MagikeMaxBase = 9;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MagikeApparatusLevel.Forbidden:
                    MagikeMaxBase = 562;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class DesertLensSender : UpgradeableLinerSender
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
                case MagikeApparatusLevel.Quicksand:
                    UnitDeliveryBase = 3;
                    SendDelayBase = 10;
                    break;
                case MagikeApparatusLevel.Forbidden:
                    UnitDeliveryBase = 150;
                    SendDelayBase = 8;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class DesertProducer : UpgradeableBiomeProducer
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

        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 1;
                    break;
                case MagikeApparatusLevel.Quicksand:
                    ProductionDelayBase = 10;
                    ThroughputBase = 1;
                    break;
                case MagikeApparatusLevel.Forbidden:
                    ProductionDelayBase = 8;
                    ThroughputBase = 50;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
