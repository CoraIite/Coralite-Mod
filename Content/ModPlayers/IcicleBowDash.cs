using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        public bool UsingIcicleBowDash()
        {
            if (Player.HeldItem.type != ModContent.ItemType<IcicleBow>())
                return false;

            Vector2 newVelocity = Player.velocity;
            switch (DashDir)
            {
                case DashLeft:
                case DashRight:
                    {
                        float dashDirection = DashDir == DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 10;
                        break;
                    }
                default:
                    return false;
            }

            DashDelay = 80;
            DashTimer = 20;
            Player.immuneTime = 20;
            Player.immune = true;
            Player.velocity = newVelocity;

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, Player.Center);
                for (int i = 0; i < 4; i++)//生成冰晶粒子
                {
                    Vector2 center = Player.Center + (-1.57f + i * 1.57f).ToRotationVector2() * 64;
                    Vector2 velocity = (i * 1.57f).ToRotationVector2() * 4;
                    IceStarLight.Spawn(center, velocity, 1f, () => Player.Center, 16);
                }

                for (int i = 0; i < 1000; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.ModProjectile is IcicleBowHeldProj)
                    {
                        proj.Kill();
                        break;
                    }
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<IcicleBowHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, (Main.MouseWorld - Player.Center).ToRotation(), 1);
            }

            return true;
        }
    }
}