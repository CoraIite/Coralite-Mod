﻿using Coralite.Content.Items.DigDigDig.EyeOfGlistent;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Tiles.DigDigDig
{
    public class GlistentRelicTile : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.DigDigDigTiles + "GlistentRelicPedestal";
        public override string RelicTextureName => AssetDirectory.GlistentItems + "GlistentBarFlip";

        public override bool CanDrop(int i, int j) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<GlistentRelic>())
            ];
        }
    }
}