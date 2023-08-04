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
using Terraria.DataStructures;
using Coralite.Content.NPCs.Magike;

namespace Coralite.Content.WorldGeneration
{
    public class WorldGenTester : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //if (Main.myPlayer == player.whoAmI)
            //{
            //    float rot = (Main.MouseWorld - player.Center).ToRotation();
            //    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<CrystalLaser>(), 10, 0, player.whoAmI, rot);

            //}
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            Main.dayTime = true;
            Main.time = 4000;
            return base.CanUseItem(player);
        }

    }
}
