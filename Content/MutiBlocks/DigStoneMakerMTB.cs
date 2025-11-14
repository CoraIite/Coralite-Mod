using Coralite.Content.Items.Magike.Factorys;
using Coralite.Content.Tiles.DigDigDig;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core.Systems.MTBStructure;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.MutiBlocks
{
    public class DigStoneMakerMTB : PreviewMultiblock
    {
        public override void SetStaticDefaults()
        {
            int CrystalBrick = ModContent.TileType<MagicCrystalBrickTile>();
            int HardBasalt = ModContent.TileType<HardBasaltTile>();
            int Basalt = ModContent.TileType<BasaltTile>();
            int Core = ModContent.TileType<StoneMakerCoreTile>();

            //{cb,-1,cb },
            //{b,core,b },
            //{h, h, h }

            AddTiles((-1, -1, CrystalBrick), (1, -1, CrystalBrick));
            AddTiles((-1, 0, Basalt), (0,0, Core), (1, 0, Basalt));
            AddTiles((-1, 1, HardBasalt), (0,1, HardBasalt), (1, 1, HardBasalt));

        }

        public override void OnSuccess(Point16 origin)
        {
            base.OnSuccess(origin);

            KillAll(origin);

            for (int i = 0; i < 2; i++)
            {
                Tile t = Framing.GetTileSafely(origin + new Point16(i - 1, 2));
                if (!Helper.HasReallySolidTile(t))//如果没有实心物块就掉落出来
                    goto dropItem;
            }

            int tileType = ModContent.TileType<StoneMakerTile>();
            WorldGen.PlaceTile(origin.X-1, origin.Y + 1, tileType);

            Tile t2 = Framing.GetTileSafely(origin+new Point16(-1,1));
            if (t2.TileType != tileType)//放置失败，生成物品
                goto dropItem;

            return;
        dropItem:
            Helper.SpawnItemTileBreakNet<StoneMaker >(origin);
        }
    }
}
