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

namespace Coralite.Content.Items.Magike.Pedestal
{
    public class BasicPedestal() : MagikeApparatusItem(TileType<BasicPedestalTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikePedestals)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicPedestalTile() : BasePedestalTile
        (1, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikePedestalTiles + Name;
        public override int DropItemType => ItemType<BasicPedestal>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                CrystalLevel.ID,
                GlistentLevel.ID,
                CrimsonLevel.ID,
                CorruptionLevel.ID,
                IcicleLevel.ID,
                BrilliantLevel.ID,
                HallowLevel.ID,
                SoulLevel.ID,
                FeatherLevel.ID,
                HolyLightLevel.ID,
                SplendorLevel.ID,
                HellstoneLevel.ID,
                EternalFlameLevel.ID,
                QuicksandLevel.ID,
                ForbiddenLevel.ID,
                EiderdownLevel.ID,
                FlightLevel.ID,
                SeashoreLevel.ID,
                PelagicLevel.ID,
                RedJadeLevel.ID,
            ];
        }
    }

    public class BasicPedestalTileEntity : Pedestal<BasicPedestalTile>
    {
        public override ItemContainer GetStartItemContainer() => new BasicPedestalContainer() { CapacityBase = 1 };
    }

    public class BasicPedestalContainer : UpgradeableItemContainer<BasicPedestalTile>
    {

    }
}
