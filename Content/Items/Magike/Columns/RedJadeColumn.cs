using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.RedJades;
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
    public class RedJadeColumn() : MagikeApparatusItem(TileType<RedJadeColumnTile>(), Item.sellPrice(silver: 5)
            , ItemRarityID.Blue, AssetDirectory.MagikeColumns)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(20)
                .AddIngredient<RedJade>(3)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class RedJadeColumnTile() : BaseColumnTile
        (3, 3, Coralite.RedJadeRed, DustID.GemRuby)
    {
        public override string Texture => AssetDirectory.MagikeColumnTiles + Name;
        public override int DropItemType => ItemType<RedJadeColumn>();

        public override List<ushort> GetAllLevels()
        {
            return [
                NoneLevel.ID,
                RedJadeLevel.ID,
                ];
        }
    }

    public class RedJadeColumnTileEntity : BaseSenderTileEntity<RedJadeColumnTile>
    {
        public override int MainComponentID => MagikeComponentID.MagikeContainer;

        public override MagikeContainer GetStartContainer()
            => new RedJadeColumnTileContainer();

        public override MagikeLinerSender GetStartSender()
            => new RedJadeColumnTileSender();
    }

    public class RedJadeColumnTileContainer : UpgradeableContainer<RedJadeColumnTile>
    {
    }

    public class RedJadeColumnTileSender : UpgradeableLinerSender<RedJadeColumnTile>
    {
    }
}
