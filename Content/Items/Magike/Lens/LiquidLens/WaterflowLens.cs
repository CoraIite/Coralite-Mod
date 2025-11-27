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
    public class WaterflowLens() : MagikeApparatusItem(TileType<WaterflowLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddIngredient(ItemID.WaterBucket)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class WaterflowLensTile() : BaseLensTile
        (Color.Red, DustID.Water)
    {
        public override int DropItemType => ItemType<WaterflowLens>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                GlistentLevel.ID,
                PelagicLevel.ID,
            ];
        }
    }

    public class WaterflowLensTileEntity : BaseActiveProducerTileEntity<WaterflowLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new WaterflowLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new WaterflowLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new WaterflowProducer();
    }

    public class WaterflowLensContainer : UpgradeableContainer<WaterflowLensTile>
    {
    }

    public class WaterflowLensSender : UpgradeableLinerSender<WaterflowLensTile>
    {
    }

    public class WaterflowProducer : UpgradeableProducerByLiquid<WaterflowLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.WaterflowLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.WaterflowCondition;

        public override int LiquidType => LiquidID.Water;
    }
}