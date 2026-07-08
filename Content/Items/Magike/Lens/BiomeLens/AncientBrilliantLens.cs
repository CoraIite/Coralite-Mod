using Coralite.Content.Biomes;
using Coralite.Content.Dusts;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
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
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.BiomeLens
{
    public class AncientBrilliantLens() : MagikeApparatusItem(TileType<AncientBrilliantLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.InModBiome<CrystallineSkyIsland>();
        }
    }

    public class AncientBrilliantLensTile() : BaseLensTile
        (Coralite.CrystallinePurple, DustType<SkarnDust>(),3,3)
    {
        public override int DropItemType => ItemType<AncientBrilliantLens>();
        public override CoraliteSetsSystem.MagikeTileType PlaceType => CoraliteSetsSystem.MagikeTileType.None;

        public override void QuickLoadAsset(ushort level) { }

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileType<SkarnTile>(), TileType<SkarnBrickTile>(), TileType<CrystallineSkarnTile>(), TileType<ChalcedonySkarn>(),TileType<ChalcedonySmoothSkarn>(),TileType<SmoothSkarnTile>(),TileType<CrystallineBlockTile>(),TileType<CrystallineBrickTile>()
            ];
        }

        public override List<ushort> GetAllLevels()
        {
            return
            [
                BrilliantLevel.ID,
            ];
        }
    }

    public class AncientBrilliantLensTileEntity : BaseActiveProducerTileEntity<AncientBrilliantLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new AncientBrilliantLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new AncientBrilliantLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new CrystallineSkyIslandProducer();
    }

    public class AncientBrilliantLensContainer : UpgradeableContainer<AncientBrilliantLensTile>
    {
    }

    public class AncientBrilliantLensSender : UpgradeableLinerSender<AncientBrilliantLensTile>
    {
    }

    public class CrystallineSkyIslandProducer : UpgradeableProducerByBiome<AncientBrilliantLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.AncientBrilliantLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.CrystallineSkyIslandCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType == TileType<SkarnTile>() || tile.TileType == TileType<SkarnBrickTile>() || tile.TileType == TileType<CrystallineSkarnTile>() || tile.TileType == TileType<ChalcedonySkarn>() || tile.TileType == TileType<ChalcedonySmoothSkarn>() || tile.TileType == TileType<SmoothSkarnTile>() || tile.TileType == TileType<CrystallineBlockTile>() || tile.TileType == TileType<CrystallineBrickTile>();

        public override bool CheckWall(Tile tile)
            => true;
    }
}
