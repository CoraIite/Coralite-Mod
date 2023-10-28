using Coralite.Content.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
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
                                        if (Main.rand.NextBool(5, 100))
                                            foreach (var item in chest.item)
                                                if (item.IsAir)
                                                {
                                                    item.SetDefaults(ModContent.ItemType<MaintenanceFluid>());
                                                    break;
                                                }
                                    }

                                }
                                break;
                            case 1 * 18 * 2://金箱子
                                {
                                    if (chest.y > Main.worldSurface)  //必须要是地下金箱子才行
                                    {
                                        if (Main.rand.NextBool(25, 100))
                                            foreach (var item in chest.item)
                                                if (item.IsAir)
                                                {
                                                    item.SetDefaults(ModContent.ItemType<Items.RedJades.HiddenRed>());
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
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
