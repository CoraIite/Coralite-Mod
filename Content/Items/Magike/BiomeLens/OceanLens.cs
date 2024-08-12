using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.BiomeLens
{
    public class OceanLens : BaseMagikePlaceableItem, IMagikeGeneratorItem, IMagikeSenderItem
    {
        public OceanLens() : base(TileType<OceanLensTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeLens)
        { }

        public override int MagikeMax => 350;
        public string SendDelay => "10";
        public int HowManyPerSend => 20;
        public int ConnectLengthMax => 5;
        public int HowManyToGenerate => 18;
        public string GenerateDelay => "10";

        public override void AddRecipes()
        {
            //TODO: 添加虹水母/巨蟹后修改合成表
            CreateRecipe()
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient(ItemID.Coral, 10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneBeach;
        }
    }

    public class OceanLensTile : OldBaseLensTile
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
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.WaterPlacement = Terraria.Enums.LiquidPlacement.Allowed;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<OceanLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Aqua);
            DustType = DustID.BubbleBurst_Blue;
        }
    }

    public class OceanLensEntity : MagikeGenerator_Normal
    {
        public const int sendDelay = 10 * 60;
        public int sendTimer;
        public OceanLensEntity() : base(350, 5 * 16, 10 * 60) { }

        public override ushort TileType => (ushort)TileType<OceanLensTile>();

        public override int HowManyPerSend => 20;
        public override int HowManyToGenerate => 18;

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
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.Aqua, DustID.BubbleBurst_Blue);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.Aqua, DustID.BubbleBurst_Blue);
        }
    }
}
