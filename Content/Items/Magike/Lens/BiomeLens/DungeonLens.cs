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
    public class DungeonLens() : MagikeApparatusItem(TileType<DungeonLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneDungeon;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.BlueBrick, 10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.GreenBrick, 10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.PinkBrick, 10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DungeonLensTile() : BaseLensTile
        (Color.DimGray, DustID.Bone)
    {
        public override int DropItemType => ItemType<DungeonLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick,
                TileID.AncientBlueBrick, TileID.AncientGreenBrick, TileID.AncientPinkBrick,
            ];
        }

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                BoneLevel.ID,
                SoulLevel.ID,
            ];
        }
    }

    public class DungeonLensTileEntity : BaseActiveProducerTileEntity<DungeonLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new DungeonLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new DungeonLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new DungeonProducer();
    }

    public class DungeonLensContainer : UpgradeableContainer<DungeonLensTile>
    {
        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    switch (incomeLevel)
        //    {
        //        default:
        //            MagikeMaxBase = 0;
        //            //AntiMagikeMaxBase = 0;
        //            break;
        //        case MALevel.Bone:
        //            MagikeMaxBase = 100;
        //            //AntiMagikeMaxBase = MagikeMaxBase * 3;
        //            break;
        //        case MALevel.Soul:
        //            MagikeMaxBase = 757;
        //            //AntiMagikeMaxBase = MagikeMaxBase * 2;
        //            break;
        //    }

        //    LimitMagikeAmount();
        //    //LimitAntiMagikeAmount();
        //}
    }

    public class DungeonLensSender : UpgradeableLinerSender<DungeonLensTile>
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
        //        case MALevel.Bone:
        //            UnitDeliveryBase = 45;
        //            SendDelayBase = 4 * 60 + 30;
        //            break;
        //        case MALevel.Soul:
        //            UnitDeliveryBase = 265;
        //            SendDelayBase = 3 * 60 + 30;
        //            break;
        //    }

        //    RecheckConnect();
        //}
    }

    public class DungeonProducer : UpgradeableProducerByBiome<DungeonLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.DungeonLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.DungeonCondition;

        public override bool CheckTile(Tile tile)
            => TileID.Sets.DungeonBiome[tile.TileType] > 0;

        public override bool CheckWall(Tile tile)
            => tile.WallType is WallID.BlueDungeon or WallID.BlueDungeonUnsafe or WallID.BlueDungeonSlab or WallID.BlueDungeonSlabUnsafe or WallID.BlueDungeonTile or WallID.BlueDungeonTileUnsafe
            or WallID.GreenDungeon or WallID.GreenDungeonUnsafe or WallID.GreenDungeonSlab or WallID.GreenDungeonSlabUnsafe or WallID.GreenDungeonTile or WallID.GreenDungeonTileUnsafe
            or WallID.PinkDungeon or WallID.PinkDungeonUnsafe or WallID.PinkDungeonSlab or WallID.PinkDungeonSlabUnsafe or WallID.PinkDungeonTile or WallID.PinkDungeonTileUnsafe;

        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    switch (incomeLevel)
        //    {
        //        default:
        //            ProductionDelayBase = -1;
        //            ThroughputBase = 0;
        //            break;
        //        case MALevel.Bone:
        //            ProductionDelayBase = 4 * 60 + 30;
        //            ThroughputBase = 8;
        //            break;
        //        case MALevel.Soul:
        //            ProductionDelayBase = 3 * 60 + 30;
        //            ThroughputBase = 53;
        //            break;
        //    }

        //    Timer = ProductionDelayBase;
        //}
    }
}
