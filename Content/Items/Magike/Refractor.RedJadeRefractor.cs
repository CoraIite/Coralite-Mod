using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria;
using Coralite.Core.Prefabs.Items;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike
{
    public class RedJadeRefractor : BaseMagikePlaceableItem
    {
        public RedJadeRefractor() : base(TileType<RedJadeRefractorTile>(), Item.sellPrice(0, 0, 10, 0), RarityType<MagikeCrystalRarity>(), 25)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>()
                .AddIngredient<RedJades.RedJade>(2)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class RedJadeRefractorTile : BaseRefractorTile
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
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<RedJadeRefractorEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.RedJadeRed);
            DustType = DustID.GemRuby;
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class RedJadeRefractorEntity : MagikeSender_Line
    {
        public const int sendDelay =  60;
        public int sendTimer;
        public RedJadeRefractorEntity() : base(60, 25 * 16) { }

        public override int HowManyPerSend => 1;

        public override ushort TileType => (ushort)TileType<RedJadeRefractorTile>();

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
            MagikeHelper.SpawnDustOnSend(1, 2, Position, container, Coralite.Instance.RedJadeRed);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(1, 2, Position, Coralite.Instance.RedJadeRed);
        }
    }

}
