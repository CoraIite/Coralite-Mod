using InnoVault.PRT;

namespace Coralite.Core.Systems.ParticleSystem
{
    public class PrimitivePRTGroup : PRTGroup
    {
        public void DrawPrimitive()
        {
            foreach (var particle in _particles)
                if (particle.active && particle is IDrawParticlePrimitive p)
                    p.DrawPrimitive();
        }
    }
}
