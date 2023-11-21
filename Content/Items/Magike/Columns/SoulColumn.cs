using Coralite.Content.Raritys;
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
    public class SoulColumn : BaseMagikePlaceableItem, IMagikeSenderItem
    {
        public SoulColumn() : base(TileType<SoulColumnTile>(), Item.sellPrice(0, 1, 0, 0), RarityType<CrystallineMagikeRarity>(), 600)
        { }

        public override int MagikeMax => 9000;
        public string SendDelay => "3";
        public int HowManyPerSend => 200;
        public int ConnectLengthMax => 5;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BrilliantColumn>()
                .AddIngredient(ItemID.Ectoplasm, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SoulColumnTile : BaseColumnTile
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<SoulColumnEntity>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);

            AddMapEntry(Color.SkyBlue);
            DustType = DustID.BlueFairy;
        }
    }

    public class SoulColumnEntity : MagikeSender_Line
    {
        public const int sendDelay = 3 * 60;
        public int sendTimer;
        public SoulColumnEntity() : base(9000, 5 * 16) { }

        public override ushort TileType => (ushort)TileType<SoulColumnTile>();

        public override int HowManyPerSend => 200;

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
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.SkyBlue);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.SkyBlue);
        }
    }
}
