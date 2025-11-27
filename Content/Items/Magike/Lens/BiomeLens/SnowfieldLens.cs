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

                public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                IcicleLevel.ID,
                FrostLevel.ID,
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

    public class SnowfieldLensContainer : UpgradeableContainer<SnowfieldLensTile>
    {
        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    switch (incomeLevel)
        //    {
        //        default:
        //            MagikeMaxBase = 0;
        //            //AntiMagikeMaxBase = 0;
        //            break;
        //        case MALevel.Icicle:
        //            MagikeMaxBase = 78;
        //            //AntiMagikeMaxBase = MagikeMaxBase * 3;
        //            break;
        //        case MALevel.Frost:
        //            MagikeMaxBase = 300;
        //            //AntiMagikeMaxBase = MagikeMaxBase * 2;
        //            break;
        //    }

        //    LimitMagikeAmount();
        //    //LimitAntiMagikeAmount();
        //}
    }

    public class SnowfieldLensSender : UpgradeableLinerSender<SnowfieldLensTile>
    {
        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    MaxConnectBase = 1;
        //    ConnectLengthBase = 6 * 16;

        //    switch (incomeLevel)
        //    {
        //        default:
        //            MaxConnectBase = 0;
        //            UnitDeliveryBase = 0;
        //            SendDelayBase = -1;
        //            ConnectLengthBase = 0;
        //            break;
        //        case MALevel.Icicle:
        //            UnitDeliveryBase = 35;
        //            SendDelayBase = 4 * 60 + 30;
        //            break;
        //        case MALevel.Frost:
        //            UnitDeliveryBase = 120;
        //            SendDelayBase = 4 * 60;
        //            break;
        //    }

        //    RecheckConnect();
        //}
    }

    public class SnowfieldProducer : UpgradeableProducerByBiome<SnowfieldLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.SnowfieldLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.SnowfieldCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType is TileID.SnowBlock or TileID.SnowBrick or TileID.IceBrick or TileID.IceBlock;

        public override bool CheckWall(Tile tile)
            => tile.WallType is WallID.SnowBrick or WallID.SnowWallEcho or WallID.SnowWallUnsafe
                             or WallID.IceBrick or WallID.IceEcho or WallID.IceUnsafe;

        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    switch (incomeLevel)
        //    {
        //        default:
        //            ProductionDelayBase = -1;
        //            ThroughputBase = 0;
        //            break;
        //        case MALevel.Icicle:
        //            ProductionDelayBase = 4 * 60 + 30;
        //            ThroughputBase = 7;
        //            break;
        //        case MALevel.Frost:
        //            ProductionDelayBase = 4 * 60;
        //            ThroughputBase = 24;
        //            break;
        //    }

        //    Timer = ProductionDelayBase;
        //}
    }
}
