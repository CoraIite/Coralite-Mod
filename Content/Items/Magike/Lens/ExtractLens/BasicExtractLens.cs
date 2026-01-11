using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
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

namespace Coralite.Content.Items.Magike.Lens.ExtractLens
{
    public class BasicExtractLens() : MagikeApparatusItem(TileType<BasicExtractLensTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(12)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicExtractLensTile() : BaseLensTile
        (Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override int DropItemType => ItemType<BasicExtractLens>();

        public override List<ushort> GetAllLevels()
        {
            return [
                NoneLevel.ID,
                CrystalLevel.ID,
                CrimsonLevel.ID,
                CorruptionLevel.ID,
                IcicleLevel.ID,
                BrilliantLevel.ID,
                SoulLevel.ID,
                FeatherLevel.ID,
                SplendorLevel.ID
                ];
        }
    }

    public class BasicExtractLensTileEntity : BaseCostItemProducerTileEntity<BasicExtractLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicExtractLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicExtractLensSender();

        public override MagikeCostItemProducer GetStartProducer()
            => new BasicExtractProducer();

        public override ItemContainer GetStartItemContainer()
            => new()
            {
                CapacityBase = 1
            };
    }

    public class BasicExtractLensContainer : UpgradeableContainer<BasicExtractLensTile>
    {
    }

    public class BasicExtractLensSender : UpgradeableLinerSender<BasicExtractLensTile>
    {
    }

    public class BasicExtractProducer : UpgradeableExtractProducer<BasicExtractLensTile>
    {
    }
}
