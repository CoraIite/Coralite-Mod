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

namespace Coralite.Content.Items.Magike.Chargers
{
    public class SplendorCharger : BaseMagikePlaceableItem, IMagikeFactoryItem
    {
        public SplendorCharger() : base(TileType<SplendorChargerTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<SplendorMagicoreRarity>(), 1000, AssetDirectory.MagikeFactories)
        { }

        public override int MagikeMax => 1500;
        public string WorkTimeMax => "?";
        public string WorkCost => "25";

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<CrystalCharger>()
            //    .AddIngredient<CrystallineMagike>(5)
            //    .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
            //    .AddTile(TileID.Anvils)
            //    .Register();
        }
    }

    public class SplendorChargerTile : BaseChargerTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[2] {
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<SplendorChargerEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.SplendorMagicoreLightBlue);
            DustType = DustID.BlueFairy;
        }
    }

    public class SplendorChargerEntity : MagikeCharger
    {
        public SplendorChargerEntity() : base(1500, 25) { }

        public override ushort TileType => (ushort)TileType<SplendorChargerTile>();
        public override Color MainColor => Coralite.SplendorMagicoreLightBlue;

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 2, Position, Coralite.SplendorMagicoreLightBlue);
        }
    }
}
