﻿using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class ShlimmerMushrooms : BaseMaterial
    {
        public ShlimmerMushrooms() : base(9999, Item.sellPrice(0, 0, 0, 50), ItemRarityID.Green, AssetDirectory.Materials) { }
    }
}
