using Coralite.Content.Items.Glistent;
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
    public class ForestLens() : MagikeApparatusItem(TileType<ForestLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneForest;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient<GlistentBar>(3)
                .AddIngredient(ItemID.GrassSeeds)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ForestLensTile() : BaseLensTile
        (Color.Green, DustID.Grass)
    {
        public override int DropItemType => ItemType<ForestLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Grass, TileID.HallowedGrass, TileID.GolfGrass, TileID.GolfGrassHallowed
            ];
        }

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                GlistentLevel.ID,
                BrilliantLevel.ID,
                SplendorLevel.ID
            ];
        }
    }

    public class ForestLensTileEntity : BaseActiveProducerTileEntity<ForestLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new ForestLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new ForestLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new ForestProducer();
    }

    public class ForestLensContainer : UpgradeableContainer<ForestLensTile>
    {
    }

    public class ForestLensSender : UpgradeableLinerSender<ForestLensTile>
    {
    }

    public class ForestProducer : UpgradeableProducerByBiome<ForestLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.ForestLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.ForestCondition;

        public override bool CheckTile(Tile tile)
            => TileID.Sets.Grass[tile.TileType];

        public override bool CheckWall(Tile tile)
            => tile.WallType is WallID.Grass or WallID.GrassUnsafe or WallID.Flower or WallID.FlowerUnsafe;
    }
}
