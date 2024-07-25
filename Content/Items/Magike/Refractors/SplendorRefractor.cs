using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Tile;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class SplendorRefractor : BaseMagikePlaceableItem, IMagikeSenderItem
    {
        public SplendorRefractor() : base(TileType<SplendorRefractorTile>(), Item.sellPrice(0, 0, 50, 0)
            , RarityType<SplendorMagicoreRarity>(), 1000, AssetDirectory.MagikeRefractors)
        { }

        public override int MagikeMax => 1000;
        public int ConnectLengthMax => 45;
        public string SendDelay => "2";
        public int HowManyPerSend => 100;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FeatheredRefractor>()
                .AddIngredient<SplendorMagicore>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SplendorRefractorTile : OldBaseRefractorTile
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + "SplendorPentaprismTile";

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[2] {
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<SplendorRefractorEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.SplendorMagicoreLightBlue);
            DustType = DustID.BlueFairy;
        }
    }

    public class SplendorRefractorEntity : MagikeSender_Line
    {
        public const int sendDelay = 2 * 60;
        public int sendTimer;
        public SplendorRefractorEntity() : base(1000, 45 * 16) { }

        public override int HowManyPerSend => 100;

        public override ushort TileType => (ushort)TileType<SplendorRefractorTile>();

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
            MagikeHelper.SpawnDustOnSend(1, 2, Position, container, Coralite.Instance.SplendorMagicoreLightBlue);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(1, 2, Position, Coralite.Instance.SplendorMagicoreLightBlue);
        }
    }
}
