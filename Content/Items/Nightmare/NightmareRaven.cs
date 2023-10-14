using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Nightmare
{
    public class NightmareRaven:ModProjectile
    {
        public override string Texture => AssetDirectory.NightmarePlantera+ "NightmareCrow";

        public Color drawColor;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Rectangle frameBox = mainTex.Frame(1, 5, 0, Projectile.frame);
            Vector2 origin = frameBox.Size() / 2;
            SpriteEffects effect = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //绘制残影
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 0; i < 10; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    drawColor * (0.5f - i * 0.05f), Projectile.oldRot[i], frameBox.Size() / 2, 1, effect, 0);

            //向上下左右四个方向绘制一遍
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (i * MathHelper.PiOver2).ToRotationVector2() * 4, frameBox, drawColor, Projectile.rotation, origin, 1,
                   effect, 0);
            }

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.White, Projectile.rotation, origin, 1, effect, 0);
            return false;
        }
    }
}
