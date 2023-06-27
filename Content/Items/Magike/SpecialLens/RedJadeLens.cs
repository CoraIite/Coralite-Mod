using Coralite.Content.Raritys;
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

namespace Coralite.Content.Items.Magike.SpecialLens
{
    public class RedJadeLens : BaseMagikePlaceableItem
    {
        public RedJadeLens() : base(TileType<RedJadeLensTile>(), Item.sellPrice(0, 0, 10, 0), RarityType<MagikeCrystalRarity>(), 50)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddIngredient<RedJades.RedJade>(5)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class RedJadeLensTile : BaseCostItemLensTile
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<RedJadeLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.RedJadeRed);
            DustType = DustID.GemRuby;
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class RedJadeLensEntity : MagikeGenerator
    {
        public const int sendDelay = 60 * 10;
        public int sendTimer;
        public RedJadeLensEntity() : base(20, 5 * 16) { }

        public override ushort TileType => (ushort)TileType<RedJadeLensTile>();

        public override int HowManyPerSend => 1;

        public override int HowManyToGenerate => 0;

        public override bool CanSend()
        {
            sendTimer++;
            if (sendTimer > sendDelay)
            {
                sendTimer = 0;
                //Main.NewText(receiverPoints[0]);
                //Main.NewText(magike);
                return true;
            }

            return false;
        }

        public override void CheckActive()
        {
            active = magike > 0;
        }

        public override bool CanGenerate()
        {
            return false;
        }

        public override void OnGenerate(int howMany) { }

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Coralite.Instance.RedJadeRed);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.Instance.RedJadeRed);
        }
    }
}
