//using Coralite.Content.Items.MagikeSeries1;
//using Coralite.Content.Raritys;
//using Coralite.Core;
//using Coralite.Core.Systems.MagikeSystem;
//using Coralite.Core.Systems.MagikeSystem.BaseItems;
//using Coralite.Core.Systems.MagikeSystem.TileEntities;
//using Coralite.Core.Systems.MagikeSystem.Tiles;
//using Coralite.Helpers;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ObjectData;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Items.Magike.Chargers
//{
//    public class CrystalCharger : BaseMagikePlaceableItem, IMagikeFactoryItem
//    {
//        public CrystalCharger() : base(TileType<CrystalChargerTile>(), Item.sellPrice(0, 0, 10, 0)
//            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeFactories)
//        { }

//        public override int MagikeMax => 300;
//        public string WorkTimeMax => "?";
//        public string WorkCost => "5";

//        public override void AddRecipes()
//        {
//            CreateRecipe()
//                .AddIngredient<MagicCrystal>(10)
//                .AddIngredient<Basalt>(10)
//                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
//                .AddTile(TileID.Anvils)
//                .Register();
//        }
//    }

//    public class CrystalChargerTile : BaseChargerTile
//    {
//        public override void SetStaticDefaults()
//        {
//            Main.tileShine[Type] = 400;
//            Main.tileFrameImportant[Type] = true;
//            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
//            TileID.Sets.IgnoredInHouseScore[Type] = true;

//            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
//            TileObjectData.newTile.CoordinatePadding = 2;
//            TileObjectData.newTile.Height = 2;
//            TileObjectData.newTile.CoordinateHeights = new int[2] {
//                16,
//                16
//            };
//            TileObjectData.newTile.DrawYOffset = 2;
//            TileObjectData.newTile.LavaDeath = false;
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrystalChargerEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Coralite.MagicCrystalPink);
//            DustType = DustID.CrystalSerpent_Pink;
//        }
//    }

//    public class CrystalChargerEntity : MagikeCharger
//    {
//        public CrystalChargerEntity() : base(300, 5) { }

//        public override ushort TileType => (ushort)TileType<CrystalChargerTile>();
//        public override Color MainColor => Coralite.MagicCrystalPink;

//        public override void OnReceiveVisualEffect()
//        {
//            MagikeHelper.SpawnDustOnGenerate(2, 2, Position, Coralite.MagicCrystalPink);
//        }
//    }
//}
