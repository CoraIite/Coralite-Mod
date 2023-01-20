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
            if ((timer-minTime) <= halfTime)
                factor = (timer - minTime) / halfTime;
            else
                factor = 1 - (timer - minTime - halfTime) / halfTime;

            return 1 + (Helper.BezierEase(factor) * 2 - 1) * 0.9f;
        }
    }

    public class Fast2SlowLinerSmoother : ISmoother
    {
        public float yWidth;

        public Fast2SlowLinerSmoother(float yWidth)
        {
            this.yWidth=yWidth;
        }

        public void ReCalculate(int minTime, int maxTime) { }

        public float Smoother(int timer, int minTime, int maxTime)
        {
            float factor = (timer - minTime) / (maxTime - minTime);
            return Helper.Lerp(yWidth, 1, factor);
        }
    }

    public class Fast2SlowSmoother : ISmoother
    {
        public float fastCoefficient;
        public float slowCoefficient;
        public float slowPercent;
        public int slowTime;
        public int slowTimeWidth;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slowPercent">减速是的时间占总时间的百分比</param>
        /// <param name="fastCoefficient">快速时的加速倍率</param>
        /// <param name="slowCoefficient">慢速时的减速倍率</param>
        /// <param name="slowTimeWidth">减速所需时长</param>
        public Fast2SlowSmoother(float slowPercent, float fastCoefficient, float slowCoefficient, int slowTimeWidth)
        {
            this.slowPercent = slowPercent;
            this.fastCoefficient = fastCoefficient;
            this.slowCoefficient = slowCoefficient;
            this.slowTimeWidth = slowTimeWidth;
        }

        public void ReCalculate(int minTime, int maxTime)
        {
            slowTime = (int)((maxTime - minTime) * slowPercent);
        }

        public float Smoother(int timer, int minTime, int maxTime)
        {
            timer -= minTime;
            if (timer < slowTime)
                return fastCoefficient;
            if (timer < slowTime + slowTimeWidth)
                return Helper.Lerp(fastCoefficient, slowCoefficient, (timer - slowTime) / slowTimeWidth);

            return slowCoefficient;
        }
    }
}
