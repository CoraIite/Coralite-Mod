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
    public class HellLens() : MagikeApparatusItem(TileType<HellLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneUnderworldHeight;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.AshBlock, 10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HellLensTile() : BaseLensTile
        (Color.DarkRed, DustID.Torch)
    {
        public override int DropItemType => ItemType<HellLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Ash,TileID.AshGrass,TileID.Hellstone,TileID.HellstoneBrick,TileID.AncientHellstoneBrick,TileID.ObsidianBrick
            ];
        }

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

    public class HellLensTileEntity : BaseActiveProducerTileEntity<HellLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new HellLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new HellLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new HellProducer();
    }

    public class HellLensContainer : UpgradeableContainer<HellLensTile>
    {
    }

    public class HellLensSender : UpgradeableLinerSender<HellLensTile>
    {
    }

    public class HellProducer : UpgradeableProducerByBiome<HellLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.HellLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.HellCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType is TileID.Ash or TileID.AshGrass or TileID.Hellstone or TileID.HellstoneBrick or TileID.AncientHellstoneBrick or TileID.ObsidianBrick;

        public override bool CheckWall(Tile tile)
            => true;
    }
}
