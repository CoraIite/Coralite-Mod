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
    public class GlowingMushroomLens() : MagikeApparatusItem(TileType<GlowingMushroomLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneGlowshroom;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.GlowingMushroom, 10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GlowingMushroomLensTile() : BaseLensTile
        (Color.DarkBlue, DustID.GlowingMushroom)
    {
        public override int DropItemType => ItemType<GlowingMushroomLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.MushroomGrass
            ];
        }

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                EmperorLevel.ID,
                ShroomiteLevel.ID,
            ];
        }
    }

    public class GlowingMushroomLensTileEntity : BaseActiveProducerTileEntity<GlowingMushroomLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new GlowingMushroomLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new GlowingMushroomLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new GlowingMushroomProducer();
    }

    public class GlowingMushroomLensContainer : UpgradeableContainer<GlowingMushroomLensTile>
    {
        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    switch (incomeLevel)
        //    {
        //        default:
        //            MagikeMaxBase = 0;
        //            //AntiMagikeMaxBase = 0;
        //            break;
        //        case MALevel.Emperor:
        //            MagikeMaxBase = 78;
        //            //AntiMagikeMaxBase = MagikeMaxBase * 3;
        //            break;
        //        case MALevel.Shroomite:
        //            MagikeMaxBase = 714;
        //            //AntiMagikeMaxBase = MagikeMaxBase * 2;
        //            break;
        //    }

        //    LimitMagikeAmount();
        //    //LimitAntiMagikeAmount();
        //}
    }

    public class GlowingMushroomLensSender : UpgradeableLinerSender<GlowingMushroomLensTile>
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
        //        case MALevel.Emperor:
        //            UnitDeliveryBase = 35;
        //            SendDelayBase = 4 * 60 + 30;
        //            break;
        //        case MALevel.Shroomite:
        //            UnitDeliveryBase = 250;
        //            SendDelayBase = 3 * 60 + 30;
        //            break;
        //    }

        //    RecheckConnect();
        //}
    }

    public class GlowingMushroomProducer : UpgradeableProducerByBiome<GlowingMushroomLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.GlowingMushroomLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.GlowingMushroomCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType == TileID.MushroomGrass;

        public override bool CheckWall(Tile tile)
            => true;

        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    switch (incomeLevel)
        //    {
        //        default:
        //            ProductionDelayBase = -1;
        //            ThroughputBase = 0;
        //            break;
        //        case MALevel.Emperor:
        //            ProductionDelayBase = 4 * 60 + 30;
        //            ThroughputBase = 7;
        //            break;
        //        case MALevel.Shroomite:
        //            ProductionDelayBase = 3 * 60 + 30;
        //            ThroughputBase = 25;
        //            break;
        //    }

        //    Timer = ProductionDelayBase;
        //}
    }
}
