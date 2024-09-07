//using Coralite.Content.Items.MagikeSeries1;
//using Coralite.Content.Raritys;
//using Coralite.Core;
//using Coralite.Core.Systems.MagikeSystem;
//using Coralite.Core.Systems.MagikeSystem.BaseItems;
//using Coralite.Core.Systems.MagikeSystem.Tile;
//using Coralite.Core.Systems.MagikeSystem.TileEntities;
//using Coralite.Helpers;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ObjectData;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Items.Magike.Refractors
//{
//    public class CrystalRefractor : BaseMagikePlaceableItem, IMagikeSenderItem
//    {
//        public CrystalRefractor() : base(TileType<CrystalRefractorTile>(), Item.sellPrice(0, 0, 5, 0)
//            , RarityType<MagicCrystalRarity>(), 25, AssetDirectory.MagikeRefractors)
//        { }

//        public override int MagikeMax => 50;
//        public int ConnectLengthMax => 25;
//        public string SendDelay => "5";
//        public int HowManyPerSend => 5;

//        public override void AddRecipes()
//        {
//            CreateRecipe()
//                .AddIngredient<MagicCrystal>(5)
//                .AddIngredient<Basalt>(10)
//                .AddCondition(CoraliteConditions.LearnedMagikeBase)
//                .AddTile(TileID.Anvils)
//                .Register();
//        }
//    }

//    public class CrystalRefractorTile : OldBaseRefractorTile
//    {
//        public override void SetStaticDefaults()
//        {
//            Main.tileShine[Type] = 400;
//            Main.tileFrameImportant[Type] = true;
//            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
//            TileID.Sets.IgnoredInHouseScore[Type] = true;

//            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
//            TileObjectData.newTile.Height = 2;
//            TileObjectData.newTile.CoordinateHeights = new int[2] {
//                16,
//                16
//            };
//            TileObjectData.newTile.DrawYOffset = 2;
//            TileObjectData.newTile.LavaDeath = false;
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrystalRefractorEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Coralite.MagicCrystalPink);
//            DustType = DustID.CrystalSerpent_Pink;
//        }
//    }

//    public class CrystalRefractorEntity : MagikeSender_Line
//    {
//        public const int sendDelay = 5 * 60;
//        public int sendTimer;
//        public CrystalRefractorEntity() : base(50, 25 * 16) { }

//        public override int HowManyPerSend => 5;

//        public override ushort TileType => (ushort)TileType<CrystalRefractorTile>();

//        public override bool CanSend()
//        {
//            sendTimer++;
//            if (sendTimer > sendDelay)
//            {
//                sendTimer = 0;
//                return true;
//            }

//            return false;
//        }

//        public override void SendVisualEffect(IMagikeContainer container)
//        {
//            MagikeHelper.SpawnDustOnSend(1, 2, Position, container, Coralite.MagicCrystalPink);
//        }

//        public override void OnReceiveVisualEffect()
//        {
//            MagikeHelper.SpawnDustOnGenerate(1, 2, Position, Coralite.MagicCrystalPink);
//        }
//    }
//}
