using Coralite.Content.ModPlayers;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Projectiles.Globals
{
    public class CoraliteGlobalProjectile : GlobalProjectile
    {
        public bool isBossProjectile;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is NPC npc && npc.boss)
                    isBossProjectile = true;
            }

            if (source is EntitySource_ItemUse_WithAmmo itemUse
                && itemUse.Item.DamageType == DamageClass.Ranged
                && projectile.type is not (ProjectileID.VortexBeater or ProjectileID.Phantasm))
            {
                if (projectile.ModProjectile is not null && projectile.ModProjectile.Mod.Name != "Coralite" && projectile.ModProjectile.Mod is not ICoralite)
                    return;

                if (itemUse.Player.TryGetModPlayer(out CoralitePlayer cp))
                {
                    if (cp.split == 2)
                    {
                        projectile.damage = (int)(projectile.damage * 0.65f);
                        projectile.NewProjectileFromThis(projectile.Center, projectile.velocity.RotatedBy(0.1f), projectile.type,
                           (int)(projectile.damage * 0.65f), projectile.knockBack / 2, projectile.ai[0], projectile.ai[1], projectile.ai[2]);
                    }
                    else if (cp.split > 2)
                    {
                        projectile.damage = (int)(projectile.damage * 0.55f);
                        projectile.NewProjectileFromThis(projectile.Center, projectile.velocity.RotatedBy(0.1f), projectile.type,
                           (int)(projectile.damage * 0.5f), projectile.knockBack / 2, projectile.ai[0], projectile.ai[1], projectile.ai[2]);
                        projectile.NewProjectileFromThis(projectile.Center, projectile.velocity.RotatedBy(-0.1f), projectile.type,
                           (int)(projectile.damage * 0.5f), projectile.knockBack / 2, projectile.ai[0], projectile.ai[1], projectile.ai[2]);
                    }
                }
            }
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (CoraliteWorld.coralCatWorld)
            {
                switch (projectile.type)
                {
                    default:
                        break;
                    case ProjectileID.Boulder:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "CatBoulder").Value;
                            Main.EntitySpriteDraw(mainTex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, mainTex.Size()/2, projectile.scale, 0, 0);
                        }
                        return false;
                    case ProjectileID.BouncyBoulder:
                        {
                            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "RainbowCatBoulder").Value;
                            Main.EntitySpriteDraw(mainTex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, mainTex.Size() / 2, projectile.scale, 0, 0);
                        }
                        return false;
                }
            }
            return base.PreDraw(projectile, ref lightColor);
        }
    }
}
