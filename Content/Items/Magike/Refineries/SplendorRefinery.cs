﻿//using Coralite.Content.Raritys;
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

//namespace Coralite.Content.Items.Magike.Refineries
//{
//    public class SplendorRefinery : BaseMagikePlaceableItem, IMagikeFactoryItem
//    {
//        public SplendorRefinery() : base(TileType<SplendorRefineryTile>(), Item.sellPrice(0, 0, 10, 0)
//            , RarityType<SplendorMagicoreRarity>(), 1000, AssetDirectory.MagikeFactories)
//        { }

//        public override int MagikeMax => 2500;
//        public string WorkTimeMax => "5";
//        public string WorkCost => "50";

//        public override void AddRecipes()
//        {
//            CreateRecipe()
//                .AddIngredient<BrilliantRefinery>()
//                .AddIngredient<SplendorMagicore>(10)
//                .AddTile(TileID.MythrilAnvil)
//                .Register();
//        }
//    }

//    public class SplendorRefineryTile : BaseRefineryTile
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
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<SplendorRefineryEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Coralite.CrystallineMagikePurple);
//            DustType = DustID.PurpleTorch;
//        }
//    }

//    public class SplendorRefineryEntity : Refinery
//    {
//        public SplendorRefineryEntity() : base(2500, 5 * 60, 50, 1) { }

//        public override ushort TileType => (ushort)TileType<SplendorRefineryTile>();
//        public override Color MainColor => Coralite.CrystallineMagikePurple;

//        public override void OnReceiveVisualEffect()
//        {
//            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.CrystallineMagikePurple);
//        }
//    }
//}
