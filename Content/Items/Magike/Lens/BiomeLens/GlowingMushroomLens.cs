using Coralite.Content.Items.MagikeSeries1;
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
        (2, 3, Color.DarkBlue, DustID.GlowingMushroom)
    {
        public override string Texture => AssetDirectory.MagikeLensTiles + Name;
        public override int DropItemType => ItemType<GlowingMushroomLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.MushroomGrass
            ];
        }

        public override MagikeTileEntity GetEntityInstance() => GetInstance<GlowingMushroomLensTileEntity>();

        public override MagikeApparatusLevel[] GetAllLevels()
        {
            return
            [
                MagikeApparatusLevel.None,
                MagikeApparatusLevel.Emperor,
                MagikeApparatusLevel.Shroomite,
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
        public override void Upgrade(MagikeApparatusLevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    AntiMagikeMaxBase = 0;
                    break;
                case MagikeApparatusLevel.Emperor:
                    MagikeMaxBase = 9;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MagikeApparatusLevel.Shroomite:
                    MagikeMaxBase = 562;
                    AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class GlowingMushroomLensSender : UpgradeableLinerSender
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
                case MagikeApparatusLevel.Emperor:
                    UnitDeliveryBase = 3;
                    SendDelayBase = 10;
                    break;
                case MagikeApparatusLevel.Shroomite:
                    UnitDeliveryBase = 150;
                    SendDelayBase = 8;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class GlowingMushroomProducer : UpgradeableBiomeProducer
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.GlowingMushroomLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.GlowingMushroomCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType == TileID.MushroomGrass;

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
                case MagikeApparatusLevel.Emperor:
                    ProductionDelayBase = 10;
                    ThroughputBase = 1;
                    break;
                case MagikeApparatusLevel.Shroomite:
                    ProductionDelayBase = 8;
                    ThroughputBase = 50;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
