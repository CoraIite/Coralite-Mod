using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Coralite.Content.Items.Glistent
{
    public abstract class LeafParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "SmallLeafParticle";
        public int FrameMax { get => LeafType == 0 ? 8 : 5; }
        public int frameCounter;
        public int frameCounterMax = 5;

        public int LeafType;
        public float alpha;

        public static Asset<Texture2D> BigLeaf;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            BigLeaf = ModContent.Request<Texture2D>(AssetDirectory.MagikeProjectiles + "LeafShield");
        }

        public override void Unload()
        {
            BigLeaf = null;
        }

        public void UpdateFrame()
        {
            if (++frameCounter > frameCounterMax)
            {
                frameCounter = 0;

                if (++Frame.Y > FrameMax - 1)
                    Frame.Y = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = LeafType == 0 ? TexValue : BigLeaf.Value;
            Rectangle frame = mainTex.Frame(1, FrameMax, 0, Frame.Y);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, Lighting.GetColor(Position.ToTileCoordinates()) * (Color.A / 255f) * alpha, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}
