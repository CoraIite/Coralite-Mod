using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.SpecialLens
{
    public class CogLens() : MagikeApparatusItem(TileType<CogLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneDesert;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Skarn>(10)
                .AddIngredient(ItemID.Cog, 5)
                .AddCondition(CoraliteConditions.LearnedMagikeAdvance)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class CogLensTile() : BaseLensTile
        (Color.RosyBrown, DustID.SandstormInABottle)
    {
        public override int DropItemType => ItemType<CogLens>();

        public override MagikeTileEntity GetEntityInstance() => GetInstance<CogLensTileEntity>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Hallow,
            ];
        }
    }

    public class CogLensTileEntity : BaseActiveProducerTileEntity<CogLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new CogLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new CogLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new CogProducer();
    }

    public class CogLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Hallow:
                    MagikeMaxBase = 787;
                    AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
            }

            LimitMagikeAmount();
            LimitAntiMagikeAmount();
        }
    }

    public class CogLensSender : UpgradeableLinerSender
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
                    SendDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ConnectLengthBase = 0;
                    break;
                case MALevel.Hallow:
                    UnitDeliveryBase = 210;
                    SendDelayBase = 8;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class CogProducer : UpgradeableActiveProducer, IUIShowable
    {
        public MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.CogLensName;

        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 1;
                    break;
                case MALevel.Hallow:
                    ProductionDelayBase = 8;
                    ThroughputBase = 70;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(ApparatusName(), parent);

            UIList list =
            [
                //生产时间
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ProduceTime), parent),
                this.NewTextBar(ProductionDelayText,parent),

                //生产量
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ProduceAmount), parent),
                this.NewTextBar(ThroughputText, parent),
            ];

            list.SetTopLeft(title.Height.Pixels + 8, 0);
            list.SetSize(0, -list.Top.Pixels, 1, 1);
            list.QuickInvisibleScrollbar();

            parent.Append(list);
        }

        #endregion
    }
}
