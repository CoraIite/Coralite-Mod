using Coralite.Helpers;

namespace Coralite.Core.Prefabs.Projectiles
{
    public interface ISmoother
    {
        void ReCalculate(int maxTime);

        float Smoother(int timer, int maxTime);
    }

    public class NoSmoother : ISmoother
    {
        public void ReCalculate(int maxTime) { }

        public float Smoother(int timer, int maxTime)
        {
            return (float)timer / maxTime;
        }
    }

    public class BezierEaseSmoother : ISmoother
    {
        public int halfTime;

        public void ReCalculate(int maxTime)
        {
            halfTime = maxTime / 2;
        }

        public float Smoother(int timer, int maxTime)
        {
            return Helper.BezierEase((float)timer / maxTime);
        }
    }
}
