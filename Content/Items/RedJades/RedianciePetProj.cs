using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedianciePetProj : BasePetProj
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedianciePet";
        protected override int PetBuffType => ModContent.BuffType<RedianciePetBuff>();

        protected override void SetPetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FairyQueenPet);
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active)
            {
                Projectile.Kill();
                return;
            }

            CheckActive(owner);
            Idle(owner);
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0, 0));
            if (Main.rand.NextBool(32))
            {
                int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby);
                Main.dust[index].noGravity = true;
            }
        }

        private void Idle(Player owner)
        {
            Vector2 distanceToOwner = owner.Center - Projectile.Center + new Vector2(owner.direction * 32, -48);
            float lengthToOwner = distanceToOwner.Length();

            if (lengthToOwner < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }

            TeleportToOwner(owner);

            if (Math.Abs(distanceToOwner.X) > 20f || Math.Abs(distanceToOwner.Y) > 10f)
            {
                distanceToOwner = distanceToOwner.SafeNormalize(Vector2.Zero) * 10f;
                distanceToOwner *= new Vector2(1.2f, 0.8f);
                Projectile.velocity = ((Projectile.velocity * 15f) + distanceToOwner) / 16f;
            }
            else if (Projectile.velocity.Length() > 2f)
                Projectile.velocity *= 0.97f;
        }
    }
}
