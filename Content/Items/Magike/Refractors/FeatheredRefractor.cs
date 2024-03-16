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

namespace Coralite.Content.Items.Magike.Refractors
{
    public class FeatheredRefractor : BaseMagikePlaceableItem, IMagikeSenderItem
    {
        public FeatheredRefractor() : base(TileType<FeatheredRefractorTile>(), Item.sellPrice(0, 0, 50, 0)
            , RarityType<CrystallineMagikeRarity>(), 300, AssetDirectory.MagikeRefractors)
        { }

        public override int MagikeMax => 500;
        public int ConnectLengthMax => 40;
        public string SendDelay => "3";
        public int HowManyPerSend => 60;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BrilliantRefractor>()
                .AddIngredient<FlyingSnakeFeather>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class FeatheredRefractorTile : BaseRefractorTile
    {
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<FeatheredRefractorEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.GreenYellow);
            DustType = DustID.JungleTorch;
        }
    }

    public class FeatheredRefractorEntity : MagikeSender_Line
    {
        public const int sendDelay = 3 * 60;
        public int sendTimer;
        public FeatheredRefractorEntity() : base(500, 40 * 16) { }

        public override int HowManyPerSend => 60;

        public override ushort TileType => (ushort)TileType<FeatheredRefractorTile>();

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
            MagikeHelper.SpawnDustOnSend(1, 2, Position, container, Color.GreenYellow);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(1, 2, Position, Color.GreenYellow);
        }
    }

}
