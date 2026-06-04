using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class GelSpike : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;

            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 14)
                Projectile.velocity.Y = 14;

            //粒子
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Alpha: 150, newColor: new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 1.4f));
            dust.noGravity = true;
            dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                       -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.1f, 0.3f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            if (Main.zenithWorld)
                lightColor = SlimeEmperor.BlackSlimeColor;

            Rectangle frame = mainTex.Frame(1, 2, 0, 0);
            var origin = frame.Size() / 2;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, 0.8f, 0, 0);

            frame = mainTex.Frame(1, 2, 0, 1);
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f);
            Color color = new Color(50, 152 + (int)(100 * factor), 225);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frame, color, Projectile.rotation, origin, 0.8f, 0, 0);

            return false;
        }
    }
}
