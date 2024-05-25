using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowBeam : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        Vector2 originVelocity;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 25);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.extraUpdates = 4;
            Projectile.hostile = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            originVelocity = Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? CanDamage()
        {
            const int SlowTime = 40 * 5;
            const int StayTime = SlowTime + 10 * 5;

            if (Projectile.ai[0] < StayTime)
                return false;
            return base.CanDamage();
        }

        public override void AI()
        {
            const int SlowTime = 40 * 5;
            const int StayTime = SlowTime + 10 * 5;
            const int AccTime = StayTime + 20 * 5;

            Lighting.AddLight(Projectile.Center, new Vector3(0.6f, 0.1f, 0.85f));
            if (Projectile.ai[0] < SlowTime)
            {
                Projectile.velocity = Vector2.Lerp(originVelocity, Vector2.Zero, Projectile.ai[0] / SlowTime);
                //先减慢
                Projectile.ai[0]++;
            }
            else if (Projectile.ai[0] < StayTime)
            {
                Projectile.ai[0]++;//停一会
            }
            else if (Projectile.ai[0] < AccTime)
            {
                //加速
                Projectile.velocity = Vector2.Lerp(Vector2.Zero, originVelocity.SafeNormalize(Vector2.Zero) * 12,
                    (Projectile.ai[0] - StayTime) / (AccTime - StayTime));
                Projectile.ai[0]++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            Color c = lightColor;
            c.A = 0;
            Projectile.DrawShadowTrails(c, 0.8f, 0.8f / 25, 1, 25, 1);

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor * 0.9f, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
