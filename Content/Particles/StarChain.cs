using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    /// <summary>
    /// 星星链条粒子，设置<see cref="ChainedParticle"/>来添加线条连接
    /// </summary>
    public class StarChain : Particle
    {
        public override string Texture => "Terraria/Images/Projectile_16";

        public Particle ChainedParticle = null;
        public int ShineTime = 10;
        public float LineWidth = 6;
        public int FadeTime = 6;
        public float TargetScale = 1;
        public float Alpha = 1;

        public Player FollowPlayer = null;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Scale = 0.01f;
        }

        public override void AI()
        {
            Opacity++;
            if (ChainedParticle != null && !ChainedParticle.active)
                ChainedParticle = null;
            if (FollowPlayer != null)
                Position += FollowPlayer.position - FollowPlayer.oldPosition;

            Lighting.AddLight(Position, Color.ToVector3() / 2 * Scale);

            if (Opacity < FadeTime)
            {
                float f = Helper.SqrtEase(Opacity / FadeTime);
                Scale = Helper.Lerp(0.01f, TargetScale, f);
            }
            else if (Opacity < FadeTime + ShineTime)
            {

            }
            else if (Opacity < ShineTime + FadeTime * 2)
            {
                float f = Helper.SqrtEase((Opacity - FadeTime - ShineTime) / FadeTime);
                Scale = Helper.Lerp(TargetScale, 0.01f, f);
            }
            else
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            //绘制连线
            Vector2 pos = Position - Main.screenPosition;
            if (ChainedParticle != null)
            {
                Texture2D lineTex = CoraliteAssets.Sparkle.ShotLineSPA2.Value;
                Vector2 dir = ChainedParticle.Position - Position;
                float rotation = dir.ToRotation();
                Vector2 scale = new(dir.Length() / lineTex.Width, LineWidth * Scale / lineTex.Height);
                Vector2 origin = new(0, lineTex.Height / 2);

                spriteBatch.Draw(lineTex, pos, null, Color* Alpha, rotation, origin, scale, 0, 0);
                scale.Y *= 0.7f;
                spriteBatch.Draw(lineTex, pos, null, (Color with { A = 0 }) * 0.5f * Alpha, rotation, origin, scale, 0, 0);
            }

            TexValue.QuickCenteredDraw(spriteBatch, pos, Color.White * Alpha, scale: Scale);
            return false;
        }
    }
}
