using Coralite.Content.Items.Glistent;
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

namespace Coralite.Content.Items.Magike.Altars
{
    public class GlistentAltar : BaseMagikePlaceableItem, IMagikeSenderItem, IMagikeFactoryItem
    {
        public GlistentAltar() : base(TileType<GlistentAltarTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeAltars)
        { }

        public override int MagikeMax => 300;
        public string SendDelay => "20";
        public int HowManyPerSend => 1;
        public int ConnectLengthMax => 8;
        public int HowManyCanConnect => 4;
        public string WorkTimeMax => "3.5 × 次要物品数量";
        public string WorkCost => "?";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystalAltar>()
                .AddIngredient<GlistentBar>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GlistentAltarTile : BaseAltarTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[2] {
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<GlistentAltarEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.GreenYellow);
            DustType = DustID.GreenTorch;
        }
    }

    public class GlistentAltarEntity : MagikeFactory_PolymerizeAltar
    {
        public GlistentAltarEntity() : base(300, 60 * 3 + 30, 16 * 8, 4) { }

        public override Color MainColor => Color.GreenYellow;

        public override ushort TileType => (ushort)TileType<GlistentAltarTile>();

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 2, Position, Color.GreenYellow);
        }
    }
}
