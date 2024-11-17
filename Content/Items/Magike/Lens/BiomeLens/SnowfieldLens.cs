﻿using Coralite.Content.Items.MagikeSeries1;
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
    public class SnowfieldLens() : MagikeApparatusItem(TileType<SnowfieldLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneSnow;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.IceBlock, 5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SnowfieldLensTile() : BaseLensTile
        (Coralite.IcicleCyan, DustID.Frost)
    {
        public override int DropItemType => ItemType<SnowfieldLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.SnowBlock,TileID.SnowBrick,TileID.IceBlock,TileID.IceBrick
            ];
        }

        public override MagikeTP GetEntityInstance() => GetInstance<SnowfieldLensTileEntity>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Icicle,
                MALevel.Frost,
            ];
        }
    }

    public class SnowfieldLensTileEntity : BaseActiveProducerTileEntity<SnowfieldLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new SnowfieldLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new SnowfieldLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new SnowfieldProducer();
    }

    public class SnowfieldLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Icicle:
                    MagikeMaxBase = 140;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.Frost:
                    MagikeMaxBase = 675;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class SnowfieldLensSender : UpgradeableLinerSender
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
                case MALevel.Icicle:
                    UnitDeliveryBase = 42;
                    SendDelayBase = 9;
                    break;
                case MALevel.Frost:
                    UnitDeliveryBase = 180;
                    SendDelayBase = 8;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class SnowfieldProducer : UpgradeableProducerByBiome
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.SnowfieldLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.SnowfieldCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType is TileID.SnowBlock or TileID.SnowBrick or TileID.IceBrick or TileID.IceBlock;

        public override bool CheckWall(Tile tile)
            => tile.WallType is WallID.SnowBrick or WallID.SnowWallEcho or WallID.SnowWallUnsafe;

        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 1;
                    break;
                case MALevel.Icicle:
                    ProductionDelayBase = 9;
                    ThroughputBase = 14;
                    break;
                case MALevel.Frost:
                    ProductionDelayBase = 8;
                    ThroughputBase = 65;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
