using Coralite.Content.Dusts;
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
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Columns
{
    public class AncientBrilliantColumn() : MagikeApparatusItem(TileType<AncientBrilliantColumnTile>(), Item.sellPrice(silver: 5)
            , RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeColumns)
    {
    }

    public class AncientBrilliantColumnTile() : BaseColumnTile
        (3, 3, Coralite.CrystallinePurple, DustType<SkarnDust>())
    {
        public override string Texture => AssetDirectory.MagikeColumnTiles + Name;
        public override int DropItemType => ItemType<AncientBrilliantColumn>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                BrilliantLevel.ID,
            ];
        }
    }

    public class AncientBrilliantColumnEntity : BaseSenderTileEntity<AncientBrilliantColumnTile>
    {
        public override int MainComponentID => MagikeComponentID.MagikeContainer;

        public override MagikeContainer GetStartContainer()
            => new AncientBrilliantColumnContainer();

        public override MagikeLinerSender GetStartSender()
            => new AncientBrilliantColumnSender();
    }

    public class AncientBrilliantColumnContainer : UpgradeableContainer<AncientBrilliantColumnTile>
    {
    }

    public class AncientBrilliantColumnSender : UpgradeableLinerSender<AncientBrilliantColumnTile>
    {
    }
}
