using Coralite.Helpers;
using System;

namespace Coralite.Core.Prefabs.Projectiles
{
    public interface ISmoother
    {
        void ReCalculate(int maxTime);

        /// <summary>
        /// 一般返回一个0-1之间的插值
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="maxTime"></param>
        /// <returns></returns>
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

    public class HeavySmoother : ISmoother
    {
        public void ReCalculate(int maxTime) { }

        public float Smoother(int timer, int maxTime)
        {
            float factor = (float)timer / maxTime;
            return Helper.HeavyEase(factor);
        }
    }

    public class X2Smoother : ISmoother
    {
        public void ReCalculate(int maxTime) { }

        public float Smoother(int timer, int maxTime)
        {
            float factor = (float)timer / maxTime;
            return factor * factor;
        }
    }

    public class SqrtSmoother:ISmoother
    {
        public void ReCalculate(int maxTime) { }

        public float Smoother(int timer, int maxTime)
        {
            float factor = (float)timer / maxTime;
            return MathF.Sqrt(factor);
        }
    }
}
