using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Particles
{
    public class StarRot : Particle
    {
        public override string Texture => AssetDirectory.Blank;

        private int frameCounter;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 22, 26);
        }

        public override void AI()
        {
            Opacity++;
            if (Opacity < 4)
            {
                Scale *= 1.4f;
            }

            if (Opacity > 4 + 8)
            {
                Scale *= 0.9f;
                Velocity *= 0.8f;
            }

            Rotation += Math.Sign(Velocity.X) * Velocity.Length() / 14;

            if (++frameCounter > 5)
            {
                frameCounter = 0;
                Frame.Y += 26;
                if (Frame.Y > 181)
                    Frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Main.instance.LoadItem(ItemID.FallenStar);
            Texture2D mainTex = TextureAssets.Item[ItemID.FallenStar].Value;

            Vector2 position = Position - Main.screenPosition;
            spriteBatch.Draw(mainTex, position, Frame, Color, Rotation, new Vector2(11, 13)
                , Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
