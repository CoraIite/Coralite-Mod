using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        public bool UsingIcicleCoccyxDash()
        {
            if (Player.HeldItem.type != ModContent.ItemType<IcicleCoccyx>())
                return false;
            if (!(Player.HeldItem.ModItem as IcicleCoccyx).canDash)
                return false;

            (Player.HeldItem.ModItem as IcicleCoccyx).canDash = false;
            switch (DashDir)
            {
                case DashLeft:
                case DashRight:
                        break;
                default:
                    return false;
            }

            DashDelay = 80;
            DashTimer = 20;
            Player.immuneTime = 20;
            Player.immune = true;

            if (Player.heldProj>0&&Main.projectile[Player.heldProj].active && Main.projectile[Player.heldProj].owner == Player.whoAmI)
                Main.projectile[Player.heldProj].Kill();

            if (Player.whoAmI == Main.myPlayer)
            {
                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<IcicleCoccyx_Ex3>(),
                    Player.HeldItem.damage*2, Player.HeldItem.knockBack, Player.whoAmI, DashDir);
            }

            return true;
        }

    }
}
