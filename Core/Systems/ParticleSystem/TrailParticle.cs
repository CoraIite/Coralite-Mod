using Coralite.Core.Systems.Trails;

namespace Coralite.Core.Systems.ParticleSystem
{
    public abstract class TrailParticle : Particle, IDrawParticlePrimitive
    {
        public Trail trail;

        public virtual void DrawPrimitives() { }
    }
}
