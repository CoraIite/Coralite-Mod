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
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.BasicLens
{
    public class BrilliantLens : BaseMagikePlaceableItem
    {
        public BrilliantLens() : base(TileType<BrilliantLensTile>(), Item.sellPrice(0, 1, 0, 0), RarityType<CrystallineMagikeRarity>(), 600)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrimtaneLens>()
                .AddIngredient<CrystallineMagike>(10)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<DemoniteLens>()
                .AddIngredient<CrystallineMagike>(10)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class BrilliantLensTile : BaseCostItemLensTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + "CrystalLensTile";

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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<BrilliantLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.CrystallineMagikePurple);
            DustType = DustID.PurpleTorch;
        }
    }

    public class BrilliantLensEntity : MagikeGenerator_FromMagItem
    {
        public const int sendDelay = 7 * 60;
        public int sendTimer;
        public BrilliantLensEntity() : base(600, 5 * 16, 420) { }

        public override ushort TileType => (ushort)TileType<BrilliantLensTile>();

        public override int HowManyPerSend => 60;

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
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Coralite.Instance.CrystallineMagikePurple);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.Instance.CrystallineMagikePurple);
        }
    }
}
