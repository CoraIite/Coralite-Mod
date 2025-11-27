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

namespace Coralite.Content.Items.Magike.Refractors
{
    public class BasicRefractor() : MagikeApparatusItem(TileType<BasicRefractorTile>(), Item.sellPrice(silver: 5)
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

    public class BasicRefractorTile() : BaseRefractorTile
        (1, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public override int DropItemType => ItemType<BasicRefractor>();

        public override List<ushort> GetAllLevels()
        {
            return [
                NoneLevel.ID,
                CrystalLevel.ID,
                CrimsonLevel.ID,
                CorruptionLevel.ID,
                IcicleLevel.ID,
                BrilliantLevel.ID,
                SoulLevel.ID,
                FeatherLevel.ID,
                SplendorLevel.ID
                ];
        }
    }

    public class BasicRefractorTileEntity : BaseSenderTileEntity<BasicRefractorTile>
    {
        public override MagikeContainer GetStartContainer()
            => new BasicRefractorContainer();

        public override MagikeLinerSender GetStartSender()
            => new BasicRefractorSender();
    }

    public class BasicRefractorContainer : UpgradeableContainer<BasicRefractorTile>
    {
    }

    public class BasicRefractorSender : UpgradeableLinerSender<BasicRefractorTile>
    {
    }
}
