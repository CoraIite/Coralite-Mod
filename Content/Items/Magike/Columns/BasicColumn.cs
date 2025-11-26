using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Columns
{
    public class BasicColumn() : MagikeApparatusItem(TileType<BasicColumnTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeColumns)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(20)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicColumnTile() : BaseColumnTile
        (3, 3, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeColumnTiles + Name;
        public override int DropItemType => ItemType<BasicColumn>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                CrystalLevel.ID,
                CrimsonLevel.ID,
                CorruptionLevel.ID,
                IcicleLevel.ID,
                BrilliantLevel.ID,
                SoulLevel.ID,
                FeatherLevel.ID,
                SplendorLevel.ID,
            ];
        }
    }

    public class BasicColumnTileEntity : BaseSenderTileEntity<BasicColumnTile>
    {
        public override int MainComponentID => MagikeComponentID.MagikeContainer;

        public override MagikeContainer GetStartContainer()
            => new BasicColumnTileContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicColumnTileSender();
    }

    public class BasicColumnTileContainer : UpgradeableContainer<BaseColumnTile>
    {
        public override void Upgrade(ushort incomeLevel)
        {
            string name = this.GetDataPreName();
            MagikeMaxBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(MagikeMaxBase));

            //MagikeMaxBase = incomeLevel switch
            //{
            //    MALevel.MagicCrystal => 2700,
            //    MALevel.Crimson
            //    or MALevel.Corruption
            //    or MALevel.Icicle => 12000,
            //    MALevel.CrystallineMagike => 43200,
            //    MALevel.Soul
            //    or MALevel.Feather => 12_0000,
            //    MALevel.SplendorMagicore => 54_0000,
            //    _ => 0,
            //};
            LimitMagikeAmount();

            //AntiMagikeMaxBase = MagikeMaxBase / 2;
            //LimitAntiMagikeAmount();
        }
    }

    public class BasicColumnTileSender : UpgradeableLinerSender<BaseColumnTile>
    {
        //public override void Upgrade(ushort incomeLevel)
        //{
        //    string name = this.GetDataPreName();
        //    MaxConnectBase = MagikeSystem.GetLevelDataByte(incomeLevel, name + nameof(MaxConnectBase));
        //    UnitDeliveryBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(UnitDeliveryBase));
        //    SendDelayBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(SendDelayBase));
        //    ConnectLengthBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(ConnectLengthBase));

            //switch (incomeLevel)
            //{
            //    default:
            //    case MALevel.None:
            //        break;
            //    case MALevel.MagicCrystal:
            //        UnitDeliveryBase = 180;
            //        break;
            //    case MALevel.Crimson:
            //    case MALevel.Corruption:
            //    case MALevel.Icicle:
            //        UnitDeliveryBase = 600;
            //        break;
            //    case MALevel.CrystallineMagike:
            //        UnitDeliveryBase = 1440;
            //        break;
            //    case MALevel.Soul:
            //    case MALevel.Feather:
            //        UnitDeliveryBase = 2400;
            //        break;
            //    case MALevel.SplendorMagicore:
            //        UnitDeliveryBase = 6000;
            //        break;
            //}

        //    RecheckConnect();
        //}
    }
}
