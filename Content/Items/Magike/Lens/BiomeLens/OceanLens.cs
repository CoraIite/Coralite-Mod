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

namespace Coralite.Content.Items.Magike.Lens.BiomeLens
{
    public class OceanLens() : MagikeApparatusItem(TileType<OceanLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneBeach;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.SandBlock, 5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class OceanLensTile() : BaseLensTile
        (Color.SandyBrown, DustID.Sand)
    {
        public override int DropItemType => ItemType<OceanLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Sand
            ];
        }

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                SeashoreLevel.ID,
                PelagicLevel.ID,
            ];
        }
    }

    public class OceanLensTileEntity : BaseActiveProducerTileEntity<OceanLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new OceanLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new OceanLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new OceanProducer();
    }

    public class OceanLensContainer : UpgradeableContainer<OceanLensTile>
    {
    }

    public class OceanLensSender : UpgradeableLinerSender<OceanLensTile>
    {
    }

    public class OceanProducer : UpgradeableProducerByBiome<OceanLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.OceanLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.OceanCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType == TileID.Sand;

        public override bool CheckWall(Tile tile)
            => true;
    }
}
