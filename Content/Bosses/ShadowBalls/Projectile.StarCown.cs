using Coralite.Core;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class StarCrown:ModProjectile,IDrawOnShadowBall
    {
        public override string Texture => AssetDirectory.Blank;
        private ref float OwnerIndex => ref Projectile.ai[0];
        private ref float TotalTime => ref Projectile.ai[1];
        private ref float F => ref Projectile.ai[2];

        private ref float Timer => ref Projectile.localAI[2];

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ShadowBall>(out NPC owner, Projectile.Kill))
            {
                return;
            }

            Projectile.Center = owner.Center + new Vector2(0, -140);

            if (Timer < 30)
            {
                F = Timer / 30f*0.5f;
            }
            else if (Timer > TotalTime - 30)
            {
                F =0.5f+ (Timer-TotalTime+30) / 30;
            }
            if (Timer > TotalTime)
            {
                Projectile.Kill();
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void Draw()
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color lightGreen = new Color(230, 250, 151) * 0.5f;
            Color green = new Color(181, 209, 157) * 0.5f;

            for (int i = 0; i < 3; i++)
                Helper.DrawPrettyStarSparkle(1, 0, pos, lightGreen, green, F, 0, 0.5f, 0.5f, 1, 0, new Vector2(1.4f, 1) * (4 - i), Vector2.One * (2.5f - i / 3f * 1.5f));

            for (int i = 0; i < 3; i++)
                Helper.DrawPrettyStarSparkle(1, 0, pos, lightGreen, green, F, 0, 0.5f, 0.5f, 1, MathHelper.PiOver4, Vector2.One * 0.6f * (4 - i), Vector2.One * 0.6f * (3 - 1));
        }
    }
}
