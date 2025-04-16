using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Biomes
{
    public class CrystallineSkyIslandCloudScreen : ModProjectile, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public Player Owner => Main.player[Projectile.owner];

        public CloudData[] datas;
        private bool init = true;

        public struct CloudData
        {
            /// <summary>
            /// 目标位置
            /// </summary>
            public Vector2 CurrentScreenPos;
            /// <summary>
            /// 目标位置
            /// </summary>
            public Vector2 targetScreenPos;

            public int CloudType;

            public CloudData(int x, int y)
            {

            }

            public void Update(int timer)
            {

            }

            public void Draw(SpriteBatch spriteBatch)
            {

            }
        }

        public override void SetDefaults()
        {

        }

        public override void AI()
        {
            Projectile.Center = Owner.Center;

            if (!Projectile.IsOwnedByLocalPlayer())
                return;

            if (init)
            {
                init = false;
                Initialize();
            }
        }

        public void Initialize()
        {
            int x = Main.screenWidth / 50;
            int y = Main.screenHeight / 40;

            datas = new CloudData[x * y];
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {

                }
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
