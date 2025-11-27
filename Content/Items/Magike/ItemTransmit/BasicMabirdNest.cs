using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.ItemTransmit
{
    public class BasicMabirdNest() : MagikeApparatusItem(TileType<BasicMabirdNestTile>(), Item.sellPrice(silver: 5)
        , RarityType<CrystallineMagikeRarity>(), AssetDirectory.ItemTransmits), IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<SmoothSkarn, BasicMabirdNest>
                (MagikeHelper.CalculateMagikeCost<BrilliantLevel>( 6, 45), 12)
                .AddIngredient<SkarnBrick>(5)
                .AddIngredient<CrystallineMagike>(3)
                .Register();
        }
    }

    public class BasicMabirdNestTile() : BaseMabirdNestTile
        (3, 6, Coralite.CrystallinePurple, DustType<SkarnDust>())
    {
        public override int DropItemType => ItemType<BasicMabirdNest>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                BrilliantLevel.ID,
            ];
        }
    }

    public class BasicMabirdNestEntity : BaseMabirdNest<BasicMabirdNestTile>
    {
        public override MabirdController GetStartMabirdController()
            => new BasicMabirdNestController();
    }

    public class BasicMabirdNestController : MabirdController
    {
        public override void Initialize()
        {
            CapacityBase = 6;
        }
    }
}
