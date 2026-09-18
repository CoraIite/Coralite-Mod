using InnoVault.Vectors;

namespace Coralite.Core.Systems.ParticleSystem
{
    public abstract class TrailParticle : Particle, IDrawParticlePrimitive
    {
        public StrokeStyle trailStyle;

        public virtual void DrawPrimitive() { }
    }
}
