using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Tile;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Chargers
{
    public class BrilliantCharger : BaseMagikePlaceableItem, IMagikeFactoryItem
    {
        public BrilliantCharger() : base(TileType<BrilliantChargerTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<CrystallineMagikeRarity>(), 600, AssetDirectory.MagikeFactories)
        { }

        public override int MagikeMax => 700;
        public string WorkTimeMax => "?";
        public string WorkCost => "15";

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<CrystalCharger>()
            //    .AddIngredient<CrystallineMagike>(5)
            //    .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
            //    .AddTile(TileID.Anvils)
            //    .Register();
        }
    }

    public class BrilliantChargerTile : BaseChargerTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[2] {
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<BrilliantChargerEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.CrystallineMagikePurple);
            DustType = DustID.PurpleTorch;
        }
    }

    public class BrilliantChargerEntity : MagikeCharger
    {
        public BrilliantChargerEntity() : base(700, 15) { }

        public override ushort TileType => (ushort)TileType<BrilliantChargerTile>();
        public override Color MainColor => Coralite.Instance.CrystallineMagikePurple;

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 2, Position, Coralite.Instance.CrystallineMagikePurple);
        }
    }
}
