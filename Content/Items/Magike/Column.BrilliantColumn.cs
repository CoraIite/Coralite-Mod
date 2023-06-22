using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria;
using Coralite.Core.Prefabs.Items;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike
{
    public class BrilliantColumn : BaseMagikePlaceableItem
    {
        public BrilliantColumn() : base(TileType<BrilliantColumnTile>(), Item.sellPrice(0, 1, 0, 0), RarityType<CrystallineMagikeRarity>(), 600)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrimtaneColumn>()
                .AddIngredient<CrystallineMagike>(10)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddCondition(this.GetLocalization("RecipeCondition"), () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<DemoniteColumn>()
                .AddIngredient<CrystallineMagike>(10)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddCondition(this.GetLocalization("RecipeCondition"), () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class BrilliantColumnTile : BaseColumnTile
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
                18
            };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<BrilliantColumnEntity>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.CrystallineMagikePurple);
            DustType = DustID.PurpleTorch;
        }
    }

    public class BrilliantColumnEntity : MagikeSender_Line
    {
        public const int sendDelay = 3 * 60 + 30;
        public int sendTimer;
        public BrilliantColumnEntity() : base(6000, 5 * 16) { }

        public override ushort TileType => (ushort)TileType<BrilliantColumnTile>();

        public override int HowManyPerSend => 120;

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

        public override void SendVisualEffect(MagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Coralite.Instance.CrystallineMagikePurple);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.Instance.CrystallineMagikePurple);
        }
    }
}
