using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public class GelSpike : ModProjectile
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.velocity.Y += 0.04f;
            if (Projectile.velocity.Y>14)
                Projectile.velocity.Y = 14;

            //粒子

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            var origin = mainTex.Size() / 2;

           Main. spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, 0, 0);
            return false;
        }
    }
}
