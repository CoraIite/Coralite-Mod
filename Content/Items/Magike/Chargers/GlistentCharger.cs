﻿//using Coralite.Content.Items.Glistent;
//using Coralite.Content.Raritys;
//using Coralite.Core;
//using Coralite.Core.Systems.MagikeSystem;
//using Coralite.Core.Systems.MagikeSystem.BaseItems;
//using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
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
//    public class GlistentCharger : BaseMagikePlaceableItem, IMagikeFactoryItem, IMagikeCraftable
//    {
//        public GlistentCharger() : base(TileType<GlistentChargerTile>(), Item.sellPrice(0, 0, 10, 0)
//            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeFactories)
//        { }

//        public override int MagikeMax => 450;
//        public string WorkTimeMax => "?";
//        public string WorkCost => "5";

//        public void AddMagikeCraftRecipe()
//        {
//            //MagikeCraftRecipe.CreateRecipe<GlistentCharger>(75)
//            //    .SetMainItem<CrystalCharger>()
//            //    .AddIngredient<GlistentBar>(2)
//            //    .Register();
//        }
//    }

//    public class GlistentChargerTile : BaseChargerTile
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
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<GlistentChargerEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Color.GreenYellow);
//            DustType = DustID.GreenFairy;
//        }
//    }

//    public class GlistentChargerEntity : MagikeCharger
//    {
//        public GlistentChargerEntity() : base(450, 5) { }

//        public override ushort TileType => (ushort)TileType<GlistentChargerTile>();
//        public override Color MainColor => Color.GreenYellow;

//        public override void OnReceiveVisualEffect()
//        {
//            MagikeHelper.SpawnDustOnGenerate(2, 2, Position, Color.GreenYellow);
//        }
//    }
//}
