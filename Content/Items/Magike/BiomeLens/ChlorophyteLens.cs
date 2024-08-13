using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.BiomeLens
{
    public class ChlorophyteLens : BaseMagikePlaceableItem, IMagikeGeneratorItem, IMagikeSenderItem
    {
        public ChlorophyteLens() : base(TileType<ChlorophyteLensTile>(), Item.sellPrice(0, 0, 50, 0)
            , RarityType<CrystallineMagikeRarity>(), 300, AssetDirectory.MagikeLens)
        { }

        public override int MagikeMax => 700;
        public string SendDelay => "6";
        public int HowManyPerSend => 15;
        public int ConnectLengthMax => 5;
        public int HowManyToGenerate => 15;
        public string GenerateDelay => "6";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneJungle;
        }
    }

    public class ChlorophyteLensTile : OldBaseLensTile
    {
        public override void SetStaticDefaults()
        {
            //Main.tileShine[Type] = 400;
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
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.WaterPlacement = Terraria.Enums.LiquidPlacement.Allowed;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<ChlorophyteLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Green);
            DustType = DustID.Chlorophyte;
        }
    }

    public class ChlorophyteLensEntity : MagikeGenerator_Normal
    {
        public const int sendDelay = 6 * 60;
        public int sendTimer;
        public ChlorophyteLensEntity() : base(700, 5 * 16, 6 * 60) { }

        public override ushort TileType => (ushort)TileType<ChlorophyteLensTile>();

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

        public override bool CanGenerate() => true;

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.Green, DustID.Chlorophyte);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.Green, DustID.Chlorophyte);
        }
    }
}
