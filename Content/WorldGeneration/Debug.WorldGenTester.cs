using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using Coralite.Content.Tiles.Magike;
using Conditions = Terraria.WorldBuilding.Conditions;
using Terraria.GameContent.Generation;
using Coralite.Content.Items.Magike;
using Terraria.ObjectData;
using Coralite.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Threading.Tasks;
using Coralite.Content.WorldGeneration.Generators;

namespace Coralite.Content.WorldGeneration
{
    public class WorldGenTester : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool CanUseItem(Player player)
        {

            return base.CanUseItem(player);
        }

    }
}
