using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class PremissionProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public Vector2 backLightScale;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Vector2 Scale1 = new Vector2(0.75f,4);
            switch (State)
            {
                default:
                case 0://展开
                    {
                        if (Timer < 20)
                        {
                            float factor = Timer / 20;
                            factor = Coralite.Instance.HeavySmootherInstance.Smoother(factor);
                            backLightScale = Vector2.Lerp(Vector2.Zero, Scale1, factor);
                        }

                        Timer++;
                    }
                    break;
                case 1://生成光粒子
                    {

                    }
                    break;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D backTex = CoraliteAssets.Trail.BoosterASP.Value;
            Vector2 backOrigin = new Vector2(0, backTex.Height / 2);
            Vector2 pos = Projectile.Center-Main.screenPosition;

            Color c = Coralite.CrystallinePurple;
            Main.spriteBatch.Draw(backTex, pos, null, c, -MathHelper.PiOver4, backOrigin, backLightScale, 0, 0);

            return false;
        }
    }
}
