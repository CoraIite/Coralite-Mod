using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MagikeSystem.Particles
{
    public class TileRenewalController : Particle
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public Point16 topLeft;
        public int startY;
        public int frameDelay = 6;

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            ShouldKillWhenOffScreen = false;
        }

        public static void Spawn(Point16 topLeft, Color color)
        {
            if (Main.dedServ)
            {
                return;
            }
            TileRenewalController particle = PRTLoader.NewParticle<TileRenewalController>(topLeft.ToWorldCoordinates(), Vector2.Zero, color, 1);
            particle.topLeft = topLeft;
            particle.startY = topLeft.Y;
        }

        public override void AI()
        {
            Tile t = Framing.GetTileSafely(topLeft);
            if (!t.HasTile)
            {
                active = false;
                return;
            }

            MagikeHelper.GetMagikeAlternateData(topLeft.X, topLeft.Y, out TileObjectData altermateData, out _);

            Opacity++;
            if (Opacity > frameDelay)
            {
                Opacity = 0;

                int x = topLeft.X;
                int y = startY;

                Rectangle tileRect = new(topLeft.X, topLeft.Y, altermateData.Width, altermateData.Height);

                bool spawn = false;

                for (int k = 0; k < altermateData.Width; k++)
                {
                    if (tileRect.Contains(x, y))//生成额外粒子
                    {
                        PRTLoader.NewParticle<TileHightlight>(new Point(x, y).ToWorldCoordinates(), Vector2.Zero, Color);
                        spawn = true;
                    }

                    x++;
                    y--;
                }

                if (!spawn)
                {
                    active = false;
                    return;
                }

                startY++;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => false;

        public override void DrawInUI(SpriteBatch spriteBatch) { }
    }
}
