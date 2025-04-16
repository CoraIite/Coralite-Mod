using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Biomes
{
    public class CrystallineSkyIslandCloudScreen : ModProjectile, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public Player Owner => Main.player[Projectile.owner];

        public const int XCount = 50;
        public const int YCount = 30;

        public struct CloudData
        {
            public Vector2 ScreenPos;
            public Vector2 OffScreenPos;

            public int CloudType;
        }

        public override void SetDefaults()
        {
        }

        public override void AI()
        {
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (!Projectile.IsOwnedByLocalPlayer())
                return;

            //绘制云朵
        }
    }
}
