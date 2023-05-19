using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Items.YujianHulu
{
    /// <summary>
    /// 只是一个用于造成伤害的弹幕罢了
    /// 使用velocity控制速度，ai[0]控制持续时间
    /// </summary>
    public class SpurtProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float maxTime => ref Projectile.ai[0];
        public ref float Width => ref Projectile.ai[1];

        public Vector2 center;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            center = Projectile.Center;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localNPCHitCooldown = (int)maxTime;
                Projectile.timeLeft = (int)maxTime;
                Projectile.localAI[0] = 1;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center, Projectile.Center, Width, ref a);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}