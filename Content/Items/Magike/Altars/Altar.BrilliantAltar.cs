using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Altars
{
    public class BrilliantAltar : BaseMagikePlaceableItem, IMagikeSenderItem, IMagikeFactoryItem
    {
        public BrilliantAltar() : base(TileType<BrilliantAltarTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<CrystallineMagikeRarity>(), 300, AssetDirectory.MagikeAltars)
        { }

        public override int MagikeMax => 1200;
        public string SendDelay => "20";
        public int HowManyPerSend => 1;
        public int ConnectLengthMax => 10;
        public int HowManyCanConnect => 5;
        public string WorkTimeMax => "3 × 次要物品数量";
        public string WorkCost => "?";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ShadowAltar>()
                .AddIngredient<CrystallineMagike>(5)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BrilliantAltarTile : BaseAltarTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[2] {
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<BrilliantAltarEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Purple);
            DustType = DustID.PurpleTorch;
        }
    }

    public class BrilliantAltarEntity : MagikeFactory_PolymerizeAltar
    {
        public BrilliantAltarEntity() : base(1200, 60 * 3, 16 * 10, 5) { }

        public override Color MainColor => Coralite.Instance.CrystallineMagikePurple;

        public override ushort TileType => (ushort)TileType<BrilliantAltarTile>();

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 2, Position, MainColor);
        }
    }
}
