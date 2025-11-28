using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Columns
{
    public class BasicColumn() : MagikeApparatusItem(TileType<BasicColumnTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeColumns)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(20)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicColumnTile() : BaseColumnTile
        (3, 3, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeColumnTiles + Name;
        public override int DropItemType => ItemType<BasicColumn>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                CrystalLevel.ID,
                CrimsonLevel.ID,
                CorruptionLevel.ID,
                IcicleLevel.ID,
                BrilliantLevel.ID,
                SoulLevel.ID,
                FeatherLevel.ID,
                SplendorLevel.ID,
            ];
        }
    }

    public class BasicColumnTileEntity : BaseSenderTileEntity<BasicColumnTile>
    {
        public override int MainComponentID => MagikeComponentID.MagikeContainer;

        public override MagikeContainer GetStartContainer()
            => new BasicColumnTileContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicColumnTileSender();
    }

    public class BasicColumnTileContainer : UpgradeableContainer<BasicColumnTile>
    {
    }

    public class BasicColumnTileSender : UpgradeableLinerSender<BasicColumnTile>
    {
    }
}
