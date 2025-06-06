﻿using Coralite.Content.Items.MagikeSeries1;
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

namespace Coralite.Content.Items.Magike.Refractors
{
    public class BasicPrism() : MagikeApparatusItem(TileType<BasicPrismTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeRefractors)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicPrismTile() : BasePrismTile
        (2, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns, 8)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public override int DropItemType => ItemType<BasicPrism>();

        public override MALevel[] GetAllLevels()
        {
            return [
                MALevel.None,
                MALevel.MagicCrystal,
                MALevel.Glistent,
                MALevel.Crimson,
                MALevel.Corruption,
                MALevel.Icicle,
                MALevel.CrystallineMagike,
                MALevel.Hallow,
                MALevel.HolyLight,
                MALevel.SplendorMagicore
                ];
        }
    }

    public class BasicPrismTileEntity : BaseSenderTileEntity<BasicPrismTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicPrismContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicPrismSender();
    }

    public class BasicPrismContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 240,
                MALevel.Glistent => 720,
                MALevel.Crimson
                or MALevel.Corruption
                or MALevel.Icicle => 1440,
                MALevel.CrystallineMagike => 5760,
                MALevel.Hallow => 7200,
                MALevel.HolyLight => 20480,
                MALevel.SplendorMagicore => 90000,
                _ => 0,
            };
            LimitMagikeAmount();

            //AntiMagikeMaxBase = MagikeMaxBase * 2;
            //LimitAntiMagikeAmount();
        }
    }

    public class BasicPrismSender : UpgradeableLinerSender
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MaxConnectBase = 2;

            switch (incomeLevel)
            {
                default:
                case MALevel.None:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = -1;
                    ConnectLengthBase = 0;
                    break;
                case MALevel.MagicCrystal:
                    UnitDeliveryBase = 18;
                    SendDelayBase = 60 * 3;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MALevel.Glistent:
                    MaxConnectBase = 3;
                    UnitDeliveryBase = 36;
                    SendDelayBase = 60 * 3;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MALevel.Crimson:
                case MALevel.Corruption:
                case MALevel.Icicle:
                    MaxConnectBase = 3;
                    UnitDeliveryBase = 72;
                    SendDelayBase = 60 * 3;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MALevel.CrystallineMagike:
                    MaxConnectBase = 4;
                    UnitDeliveryBase = 144;
                    SendDelayBase = 60 * 2;
                    ConnectLengthBase = 12 * 16;
                    break;
                case MALevel.Hallow:
                    MaxConnectBase = 4;
                    UnitDeliveryBase = 180;
                    SendDelayBase = 60 * 2;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MALevel.HolyLight:
                    MaxConnectBase = 4;
                    UnitDeliveryBase = 512;
                    SendDelayBase = 60 * 2;
                    ConnectLengthBase = 10 * 16;
                    break;
                case MALevel.SplendorMagicore:
                    MaxConnectBase = 4;
                    UnitDeliveryBase = 900;
                    SendDelayBase = 60 * 1;
                    ConnectLengthBase = 12 * 16;
                    break;
            }

            RecheckConnect();
        }
    }
}
