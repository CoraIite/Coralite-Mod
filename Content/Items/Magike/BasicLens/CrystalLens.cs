using Coralite.Content.Items.Magike.OtherPlaceables;
using Coralite.Content.Raritys;
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
    public class CrystalLens : BaseMagikePlaceableItem
    {
        public CrystalLens() : base(TileType<CrystalLensTile>(), Item.sellPrice(0, 0, 10, 0), RarityType<MagicCrystalRarity>(), 50)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(10)
                .AddIngredient<Basalt>(10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class CrystalLensTile : BaseCostItemLensTile
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrystalLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.MagicCrystalPink);
            DustType = DustID.CrystalSerpent_Pink;
        }
    }

    public class CrystalLensEntity : MagikeGenerator_FromMagItem
    {
        public const int sendDelay = 600;
        public int sendTimer;
        public CrystalLensEntity() : base(50, 5 * 16, 600) { }

        public override ushort TileType => (ushort)TileType<CrystalLensTile>();

        public override int HowManyPerSend => 5;

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

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Coralite.Instance.MagicCrystalPink);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.Instance.MagicCrystalPink);
        }
    }
}
