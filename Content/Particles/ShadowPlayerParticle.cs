using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    public class ShadowPlayerParticle : Particle
    {
        public override string Texture => AssetDirectory.Blank;

        public Player drawPlayer;
        public float alpha;

        public override void SetProperty()
        {
            base.SetProperty();
        }

        public override void AI()
        {
            Opacity++;

            alpha *= 1.05f;
            if (alpha > 1 || Opacity > 30)
            {
                active = false;
            }

            if (drawPlayer != null)
            {
                drawPlayer.itemTime = drawPlayer.itemAnimation = 2;
                drawPlayer.UpdateDyes();
                drawPlayer.UpdateSocialShadow();
                drawPlayer.PlayerFrame();
            }
        }

        public void SpawnPlayer(Player player)
        {
            drawPlayer = new Player
            {
                hair = player.hair,
                skinColor = player.skinColor,
                skinVariant = player.skinVariant,
                Male = player.Male,
                eyeColor = Color,
                hairColor = Color,
                hairDyeColor = Color,
                pantsColor = Color,
                shirtColor = Color,
                shoeColor = Color,
                underShirtColor = Color,
            };

            for (int i = 0; i < 10; i++)
            {
                drawPlayer.armor[i] = player.armor[i];//装备
                drawPlayer.armor[i + 10] = player.armor[i + 10];//外观装备
                drawPlayer.dye[i] = player.dye[i];
            }

            drawPlayer.ResetVisibleAccessories();
            drawPlayer.direction = player.direction;
            drawPlayer.itemRotation = player.itemRotation;

        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            if (drawPlayer == null)
            {
                return false;
            }

            Main.PlayerRenderer.DrawPlayer(Main.Camera, drawPlayer, Position, Rotation, drawPlayer.fullRotationOrigin, alpha, Scale);
            return false;
        }
    }
}
