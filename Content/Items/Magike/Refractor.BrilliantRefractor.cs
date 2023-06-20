using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike
{
    public class BrilliantRefractor: BaseMagikePlaceableItem
    {
        public BrilliantRefractor() : base(TileType<BrilliantRefractorTile>(), Item.sellPrice(0, 0, 50, 0), RarityType<CrystallineMagikeRarity>(), 300)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrimtaneRefractor>()
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient(ItemID.SoulofLight, 4)
                .AddCondition(this.GetLocalization("RecipeCondition"), () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<DemoniteRefractor>()
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient(ItemID.SoulofLight, 4)
                .AddCondition(this.GetLocalization("RecipeCondition"), () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class BrilliantRefractorTile : BaseRefractorTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + "CrystalRefractorTile";

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
                18
            };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<BrilliantRefractorEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.CrystallineMagikePurple);
            DustType = DustID.PurpleTorch;
        }
    }

    public class BrilliantRefractorEntity : MagikeSender
    {
        public const int sendDelay = 3 * 60+30;
        public int sendTimer;
        public BrilliantRefractorEntity() : base(300, 35 * 16) { }

        public override int HowManyPerSend => 60;

        public override ushort TileType => (ushort)TileType<BrilliantRefractorTile>();

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
            MagikeHelper.SpawnDustOnSend(1, 2, Position, container, Coralite.Instance.MagicCrystalPink);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(1, 2, Position, Coralite.Instance.MagicCrystalPink);
        }
    }
}
