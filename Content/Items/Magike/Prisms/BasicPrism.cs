using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Prisms
{
    public class BasicPrism() : MagikeApparatusItem(TileType<BasicPrismTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeRefractors)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(14)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicPrismTile() : BasePrismTile
        (2, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns, 8)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public override int DropItemType => ItemType<BasicPrism>();

        public override List<ushort> GetAllLevels()
        {
            return [
                NoneLevel.ID,
                CrystalLevel.ID,
                GlistentLevel.ID,
                CrimsonLevel.ID,
                CorruptionLevel.ID,
                IcicleLevel.ID,
                BrilliantLevel.ID,
                HallowLevel.ID,
                HolyLightLevel.ID,
                SplendorLevel.ID
                ];
        }
    }

    public class BasicPrismTileEntity : BaseSenderTileEntity<BasicPrismTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicPrismContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicPrismSender();
    }

    public class BasicPrismContainer : UpgradeableContainer<BasicPrismTile>
    {
    }

    public class BasicPrismSender : UpgradeableLinerSender<BasicPrismTile>
    {
    }
}
