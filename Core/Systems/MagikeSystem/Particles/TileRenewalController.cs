using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MagikeSystem.Particles
{
    public class TileRenewalController:Particle
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public Point16 topLeft;
        public int startY;
        public int frameDelay = 6;

        public override bool ShouldUpdateCenter() => false;

        public override void OnSpawn()
        {
            shouldKilledOutScreen = false;
        }

        public static TileRenewalController Spawn(Point16 topLeft,Color color)
        {
            TileRenewalController particle = NewParticle<TileRenewalController>(topLeft.ToWorldCoordinates(), Vector2.Zero, color, 1);
            particle.topLeft = topLeft;
            particle.startY = topLeft.Y;

            return particle;
        }

        public override void Update()
        {
            Tile t = Framing.GetTileSafely(topLeft);
            if (!t.HasTile)
            {
                active = false;
                return;
            }

            MagikeHelper.GetMagikeAlternateData(topLeft.X, topLeft.Y, out TileObjectData altermateData, out _);

            fadeIn++;
            if (fadeIn>frameDelay)
            {
                fadeIn = 0;

                int x= topLeft.X;
                int y = startY;

                Rectangle tileRect = new Rectangle(topLeft.X, topLeft.Y, altermateData.Width, altermateData.Height);

                bool spawn=false;

                for (int k = 0; k < altermateData.Width; k++)
                {
                    if (tileRect.Contains(x, y))//生成额外粒子
                    {
                        NewParticle<TileHightlight>(new Point(x, y).ToWorldCoordinates(), Vector2.Zero, color);
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

        public override void Draw(SpriteBatch spriteBatch) { }

        public override void DrawInUI(SpriteBatch spriteBatch) { }
    }
}
