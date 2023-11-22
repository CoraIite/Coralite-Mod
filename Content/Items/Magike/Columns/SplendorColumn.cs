using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Columns
{
    public class SplendorColumn : BaseMagikePlaceableItem, IMagikeSenderItem
    {
        public SplendorColumn() : base(TileType<SplendorColumnTile>(), Item.sellPrice(0, 1, 0, 0)
            , RarityType<SplendorMagicoreRarity>(), 1000, AssetDirectory.MagikeColumns)
        { }

        public override int MagikeMax => 2_0000;
        public string SendDelay => "2";
        public int HowManyPerSend => 300;
        public int ConnectLengthMax => 5;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulColumn>()
                .AddIngredient<SplendorMagicore>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SplendorColumnTile : BaseColumnTile
    {
        public override string Texture => AssetDirectory.MagikeLensTiles + "SplendorLensTile";

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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<SplendorColumnEntity>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.SplendorMagicoreLightBlue);
            DustType = DustID.BlueFairy;
        }
    }

    public class SplendorColumnEntity : MagikeSender_Line
    {
        public const int sendDelay = 2 * 60;
        public int sendTimer;
        public SplendorColumnEntity() : base(2_0000, 5 * 16) { }

        public override ushort TileType => (ushort)TileType<SplendorColumnTile>();

        public override int HowManyPerSend => 300;

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
