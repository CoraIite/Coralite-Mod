using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Tile;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.LiquidLens
{
    public class HoneypotLens : BaseMagikePlaceableItem, IMagikeGeneratorItem, IMagikeSenderItem
    {
        public HoneypotLens() : base(TileType<HoneypotLensTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeLens)
        { }

        public override int MagikeMax => 800;
        public string SendDelay => "6";
        public int HowManyPerSend => 15;
        public int ConnectLengthMax => 5;
        public int HowManyToGenerate => 15;
        public string GenerateDelay => "6";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SweetLens>()
                .AddIngredient(ItemID.ChlorophyteBar, 4)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HoneypotLensTile : BaseLensTile
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
            TileObjectData.newTile.LavaDeath = true;
            //TileObjectData.newTile.WaterDeath = true;
            TileObjectData.newTile.LavaPlacement = Terraria.Enums.LiquidPlacement.NotAllowed;
            TileObjectData.newTile.WaterPlacement = Terraria.Enums.LiquidPlacement.Allowed;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<HoneypotLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.OrangeRed);
            DustType = DustID.Lava;
        }
    }

    public class HoneypotLensEntity : MagikeGenerator_Normal
    {
        public const int sendDelay = 6 * 60;
        public int sendTimer;
        public HoneypotLensEntity() : base(800, 5 * 16, 6 * 60) { }

        public override ushort TileType => (ushort)TileType<HoneypotLensTile>();

        public override int HowManyPerSend => 15;
        public override int HowManyToGenerate => 15;

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

        public override void OnGenerate(int howMany)
        {
            GenerateAndChargeSelf(howMany);
        }

        public override bool CanGenerate()
        {
            Point point = new Point(Position.X, Position.Y + 2);
            Tile tile = Framing.GetTileSafely(point);
            return tile.LiquidType == LiquidID.Honey && tile.LiquidAmount > 128;
        }

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.OrangeRed);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.OrangeRed, DustID.FireworksRGB);
        }
    }

}
