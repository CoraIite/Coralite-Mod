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
    public class OceanLens() : MagikeApparatusItem(TileType<OceanLensTile>(), Item.sellPrice(silver: 5)
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
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class OceanLensTile() : BaseLensTile
        (Color.SandyBrown, DustID.Sand)
    {
        public override int DropItemType => ItemType<OceanLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Sand
            ];
        }

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Seashore,
                MALevel.Pelagic,
            ];
        }
    }

    public class OceanLensTileEntity : BaseActiveProducerTileEntity<OceanLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new OceanLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new OceanLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new OceanProducer();
    }

    public class OceanLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    //AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Seashore:
                    MagikeMaxBase = 10;
                    //AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.Pelagic:
                    MagikeMaxBase = 300;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            //LimitAntiMagikeAmount();
        }
    }

    public class OceanLensSender : UpgradeableLinerSender
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
                case MALevel.Seashore:
                    UnitDeliveryBase = 5;
                    SendDelayBase = 5;
                    break;
                case MALevel.Pelagic:
                    UnitDeliveryBase = 120;
                    SendDelayBase = 4;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class OceanProducer : UpgradeableProducerByBiome
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.OceanLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.OceanCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType == TileID.Sand;

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
                case MALevel.Seashore:
                    ProductionDelayBase = 5;
                    ThroughputBase = 1;
                    break;
                case MALevel.Pelagic:
                    ProductionDelayBase = 4;
                    ThroughputBase = 24;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
