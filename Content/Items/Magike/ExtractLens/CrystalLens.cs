//using Coralite.Content.Items.MagikeSeries1;
//using Coralite.Content.Raritys;
//using Coralite.Core;
//using Coralite.Core.Systems.MagikeSystem;
//using Coralite.Core.Systems.MagikeSystem.BaseItems;
//using Coralite.Core.Systems.MagikeSystem.Tiles;
//using Coralite.Core.Systems.MagikeSystem.TileEntities;
//using Coralite.Helpers;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ObjectData;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Items.Magike.BasicLens
//{
//    public class CrystalLens : BaseMagikePlaceableItem, IMagikeSenderItem, IMagikeGeneratorItem
//    {
//        public CrystalLens() : base(TileType<CrystalLensTile>(), Item.sellPrice(0, 0, 10, 0)
//            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeLens)
//        { }

//        public override int MagikeMax => 50;
//        public string SendDelay => "10";
//        public int HowManyPerSend => 5;
//        public int ConnectLengthMax => 5;
//        public int HowManyToGenerate => -1;
//        public string GenerateDelay => "10";

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

//    public class CrystalLensTile : BaseCostItemLensTile
//    {
//        public override void SetStaticDefaults()
//        {
//            Main.tileShine[Type] = 400;
//            Main.tileFrameImportant[Type] = true;
//            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
//            TileID.Sets.IgnoredInHouseScore[Type] = true;

//            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
//            TileObjectData.newTile.Height = 3;
//            TileObjectData.newTile.CoordinateHeights = new int[3] {
//                16,
//                16,
//                16
//            };
//            TileObjectData.newTile.DrawYOffset = 2;
//            TileObjectData.newTile.LavaDeath = false;
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrystalLensEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Coralite.MagicCrystalPink);
//            DustType = DustID.CrystalSerpent_Pink;
//        }
//    }

//    public class CrystalLensEntity : MagikeGenerator_FromMagItem
//    {
//        public const int sendDelay = 60 * 10;
//        public int sendTimer;
//        public CrystalLensEntity() : base(50, 5 * 16, 60 * 10) { }

//        public override ushort TileType => (ushort)TileType<CrystalLensTile>();

//        public override int HowManyPerSend => 5;

//        public override bool CanSend()
//        {
//            sendTimer++;
//            if (sendTimer > sendDelay)
//            {
//                sendTimer = 0;
//                //Main.NewText(receiverPoints[0]);
//                //Main.NewText(magike);
//                return true;
//            }

//            return false;
//        }

//        public override void SendVisualEffect(IMagikeContainer container)
//        {
//            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Coralite.MagicCrystalPink);
//        }

//        public override void OnReceiveVisualEffect()
//        {
//            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.MagicCrystalPink);
//        }
//    }
//}
