using Coralite.Content.Items.MagikeSeries1;
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

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Emperor,
                MALevel.Shroomite,
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

    public class GlowingMushroomLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    //AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Emperor:
                    MagikeMaxBase = 140;
                    //AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.Shroomite:
                    MagikeMaxBase = 1285;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            //LimitAntiMagikeAmount();
        }
    }

    public class GlowingMushroomLensSender : UpgradeableLinerSender
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
                    SendDelayBase = -1;
                    ConnectLengthBase = 0;
                    break;
                case MALevel.Emperor:
                    UnitDeliveryBase = 42;
                    SendDelayBase = 9;
                    break;
                case MALevel.Shroomite:
                    UnitDeliveryBase = 300;
                    SendDelayBase = 7;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class GlowingMushroomProducer : UpgradeableProducerByBiome
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.GlowingMushroomLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.GlowingMushroomCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType == TileID.MushroomGrass;

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
                case MALevel.Emperor:
                    ProductionDelayBase = 9;
                    ThroughputBase = 14;
                    break;
                case MALevel.Shroomite:
                    ProductionDelayBase = 7;
                    ThroughputBase = 100;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
