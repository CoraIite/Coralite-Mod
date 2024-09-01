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

//namespace Coralite.Content.Items.Magike.LiquidLens
//{
//    public class SweetLens : BaseMagikePlaceableItem, IMagikeGeneratorItem, IMagikeSenderItem
//    {
//        public SweetLens() : base(TileType<SweetLensTile>(), Item.sellPrice(0, 0, 10, 0)
//            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeLens)
//        { }

//        public override int MagikeMax => 80;
//        public string SendDelay => "10";
//        public int HowManyPerSend => 3;
//        public int ConnectLengthMax => 5;
//        public int HowManyToGenerate => 3;
//        public string GenerateDelay => "10";

//        public override void AddRecipes()
//        {
//            CreateRecipe()
//                .AddIngredient<MagicCrystal>(2)
//                .AddIngredient(ItemID.BeeWax, 8)
//                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
//                .AddTile(TileID.Anvils)
//                .Register();
//        }
//    }

//    public class SweetLensTile : OldBaseLensTile
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
//            TileObjectData.newTile.LavaDeath = true;
//            //TileObjectData.newTile.WaterDeath = true;
//            TileObjectData.newTile.LavaPlacement = Terraria.Enums.LiquidPlacement.NotAllowed;
//            TileObjectData.newTile.WaterPlacement = Terraria.Enums.LiquidPlacement.Allowed;
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<SweetLensEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Color.OrangeRed);
//            DustType = DustID.Lava;
//        }
//    }

//    public class SweetLensEntity : MagikeGenerator_Normal
//    {
//        public const int sendDelay = 10 * 60;
//        public int sendTimer;
//        public SweetLensEntity() : base(80, 5 * 16, 10 * 60) { }

//        public override ushort TileType => (ushort)TileType<SweetLensTile>();

//        public override int HowManyPerSend => 3;
//        public override int HowManyToGenerate => 3;

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

//        public override void OnGenerate(int howMany)
//        {
//            GenerateAndChargeSelf(howMany);
//        }

//        public override bool CanGenerate()
//        {
//            Point point = new(Position.X, Position.Y + 2);
//            Tile tile = Framing.GetTileSafely(point);
//            return tile.LiquidType == LiquidID.Honey && tile.LiquidAmount > 128;
//        }

//        public override void SendVisualEffect(IMagikeContainer container)
//        {
//            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.OrangeRed);
//        }

//        public override void OnReceiveVisualEffect()
//        {
//            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.OrangeRed, DustID.FireworksRGB);
//        }
//    }
//}
