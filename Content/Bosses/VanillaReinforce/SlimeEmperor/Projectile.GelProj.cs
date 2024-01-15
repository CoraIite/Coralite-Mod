using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class GelProj : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1800;

            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 1)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.localAI[0] += 0.1f;
                if (Projectile.localAI[0] > 1)
                    Projectile.localAI[0] = 1f;
            }

            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.t_Slime, Alpha: 150, newColor: new Color(78, 136, 255, 80), Scale: Main.rand.NextFloat(1f, 1.4f));
            dust.noGravity = true;
            dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 4)
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            var frameBox = mainTex.Frame(1, 5, 0, Projectile.frame);
            Color color = lightColor * Projectile.localAI[0];
            if (Main.zenithWorld)
                lightColor = SlimeEmperor.BlackSlimeColor;

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }
    }
}
