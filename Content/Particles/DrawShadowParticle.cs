using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Particles
{
    /// <summary>
    /// 残影粒子，建议传入纯色图片
    /// </summary>
    internal class DrawShadowParticle : Particle
    {
        public override string Texture => AssetDirectory.Blank;

        public ATex tex;
        public Rectangle frame;
        public float fadeFactor = 0.96f;

        public override void AI()
        {
            Color *= fadeFactor;
            if (Color.A < 10)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            tex.Value.QuickCenteredDraw(spriteBatch, frame, Position - Main.screenPosition, Color, Rotation, Scale);
            return false;
        }

        public static void Spawn(Vector2 pos, float rot, Color c, ATex tex, Rectangle frame, float scale = 1)
        {
            if (VaultUtils.isServer)
                return;

            var p = PRTLoader.NewParticle<DrawShadowParticle>(pos, Vector2.Zero, c, scale);
            p.tex = tex;
            p.frame = frame;
        }

        public static DrawShadowParticle SpawnDirectly(Vector2 pos, float rot, Color c, ATex tex, Rectangle frame, float scale = 1, float fadeF = 0.96f)
        {
            if (VaultUtils.isServer)
                return null;

            var p = PRTLoader.CreateAndInitializePRT<DrawShadowParticle>(pos, Vector2.Zero, c, scale);
            p.Rotation = rot;
            p.tex = tex;
            p.frame = frame;
            p.fadeFactor = fadeF;
            return p;
        }
    }
}
