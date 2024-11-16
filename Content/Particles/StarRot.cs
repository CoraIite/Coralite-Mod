using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
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

        public override void OnSpawn()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = new Rectangle(0, 0, 22, 26);
        }

        public override void Update()
        {
            fadeIn++;
            if (fadeIn < 4)
            {
                Scale *= 1.4f;
            }

            if (fadeIn > 4 + 8)
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            Main.instance.LoadItem(ItemID.FallenStar);
            Texture2D mainTex = TextureAssets.Item[ItemID.FallenStar].Value;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, Frame, color, Rotation, new Vector2(11, 13), Scale, SpriteEffects.None, 0f);
        }
    }
}
