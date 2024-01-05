using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refineries
{
    public class BrilliantRefinery : BaseMagikePlaceableItem, IMagikeFactoryItem
    {
        public BrilliantRefinery() : base(TileType<BrilliantRefineryTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<CrystallineMagikeRarity>(), 300, AssetDirectory.MagikeFactories)
        { }

        public override int MagikeMax => 800;
        public string WorkTimeMax => "10";
        public string WorkCost => "50";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystalRefinery>()
                .AddIngredient<CrystallineMagike>(10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class BrilliantRefineryTile : BaseRefineryTile
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<BrilliantRefineryEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.CrystallineMagikePurple);
            DustType = DustID.PurpleTorch;
        }
    }

    public class BrilliantRefineryEntity : Refinery
    {
        public BrilliantRefineryEntity() : base(800, 10 * 60, 50, 1) { }

        public override ushort TileType => (ushort)TileType<BrilliantRefineryTile>();
        public override Color MainColor => Coralite.Instance.CrystallineMagikePurple;

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.Instance.CrystallineMagikePurple);
        }
    }
}
