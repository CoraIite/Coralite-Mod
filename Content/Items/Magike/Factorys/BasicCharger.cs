using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Terraria;
using Terraria.ID;
using static Coralite.Helpers.MagikeHelper;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Factorys
{
    public class BasicCharger() : MagikeApparatusItem(TileType<BasicChargerTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeFactories)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient<MagicCrystalBlock>(5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicChargerTile() : BaseChargerTile
        (3, 1, Coralite.MagicCrystalPink, DustID.CorruptionThorns,topSoild:true)
    {
        public override string Texture => AssetDirectory.MagikeFactoryTiles + Name;
        public override int DropItemType => ItemType<BasicCharger>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.MagicCrystal,
                MALevel.Glistent,
                MALevel.CrystallineMagike,
                MALevel.SplendorMagicore,
            ];
        }
    }

    public class BasicChargerTileEntity() : MagikeCharger<BasicChargerTile>()
    {
        public override MagikeContainer GetStartContainer()
            => new BasicChargerContainer();

        public override Charger GetStartCharger()
            => new BasicChargerFactory();

        public override ItemContainer GetStartItemContainer()
            => new ItemContainer()
            {
                CapacityBase = 1,
            };
    }

    public class BasicChargerContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = CalculateMagikeCost(incomeLevel, 6);
            LimitMagikeAmount();
        }
    }

    public class BasicChargerFactory : UpgradeableCharger
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            float t = incomeLevel switch
            {
                MALevel.MagicCrystal => 3,
                MALevel.Glistent => 2.5f,
                MALevel.CrystallineMagike => 2,
                MALevel.SplendorMagicore => 1,
                _ => 10_0000_0000 / 60,
            };

            WorkTimeBase = (int)(60 * t);
            MagikePerCharge = CalculateMagikeCost(incomeLevel, 2, 20);
        }
    }
}
