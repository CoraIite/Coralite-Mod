using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class RedianciePetProj : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedianciePet";

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FairyQueenPet);
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];

            if (!Owner.active)
            {
                Projectile.Kill();
                return;
            }

            CheckActive(Owner);
            Idle(Owner);

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0, 0));
            if (Main.rand.NextBool(32))
            {
                int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby);
                Main.dust[index].noGravity = true;
            }
        }

        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(ModContent.BuffType<RedianciePetBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }

        private void Idle(Player Owner)
        {
            float _10 = 10f;

            Vector2 Center = Projectile.Center;
            Vector2 DistanceToOwner = Owner.Center - Center + new Vector2(Owner.direction * 32, -48);

            float LenthToOwner = DistanceToOwner.Length();

            if (LenthToOwner < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }

            if (LenthToOwner > 2000f)//距离过远直接传送
                Projectile.Center = Owner.Center;

            if (Math.Abs(DistanceToOwner.X) > 20f || Math.Abs(DistanceToOwner.Y) > 10f)//距离玩家有一定距离时候
            {
                DistanceToOwner = DistanceToOwner.SafeNormalize(Vector2.Zero);
                DistanceToOwner *= _10;
                DistanceToOwner *= new Vector2(1.2f, 0.8f);
                Projectile.velocity = (Projectile.velocity * 15f + DistanceToOwner) / 16f;
            }
            else if (Projectile.velocity.Length() > 2)//距离玩家近时候
            {
                Projectile.velocity *= 0.97f;
            }

        }

    }
}
