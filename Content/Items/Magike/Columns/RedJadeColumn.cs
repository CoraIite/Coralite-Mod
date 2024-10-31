using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.RedJades;
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
    public class RedJadeColumn() : MagikeApparatusItem(TileType<RedJadeColumnTile>(), Item.sellPrice(silver: 5)
            , ItemRarityID.Blue, AssetDirectory.MagikeColumns)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(20)
                .AddIngredient<RedJade>(3)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class RedJadeColumnTile() : BaseColumnTile
        (3, 3, Coralite.RedJadeRed, DustID.GemRuby)
    {
        public override string Texture => AssetDirectory.MagikeColumnTiles + Name;
        public override int DropItemType => ItemType<RedJadeColumn>();

        public override MagikeTP GetEntityInstance() => GetInstance<RedJadeColumnTileEntity>();

        public override MALevel[] GetAllLevels()
        {
            return [
                MALevel.None,
                MALevel.RedJade,
                ];
        }
    }

    public class RedJadeColumnTileEntity : BaseSenderTileEntity<RedJadeColumnTile>
    {
        public override MagikeContainer GetStartContainer()
            => new RedJadeColumnTileContainer();

        public override MagikeLinerSender GetStartSender()
            => new RedJadeColumnTileSender();
    }

    public class RedJadeColumnTileContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.RedJade => 960,
                _ => 0,
            };
            LimitMagikeAmount();

            AntiMagikeMaxBase = MagikeMaxBase / 2;
            LimitAntiMagikeAmount();
        }
    }

    public class RedJadeColumnTileSender : UpgradeableLinerSender
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MaxConnectBase = 1;
            ConnectLengthBase = 6 * 16;
            SendDelayBase = 60 * 10;

            switch (incomeLevel)
            {
                default:
                case MALevel.None:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = 1_0000_0000;//随便填个大数
                    ConnectLengthBase = 0;
                    break;
                case MALevel.RedJade:
                    UnitDeliveryBase = 80;
                    break;
            }

            RecheckConnect();
        }
    }
}
