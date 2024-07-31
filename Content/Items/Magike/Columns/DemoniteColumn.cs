//using Coralite.Content.Items.Glistent;
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
//    public class DemoniteColumn : BaseMagikePlaceableItem, IMagikePolymerizable, IMagikeSenderItem
//    {
//        public DemoniteColumn() : base(TileType<DemoniteColumnTile>(), Item.sellPrice(0, 0, 25, 0)
//            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeColumns)
//        { }

//        public override int MagikeMax => 750;
//        public string SendDelay => "4.5";
//        public int HowManyPerSend => 30;
//        public int ConnectLengthMax => 5;

//        public void AddMagikePolymerizeRecipe()
//        {
//            PolymerizeRecipe.CreateRecipe<DemoniteColumn>(75)
//                .SetMainItem<CrystalColumn>()
//                .AddIngredient<GlistentBar>(2)
//                .AddIngredient(ItemID.ShadowScale, 3)
//                .Register();
//        }
//    }

//    public class DemoniteColumnTile : BaseColumnTile
//    {
//        public override void SetStaticDefaults()
//        {
//            //Main.tileShine[Type] = 400;
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
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<DemoniteColumnEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Color.MediumPurple);
//            DustType = DustID.Demonite;
//        }
//    }

//    public class DemoniteColumnEntity : MagikeSender_Line
//    {
//        public const int sendDelay = 4 * 60 + 30;
//        public int sendTimer;
//        public DemoniteColumnEntity() : base(750, 5 * 16) { }

//        public override ushort TileType => (ushort)TileType<DemoniteColumnTile>();

//        public override int HowManyPerSend => 30;

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
//            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.MediumPurple);
//        }

//        public override void OnReceiveVisualEffect()
//        {
//            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.MediumPurple);
//        }
//    }
//}
