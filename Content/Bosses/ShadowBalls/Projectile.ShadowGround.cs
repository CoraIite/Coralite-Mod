using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowGround:ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowBalls+Name;

        protected ref float OwnerIndex => ref Projectile.ai[0];
        protected ref float Timer => ref Projectile.ai[1];
        protected ref float Alpha => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!GetOwner(out NPC npc))
            {
                Projectile.Kill();
                return;
            }

            if (Timer<8)
            {
                Alpha += 1 / 8f;
            }     
        }

        public bool GetOwner(out NPC owner)
        {
            if (!Main.npc.IndexInRange((int)OwnerIndex))
            {
                Projectile.Kill();
                owner = null;
                return false;
            }

            NPC npc = Main.npc[(int)OwnerIndex];
            if (!npc.active || npc.type != ModContent.NPCType<SmallShadowBall>())
            {
                Projectile.Kill();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
