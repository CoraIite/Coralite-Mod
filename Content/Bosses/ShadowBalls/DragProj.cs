using Coralite.Core;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class DragProj:ModProjectile,IDrawPrimitive,IDrawWarp
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float NpcIndex => ref Projectile.ai[0];

        public override bool ShouldUpdatePosition() => false;

        public override bool PreDraw(ref Color lightColor) => false;

        public override bool PreDrawExtras()
        {
            CoraliteAssets.LightBall.Ball.Value.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition, Color.White, 0, 0.3f);

            return false;
        }

        public void DrawPrimitives()
        {
        }

        public void DrawWarp()
        {
        }
    }
}
