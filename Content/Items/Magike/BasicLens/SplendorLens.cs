using Coralite.Content.Raritys;
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
    public class SplendorLens : BaseMagikePlaceableItem, IMagikeGeneratorItem, IMagikeSenderItem
    {
        public SplendorLens() : base(TileType<SplendorLensTile>(), Item.sellPrice(0, 1, 0, 0), RarityType<SplendorMagicoreRarity>(), 1000)
        { }

        public override int MagikeMax => 2000;
        public string SendDelay => "5";
        public int HowManyPerSend => 400;
        public int ConnectLengthMax => 5;
        public int HowManyToGenerate => -1;
        public string GenerateDelay => "5";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FeatheredLens>()
                .AddIngredient<SplendorMagicore>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SplendorLensTile : BaseCostItemLensTile
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<SplendorLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.SplendorMagicoreLightBlue);
            DustType = DustID.BlueFairy;
        }
    }

    public class SplendorLensEntity : MagikeGenerator_FromMagItem
    {
        public const int sendDelay = 5 * 60;
        public int sendTimer;
        public SplendorLensEntity() : base(2000, 5 * 16, 5 * 60) { }

        public override ushort TileType => (ushort)TileType<SplendorLensTile>();

        public override int HowManyPerSend => 400;

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
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Coralite.Instance.SplendorMagicoreLightBlue);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.Instance.SplendorMagicoreLightBlue);
        }
    }
}
