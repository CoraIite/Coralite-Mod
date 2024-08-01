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

//namespace Coralite.Content.Items.Magike.Columns
//{
//    public class CrystalColumn : BaseMagikePlaceableItem, IMagikeSenderItem
//    {
//        public CrystalColumn() : base(TileType<CrystalColumnTile>(), Item.sellPrice(0, 0, 10, 0)
//            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeColumns)
//        { }

//        public override int MagikeMax => 500;
//        public string SendDelay => "5";
//        public int HowManyPerSend => 10;
//        public int ConnectLengthMax => 5;

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

//    public class CrystalColumnTile : BaseColumnTile
//    {
//        public override string Texture => AssetDirectory.MagikeLensTiles + "CrystalLensTile";

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
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrystalColumnEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Coralite.MagicCrystalPink);
//            DustType = DustID.CrystalSerpent_Pink;
//        }
//    }

//    public class CrystalColumnEntity : MagikeSender_Line
//    {
//        public const int sendDelay = 5 * 60;
//        public int sendTimer;
//        public CrystalColumnEntity() : base(500, 5 * 16) { }

//        public override ushort TileType => (ushort)TileType<CrystalColumnTile>();

//        public override int HowManyPerSend => 10;

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
