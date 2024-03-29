﻿using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowEnergy : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            //ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Bone;
            ItemID.Sets.ShimmerTransformToItem[ItemID.Bone] = Type;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 0, 5, 50);
            Item.maxStack = 999;
            Item.height = Item.width = 40;
        }
    }
}
