using Coralite.Helpers;

namespace Coralite.Core.Prefabs.Projectiles
{
    public interface ISmoother
    {
        void ReCalculate(int minTime, int maxTime);

        float Smoother(int timer, int minTime, int maxTime);
    }

    public class NoSmoother : ISmoother
    {
        public void ReCalculate(int minTime, int maxTime) { }

        public float Smoother(int timer, int minTime, int maxTime)
        {
            return 1f;
        }
    }

    public class BezierEaseSmoother : ISmoother
    {
        public int halfTime;

        public void ReCalculate(int minTime, int maxTime)
        {
            halfTime = (maxTime - minTime) / 2;
        }

        public float Smoother(int timer, int minTime, int maxTime)
        {
            float factor;
            if (timer <= halfTime)
                factor = (timer - minTime) / halfTime;
            else
                factor = 1 - (timer - minTime - halfTime) / halfTime;

            return 1 + (Helper.BezierEase(factor) * 2 - 1) * 0.9f;
        }
    }
}
