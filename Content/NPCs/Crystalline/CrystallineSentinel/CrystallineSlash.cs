using Coralite.Core;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 刀光闪片：出生 4 帧后开始拉长变窄并淡出，36 帧上限。挥刀与旋风斩共用。
    /// </summary>
    public class CrystallineSlash : Particle
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public ref float Alpha => ref ai[0];
        public ref float ScaleY => ref ai[1];

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Frame.X = Main.rand.Next(0, 2);
            Rotation = Velocity.ToRotation() + 1.57f;
            Color = Color.White;
            Alpha = 1;
        }

        public override void AI()
        {
            Opacity++;
            Lighting.AddLight(Position, Color.ToVector3() * 0.3f);

            if (Opacity > 4)
            {
                Velocity *= 0.98f;
                ScaleY += 0.1f;
                Scale -= 0.02f;
                Alpha = Utils.MultiLerp((Opacity - 4) / 20, 1f, 0.9f, 0.8f, 0.6f, 0.3f, 0f);
            }

            if (Opacity > 36 || Alpha < 0.01f)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TexValue;
            Rectangle frameBox = mainTex.Frame(3, 1, Frame.X, 0);
            Vector2 origin = frameBox.Size() / 2;
            Vector2 scale = new(Scale, ScaleY);
            spriteBatch.Draw(mainTex, Position - Main.screenPosition, frameBox, Color.White * Alpha, Rotation, origin, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
