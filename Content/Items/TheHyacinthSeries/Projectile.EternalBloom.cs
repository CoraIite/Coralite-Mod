using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.TheHyacinthSeries
{
    public class EternalBloomHeldProj : BaseGunHeldProj
    {
        public EternalBloomHeldProj() : base(0.2f, 16, -8, AssetDirectory.TheHyacinthSeriesItems) { }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.6f, 0.1f));
        }
    }

    public class SeedPlantera : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_275";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SeedPlantera);
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 4;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (++Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 1)
                    Projectile.frame = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.WoodFurniture);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            var pos = Projectile.Center - Main.screenPosition;
            var frameBox = mainTex.Frame(1, 2, 0, Projectile.frame);
            var origin = frameBox.Size() / 2;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, new Color(255, 200, 100, 0), Projectile.rotation + 1.57f,
                origin, Projectile.scale * 1.3f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation + 1.57f,
                origin, Projectile.scale, 0, 0);
            return false;
        }
    }

    public class ThornBall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_277";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ThornBall);
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 4;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 16)
            {
                Projectile.velocity.Y += 0.3f;
            }

            Projectile.rotation += Projectile.velocity.X / 20;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0]++;
            Projectile.netUpdate = true;

            //简易撞墙反弹
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -Math.Sign(oldVelocity.X) * oldVelX * 0.8f;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -Math.Sign(oldVelocity.Y) * oldVelY * 0.8f;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.position -= oldVelocity;
            return Projectile.ai[0] > 5;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(new Color(255, 200, 100, 0), Projectile.scale * 1.2f, 0);
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }
}
