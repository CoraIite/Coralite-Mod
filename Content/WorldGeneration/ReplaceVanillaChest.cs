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
            progress.Message = "将额外的战利品塞入箱子";

            foreach (var chest in Main.chest)
            {
                if (chest == null)
                    continue;

                Tile tile = Framing.GetTileSafely(chest.x, chest.y);
                switch (tile.TileType)
                {
                    case TileID.Containers:
                        if (tile.TileFrameX == 0)//木箱子
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
                    default:
                        break;
                }
            }
        }
    }
}
