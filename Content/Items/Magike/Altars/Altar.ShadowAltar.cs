using Coralite.Content.Items.Shadow;
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

namespace Coralite.Content.Items.Magike.Altars
{
    public class ShadowAltar : BaseMagikePlaceableItem, IMagikeSenderItem, IMagikeFactoryItem
    {
        public ShadowAltar() : base(TileType<ShadowAltarTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeAltars)
        { }

        public override int MagikeMax => 450;
        public string SendDelay => "20";
        public int HowManyPerSend => 1;
        public int ConnectLengthMax => 8;
        public int HowManyCanConnect => 5;
        public string WorkTimeMax => "3.5 × 次要物品数量";
        public string WorkCost => "?";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlistentAltar>()
                .AddIngredient<ShadowEnergy>(5)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentAltar>()
                .AddIngredient(ItemID.Bone)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentAltar>()
                .AddIngredient<ShadowCrystal>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ShadowAltarTile : BaseAltarTile
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<ShadowAltarEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Purple);
            DustType = DustID.PurpleTorch;
        }
    }

    public class ShadowAltarEntity : MagikeFactory_PolymerizeAltar
    {
        public ShadowAltarEntity() : base(450, 60 * 3 + 30, 16 * 8, 5) { }

        public override Color MainColor => Color.GreenYellow;

        public override ushort TileType => (ushort)TileType<ShadowAltarTile>();

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 2, Position, Color.Purple);
        }
    }
}
