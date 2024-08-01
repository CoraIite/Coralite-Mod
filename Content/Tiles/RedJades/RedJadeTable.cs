﻿using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.RedJades
{
    public class RedJadeTable : ModTile
    {
        public override string Texture => AssetDirectory.RedJadeTiles + Name;

        public override void SetStaticDefaults()
        {
            this.TablePrefab(DustID.GemRuby, Coralite.RedJadeRed);
        }

        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
