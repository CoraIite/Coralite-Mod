﻿using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class BasicRefractor() : MagikeApparatusItem(TileType<BasicRefractorTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeRefractors)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicRefractorTile() : BaseRefractorTile
        (1, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public override int DropItemType => ItemType<BasicRefractor>();

        public override MagikeTileEntity GetEntityInstance() => GetInstance<BasicRefractorTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.MagicCrystal,
                MagikeApparatusLevel.Crimson,
                MagikeApparatusLevel.Corruption,
                MagikeApparatusLevel.Icicle,
                MagikeApparatusLevel.CrystallineMagike,
                MagikeApparatusLevel.Soul,
                MagikeApparatusLevel.Feather,
                MagikeApparatusLevel.SplendorMagicore
                ];
        }
    }

    public class BasicRefractorTileEntity : BaseSenderTileEntity<BasicRefractorTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicRefractorContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicRefractorSender();
    }

    public class BasicRefractorContainer : UpgradeableContainer
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MagikeApparatusLevel.MagicCrystal => 60,
                MagikeApparatusLevel.Crimson 
                or MagikeApparatusLevel.Corruption 
                or MagikeApparatusLevel.Icicle => 300,
                MagikeApparatusLevel.CrystallineMagike => 1800,
                MagikeApparatusLevel.Soul 
                or MagikeApparatusLevel.Feather => 7500,
                MagikeApparatusLevel.SplendorMagicore => 18000,
                _ => 0,
            };
            LimitMagikeAmount();
        }
    }

    public class BasicRefractorSender : UpgradeableLinerSender
    {
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            MaxConnectBase = 1;

            switch (incomeLevel)
            {
                default:
                case MagikeApparatusLevel.None:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = 1_0000_0000;//随便填个大数
                    ConnectLengthBase = 0;
                    break;
                case MagikeApparatusLevel.MagicCrystal:
                    UnitDeliveryBase = 10;
                    SendDelayBase = 60 * 5;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.Crimson:
                case MagikeApparatusLevel.Corruption:
                case MagikeApparatusLevel.Icicle:
                    UnitDeliveryBase = 50;
                    SendDelayBase = 60 * 5;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MagikeApparatusLevel.CrystallineMagike:
                    UnitDeliveryBase = 120;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MagikeApparatusLevel.Soul:
                case MagikeApparatusLevel.Feather:
                    UnitDeliveryBase = 500;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
                case MagikeApparatusLevel.SplendorMagicore:
                    UnitDeliveryBase = 900;
                    SendDelayBase = 60 * 4;
                    ConnectLengthBase = 15 * 16;
                    break;
            }

            RecheckConnect();
        }
    }
}
