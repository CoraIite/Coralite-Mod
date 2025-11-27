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

namespace Coralite.Content.Items.Magike.Lens.LiquidLens
{
    public class HoneyLens() : MagikeApparatusItem(TileType<HoneyLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddIngredient(ItemID.HoneyBucket)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HoneyLensTile() : BaseLensTile
        (Color.Honeydew, DustID.Honey)
    {
        public override int DropItemType => ItemType<HoneyLens>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                BeeswaxLevel.ID,
                BrilliantLevel.ID,
                FeatherLevel.ID,
            ];
        }
    }

    public class HoneyLensTileEntity : BaseActiveProducerTileEntity<HoneyLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new HoneyLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new HoneyLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new HoneyProducer();
    }

    public class HoneyLensContainer : UpgradeableContainer<HoneyLensTile>
    {
    }

    public class HoneyLensSender : UpgradeableLinerSender<HoneyLensTile>
    {
    }

    public class HoneyProducer : UpgradeableProducerByLiquid<HoneyLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.HoneyLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.HoneyCondition;

        public override int LiquidType => LiquidID.Honey;
    }
}