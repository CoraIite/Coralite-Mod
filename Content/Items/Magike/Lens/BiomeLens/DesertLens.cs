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
using Terraria.GameContent.Events;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.BiomeLens
{
    public class DesertLens() : MagikeApparatusItem(TileType<DesertLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneDesert;
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

    public class DesertLensTile() : BaseLensTile
        (Color.SandyBrown, DustID.Sand)
    {
        public override int DropItemType => ItemType<DesertLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.Sand,TileID.Sandstone,TileID.SandstoneBrick
            ];
        }

                public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                QuicksandLevel.ID,
                ForbiddenLevel.ID,
            ];
        }
    }

    public class DesertLensTileEntity : BaseActiveProducerTileEntity<DesertLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new DesertLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new DesertLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new DesertProducer();
    }

    public class DesertLensContainer : UpgradeableContainer<DesertLensTile>
    {
    }

    public class DesertLensSender : UpgradeableLinerSender<DesertLensTile>
    {
    }

    public class DesertProducer : UpgradeableProducerByBiome<DesertLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.DesertLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.DesertCondition;

        public override bool CheckTile(Tile tile)
            => tile.TileType is TileID.Sand or TileID.Sandstone or TileID.SandstoneBrick;

        public override int Throughput
        {
            get
            {
                if (Sandstorm.Happening)
                    return (int)(base.Throughput * 1.5f);
                return base.Throughput;
            }
        }

        public override bool CheckWall(Tile tile)
            => true;
    }
}
