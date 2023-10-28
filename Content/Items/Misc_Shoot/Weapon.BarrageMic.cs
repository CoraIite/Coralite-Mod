using Coralite.Content.Projectiles.Projectiles_Shoot;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Misc_Shoot
{
    /// <summary>
    /// 弹幕麦克风，射出“弹幕”
    /// </summary>
    public class BarrageMic : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.shoot = ModContent.ProjectileType<BarrageMicProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int height = Main.screenHeight / 2;
            Projectile.NewProjectile(source, player.Center + new Vector2(Main.screenWidth / 2, Main.rand.Next(-height, height)), new Vector2(-Main.rand.Next(8, 15), 0), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
