using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.Magike.Filters.DiffractionFilters;
using Coralite.Content.Items.Magike.Filters.ExcitedFilters;
using Coralite.Content.Items.Magike.Filters.InterferenceFilters;
using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Content.Items.Magike.Filters.PulseFilters;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Content.Tiles.MagikeSeries2;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText ReplaceVanillaChestText { get; set; }

        public void ReplaceVanillaChest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = ReplaceVanillaChestText.Value;// "正在将额外的战利品塞入箱子";

            bool spawnedBrithOfIce = false;//是否生成过冰雪的新生，用于放置运气太差导致的一个都没生成的情况

            foreach (var chest in Main.chest)
            {
                if (chest == null)
                    continue;

                Tile tile = Framing.GetTileSafely(chest.x, chest.y);
                switch (tile.TileType)
                {
                    case TileID.Containers:
                        switch (tile.TileFrameX)
                        {
                            default:
                            case 0://木箱子
                                {
                                    //判断不是地牢里的木箱子
                                    if (tile.WallType is not WallID.BlueDungeonUnsafe or WallID.GreenDungeonUnsafe or WallID.PinkDungeonUnsafe
                                                                   or WallID.BlueDungeonSlabUnsafe or WallID.GreenDungeonSlabUnsafe or WallID.PinkDungeonSlabUnsafe
                                                                   or WallID.BlueDungeonTileUnsafe or WallID.GreenDungeonTileUnsafe or WallID.PinkDungeonTileUnsafe)
                                    {
                                        //if (WorldGen.genRand.NextBool(10, 100))
                                        //    foreach (var item in chest.item)
                                        //        if (item.IsAir)
                                        //        {
                                        //            item.SetDefaults(ModContent.ItemType<MaintenanceFluid>());
                                        //            break;
                                        //        }
                                        if (WorldGen.genRand.NextBool(15, 100))
                                            chest.AddItem<FlyingShieldVarnish>();
                                        if (WorldGen.genRand.NextBool(15, 100))
                                            chest.AddItem<HeavyWedges>();
                                    }
                                }
                                break;
                            case 1 * 18 * 2://金箱子
                                {
                                    if (chest.y > Main.rockLayer)  //必须要是地下金箱子才行
                                    {
                                        if (WorldGen.genRand.NextBool(25, 100))
                                            chest.AddItem<Items.RedJades.HiddenRed>();
                                        if (WorldGen.genRand.NextBool(10, 100))
                                            chest.AddItem<FlyingShieldMaintenanceGuide>();
                                        if (WorldGen.genRand.NextBool(10, 100))
                                            chest.AddItem<FlyingShieldBattleGuide>();
                                    }
                                }
                                break;
                            case 8 * 18 * 2://红木箱
                            case 10 * 18 * 2://常春藤箱
                                {
                                    if (chest.y > Main.worldSurface)  //必须要是地下箱子才行
                                    {
                                        if (WorldGen.genRand.NextBool(10, 100))//放置丛林龟壳
                                            chest.AddItem<JungleTurtleShell>();
                                    }
                                }
                                break;
                            case 11 * 18 * 2:  //冰冻箱
                                {
                                    if (!spawnedBrithOfIce)
                                    {
                                        chest.AddItem<Items.Icicle.ANewBirthOfIce>();
                                        spawnedBrithOfIce = true;
                                        break;
                                    }

                                    if (Main.rand.NextBool(25, 100))
                                        chest.AddItem<Items.Icicle.ANewBirthOfIce>();
                                }
                                break;
                            case 12 * 18 * 2:  //生命木箱
                                {
                                    if (WorldGen.genRand.NextBool(5, 7))//放置生命大叶子
                                        chest.AddItem<LeafShield>();
                                }
                                break;
                        }
                        break;
                    case TileID.Containers2:
                        switch (tile.TileFrameX)
                        {
                            default:
                                break;
                            case 10 * 18 * 2://沙漠
                                if (WorldGen.genRand.NextBool(20, 100))
                                    chest.AddItem<TremblingBow>();

                                break;
                        }
                        break;
                    default:
                        if (tile.TileType == ModContent.TileType<BasaltChestTile>())
                        {
                            if (WorldGen.genRand.NextBool(3))
                                chest.AddItem<WarpMirror>();

                            for (int i = 0; i < 2; i++)//添加偏振滤镜
                                WorldGenHelper.RandChestItem(chest, ModContent.ItemType<MagicCrystalPolarizedFilter>(), WorldGen.genRand.Next(1, 2));

                            if (WorldGen.genRand.NextBool(3))//添加其他滤镜
                                WorldGenHelper.RandChestItem(chest, ModContent.ItemType<MagicCrystalDiffractionFilter>(), 1);
                            if (WorldGen.genRand.NextBool(3))//添加其他滤镜
                                WorldGenHelper.RandChestItem(chest, ModContent.ItemType<MagicCrystalExcitedFilter>(), 1);
                            if (WorldGen.genRand.NextBool(3))//添加其他滤镜
                                WorldGenHelper.RandChestItem(chest, ModContent.ItemType<MagicCrystalInterferenceFilter>(), 1);
                            if (WorldGen.genRand.NextBool(3))//添加其他滤镜
                                WorldGenHelper.RandChestItem(chest, ModContent.ItemType<MagicCrystalPulseFilter>(), 1);
                        }
                        else if (tile.TileType==ModContent.TileType<SkarnChestTile>())
                        {
                            for (int i = 0; i < 3; i++)//放隐身药水
                                WorldGenHelper.RandChestItem(chest, ItemID.InvisibilityPotion, WorldGen.genRand.Next(1, 5));
                            
                            //放偏振滤镜
                            WorldGenHelper.RandChestItem(chest, ModContent.ItemType<CrimsonPolarizedFilter>(), WorldGen.genRand.Next(1, 2));
                        }
                        break;
                }
            }
        }
    }
}
