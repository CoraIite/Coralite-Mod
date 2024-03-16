using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.BasicLens
{
    public class FeatheredLens : BaseMagikePlaceableItem, IMagikeGeneratorItem, IMagikeSenderItem
    {
        public FeatheredLens() : base(TileType<FeatheredLensTile>(), Item.sellPrice(0, 1, 0, 0)
            , RarityType<CrystallineMagikeRarity>(), 600, AssetDirectory.MagikeLens)
        { }

        public override int MagikeMax => 900;
        public string SendDelay => "6";
        public int HowManyPerSend => 180;
        public int ConnectLengthMax => 5;
        public int HowManyToGenerate => -1;
        public string GenerateDelay => "6";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BrilliantLens>()
                .AddIngredient<FlyingSnakeFeather>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class FeatheredLensTile : BaseCostItemLensTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[3] {
                16,
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<FeatheredLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.GreenYellow);
            DustType = DustID.JungleTorch;
        }
    }

    public class FeatheredLensEntity : MagikeGenerator_FromMagItem
    {
        public const int sendDelay = 6 * 60;
        public int sendTimer;
        public FeatheredLensEntity() : base(900, 5 * 16, 6 * 60) { }

        public override ushort TileType => (ushort)TileType<FeatheredLensTile>();

        public override int HowManyPerSend => 180;

        public override bool CanSend()
        {
            sendTimer++;
            if (sendTimer > sendDelay)
            {
                sendTimer = 0;
                return true;
            }

            return false;
        }

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.GreenYellow);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.GreenYellow);
        }
    }
}
