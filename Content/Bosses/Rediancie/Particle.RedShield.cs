using Coralite.Core;
using Coralite.Core.Attributes;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;

namespace Coralite.Content.Bosses.Rediancie
{
    [VaultLoaden(AssetDirectory.Rediancie)]
    public class RedShield : Particle
    {
        public override string Texture => AssetDirectory.Rediancie + "RedShield";

        [VaultLoaden("{@classPath}" + "RedShield_Flow")]
        public static ATex FlowTex { get; private set; }

        private Entity rediancie;
        private bool toFadeOut = false;
        private bool Init = true;

        public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            Color = Coralite.RedJadeRed;
            Color.A = 0;
            Rotation = Main.rand.NextFloat(6.282f);
            Scale = 0f;
            ShouldKillWhenOffScreen = false;
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Rotation += 0.06f;

            if (Init)
            {
                Scale += 0.05f;
                Color.A += 255 / 16;
                if (Scale > 0.8f)
                {
                    Scale = 0.8f;
                    Color.A = 255;
                    Init = false;
                }
            }

            if (rediancie != null)
                Position = rediancie.Center;

            Opacity--;

            if (Opacity < 0)
                toFadeOut = true;

            if (toFadeOut)
            {
                Color.A -= 255 / 16;
                if (Color.A < 10)
                    active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Vector2 center = this.Position - Main.screenPosition;
            Texture2D mainTex = TexValue;
            spriteBatch.Draw(mainTex, center, null, Color, Rotation, mainTex.Size() / 2, Scale, SpriteEffects.None, 0);

            float extraRot1 = Rotation + (Opacity * 0.1f);
            float extraRot2 = Rotation + (Opacity * 0.05f);
            Vector2 flowOrigin = FlowTex.Size() / 2;

            spriteBatch.Draw(FlowTex.Value, center, null, Color, extraRot1, flowOrigin, Scale - 0.1f, SpriteEffects.None, 0);
            spriteBatch.Draw(FlowTex.Value, center, null, new Color(255, 255, 255, Color.A * 3 / 4), extraRot1 + extraRot2, flowOrigin, Scale - 0.2f, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(FlowTex.Value, center, null, Color, extraRot2 + 3.141f, flowOrigin, Scale - 0.2f, SpriteEffects.FlipHorizontally, 0);
            return false;
        }

        public static void Spawn(Entity rediancie, int maxTime)
        {
            RedShield particle = PRTLoader.NewParticle<RedShield>(rediancie.Center, Vector2.Zero);
            particle.rediancie = rediancie;
            particle.Opacity = maxTime;
        }

        public static void HanderKill()
        {
            int type = CoraliteContent.ParticleType<RedShield>();
            foreach (var particle in PRTLoader.PRT_InGame_World_Inds.Where(p => p.active && p.ID == type))
            {
                particle.Opacity = -1;
            }
        }
    }
}
