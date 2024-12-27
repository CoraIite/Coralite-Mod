using Coralite.Core;
using Coralite.Core.Attributes;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Coralite.Content.Items.Glistent
{
    [AutoLoadTexture(Path = AssetDirectory.MagikeProjectiles)]
    public abstract class LeafParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "SmallLeafParticle";
        public int FrameMax { get => LeafType == 0 ? 8 : 5; }
        public int frameCounter;
        public int frameCounterMax = 5;

        public int LeafType;
        public float alpha;

        [AutoLoadTexture(Name = "LeafShield")]
        public static ATex BigLeaf { get; private set; }

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
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

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = LeafType == 0 ? TexValue : BigLeaf.Value;
            Rectangle frame = mainTex.Frame(1, FrameMax, 0, Frame.Y);
            Vector2 origin = frame.Size() / 2;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frame, Lighting.GetColor(Position.ToTileCoordinates()) * (Color.A / 255f) * alpha
                , Rotation, origin, Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
