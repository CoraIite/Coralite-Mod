using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Tile;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.BasicLens
{
    public class CrimtaneLens : BaseMagikePlaceableItem, IMagikePolymerizable, IMagikeGeneratorItem, IMagikeSenderItem
    {
        public CrimtaneLens() : base(TileType<CrimtaneLensTile>(), Item.sellPrice(0, 0, 50, 0),
            RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeLens)
        { }

        public override int MagikeMax => 150;
        public string SendDelay => "9.5";
        public int HowManyPerSend => 15;
        public int ConnectLengthMax => 5;
        public int HowManyToGenerate => -1;
        public string GenerateDelay => "9.5";

        public void AddMagikePolymerizeRecipe()
        {
            PolymerizeRecipe.CreateRecipe<CrimtaneLens>(75)
                .SetMainItem<CrystalLens>()
                .AddIngredient<GlistentBar>(2)
                .AddIngredient(ItemID.TissueSample, 2)
                .Register();
        }
    }

    public class CrimtaneLensTile : BaseCostItemLensTile
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
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrimtaneLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Red);
            DustType = DustID.Crimson;
        }
    }

    public class CrimtaneLensEntity : MagikeGenerator_FromMagItem
    {
        public const int sendDelay = 60 * 9 + 30;   //血腥相比腐化的要稍微弱那么一点点
        public int sendTimer;
        public CrimtaneLensEntity() : base(150, 5 * 16, 60 * 9 + 30) { }

        public override ushort TileType => (ushort)TileType<CrimtaneLensTile>();

        public override int HowManyPerSend => 15;

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
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.Red);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.Red);
        }
    }
}
