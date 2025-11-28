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
    public class DungeonLens() : MagikeApparatusItem(TileType<DungeonLensTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneDungeon;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.BlueBrick, 10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.GreenBrick, 10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient(ItemID.PinkBrick, 10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DungeonLensTile() : BaseLensTile
        (Color.DimGray, DustID.Bone)
    {
        public override int DropItemType => ItemType<DungeonLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick,
                TileID.AncientBlueBrick, TileID.AncientGreenBrick, TileID.AncientPinkBrick,
            ];
        }

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                BoneLevel.ID,
                SoulLevel.ID,
            ];
        }
    }

    public class DungeonLensTileEntity : BaseActiveProducerTileEntity<DungeonLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new DungeonLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new DungeonLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new DungeonProducer();
    }

    public class DungeonLensContainer : UpgradeableContainer<DungeonLensTile>
    {
    }

    public class DungeonLensSender : UpgradeableLinerSender<DungeonLensTile>
    {
    }

    public class DungeonProducer : UpgradeableProducerByBiome<DungeonLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.DungeonLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.DungeonCondition;

        public override bool CheckTile(Tile tile)
            => TileID.Sets.DungeonBiome[tile.TileType] > 0;

        public override bool CheckWall(Tile tile)
            => tile.WallType is WallID.BlueDungeon or WallID.BlueDungeonUnsafe or WallID.BlueDungeonSlab or WallID.BlueDungeonSlabUnsafe or WallID.BlueDungeonTile or WallID.BlueDungeonTileUnsafe
            or WallID.GreenDungeon or WallID.GreenDungeonUnsafe or WallID.GreenDungeonSlab or WallID.GreenDungeonSlabUnsafe or WallID.GreenDungeonTile or WallID.GreenDungeonTileUnsafe
            or WallID.PinkDungeon or WallID.PinkDungeonUnsafe or WallID.PinkDungeonSlab or WallID.PinkDungeonSlabUnsafe or WallID.PinkDungeonTile or WallID.PinkDungeonTileUnsafe;
    }
}
