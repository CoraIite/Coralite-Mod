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
    public class LavaLens() : MagikeApparatusItem(TileType<LavaLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddIngredient(ItemID.LavaBucket)
                .AddIngredient(ItemID.HellstoneBar, 3)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class LavaLensTile() : BaseLensTile
        (Color.Red, DustID.Lava)
    {
        public override int DropItemType => ItemType<LavaLens>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                HellstoneLevel.ID,
                EternalFlameLevel.ID,
            ];
        }
    }

    public class LavaLensTileEntity : BaseActiveProducerTileEntity<LavaLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new LavaLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new LavaLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new LavaProducer();
    }

    public class LavaLensContainer : UpgradeableContainer<LavaLensTile>
    {
    }

    public class LavaLensSender : UpgradeableLinerSender<LavaLensTile>
    {
    }

    public class LavaProducer : UpgradeableProducerByLiquid<LavaLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.LavaLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.LavaCondition;

        public override int LiquidType => LiquidID.Lava;
    }
}