using Coralite.Core.Systems.Trails;
using InnoVault.PRT;

namespace Coralite.Core.Systems.ParticleSystem
{
    public abstract class TrailParticle : BasePRT, IDrawParticlePrimitive
    {
        public Trail trail;

        public virtual void DrawPrimitive() { }
    }
}
