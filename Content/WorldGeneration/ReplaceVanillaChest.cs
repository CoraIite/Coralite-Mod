using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.Magike;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.Misc_Shoot;
using Coralite.Content.Tiles.MagikeSeries1;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public void ReplaceVanillaChest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "正在将额外的战利品塞入箱子";

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
                                        if (WorldGen.genRand.NextBool(10, 100))
                                            foreach (var item in chest.item)
                                                if (item.IsAir)
                                                {
                                                    item.SetDefaults(ModContent.ItemType<FlyingShieldVarnish>());
                                                    break;
                                                }
                                        if (WorldGen.genRand.NextBool(10, 100)
                                            && chest.y < Main.worldSurface)
                                            foreach (var item in chest.item)
                                                if (item.IsAir)
                                                {
                                                    item.SetDefaults(ModContent.ItemType<HeavyWedges>());
                                                    break;
                                                }
                                    }
                                }
                                break;
                            case 1 * 18 * 2://金箱子
                                {
                                    if (chest.y > Main.worldSurface)  //必须要是地下金箱子才行
                                    {
                                        if (WorldGen.genRand.NextBool(25, 100))
                                            foreach (var item in chest.item)
                                                if (item.IsAir)
                                                {
                                                    item.SetDefaults(ModContent.ItemType<Items.RedJades.HiddenRed>());
                                                    break;
                                                }
                                        if (WorldGen.genRand.NextBool(10, 100))
                                            foreach (var item in chest.item)
                                                if (item.IsAir)
                                                {
                                                    item.SetDefaults(ModContent.ItemType<FlyingShieldMaintenanceGuide>());
                                                    break;
                                                }
                                        if (WorldGen.genRand.NextBool(10, 100))
                                            foreach (var item in chest.item)
                                                if (item.IsAir)
                                                {
                                                    item.SetDefaults(ModContent.ItemType<FlyingShieldBattleGuide>());
                                                    break;
                                                }
                                    }
                                }
                                break;
                            case 11 * 18 * 2:  //冰冻箱
                                {
                                    if (!spawnedBrithOfIce)
                                    {
                                        foreach (var item in chest.item)
                                            if (item.IsAir)
                                            {
                                                item.SetDefaults(ModContent.ItemType<Items.Icicle.ANewBirthOfIce>());
                                                break;
                                            }
                                        spawnedBrithOfIce = true;
                                        break;
                                    }

                                    if (Main.rand.NextBool(25, 100))
                                        foreach (var item in chest.item)
                                            if (item.IsAir)
                                            {
                                                item.SetDefaults(ModContent.ItemType<Items.Icicle.ANewBirthOfIce>());
                                                break;
                                            }
                                }
                                break;
                            case 12 * 18 * 2:  //生命木箱
                                {
                                    if (chest.y > Main.worldSurface)  //必须要是地下箱子才行
                                    {
                                        if (WorldGen.genRand.NextBool(10, 100))//放置丛林龟壳
                                            foreach (var item in chest.item)
                                                if (item.IsAir)
                                                {
                                                    item.SetDefaults(ModContent.ItemType<JungleTurtleShell>());
                                                    break;
                                                }
                                    }
                                }
                                break;
                        }
                        break;
                    case TileID.Containers2:
                        switch (tile.TileFrameX)
                        {
                            default:
                                break;
                            case 10*18*2://沙漠
                                if (WorldGen.genRand.NextBool(20, 100))
                                    foreach (var item in chest.item)
                                        if (item.IsAir)
                                        {
                                            item.SetDefaults(ModContent.ItemType<TremblingBow>());
                                            break;
                                        }

                                break;
                        }
                        break;
                    default:
                        if (tile.TileType == ModContent.TileType<BasaltChestTile>())
                        {
                            int which = 1;
                            for (int i = 0; i < 4; i++)//究极偷懒写法，不建议学
                            {
                                foreach (var item in chest.item)
                                    if (item.IsAir)
                                    {
                                        item.SetDefaults(ModContent.ItemType<MagikeNote1>());
                                        if (item.ModItem is MagikeNote1 note1)
                                        {
                                            note1.RandomKnowledge(which);
                                            if (which > 0)
                                            {
                                                which++;
                                                if (which > 7)
                                                    which = -1;
                                            }
                                        }
                                        break;
                                    }
                                if (WorldGen.genRand.NextBool(1, 3))
                                    break;
                            }

                            if (WorldGen.genRand.NextBool(3))
                            {
                                foreach (var item in chest.item)
                                    if (item.IsAir)
                                    {
                                        item.SetDefaults(ModContent.ItemType<WarpMirror>());
                                        break;
                                    }
                            }
                        }
                        break;
                }
            }
        }
    }
}
