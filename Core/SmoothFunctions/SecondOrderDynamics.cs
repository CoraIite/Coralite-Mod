using System;

namespace Coralite.Core.SmoothFunctions
{
    /// <summary>
    /// 用于根据输入平滑运动位置或速度
    /// </summary>
    /// <param name="f">系统固有频率，单位为Hz，描述系统对于变化的响应速度，越高变化速度越快</param>
    /// <param name="z">阻尼系数，为0时无限震动，0-1之间震荡接近目标位置，大于1时不产生震动</param>
    /// <param name="r">控制系统初始响应</param>
    /// <param name="x0"></param>
    public class SecondOrderDynamics_Vec2(float f, float z, float r, Vector2 x0)
    {
        /// <summary>
        /// 上一次输入
        /// </summary>
        private Vector2 xp = x0;
        private Vector2 y = x0, yd = Vector2.Zero;
        /// <summary>
        /// 3个约束
        /// </summary>
        private float k1 = z / (MathHelper.Pi * f);
        private float k2 = 1 / ((MathHelper.TwoPi * f) * (MathHelper.TwoPi * f));
        private float k3 = r * z / (MathHelper.TwoPi * f);

        public Vector2 Update(float t, Vector2 x, Vector2? xd = null)
        {
            if (!xd.HasValue)
            {
                xd = (x - xp) / t;
                xp = x;
            }

            float k2_stable = MathF.Max(MathF.Max(k2, t * t / 2 + t * k1 / 2), t * k1);

            y += t * yd;
            yd += t * (x + k3 * xd.Value - y - k1 * yd) / k2_stable;

            //y += + t * yd;
            //yd += t * (x + k3 * xd.Value - y - k1 * yd) / k2;

            return y;
        }
    }

    /// <summary>
    /// 用于根据输入平滑运动位置或速度
    /// </summary>
    /// <param name="f">系统固有频率，单位为Hz，描述系统对于变化的响应速度，越高变化速度越快</param>
    /// <param name="z">阻尼系数，为0时无限震动，0-1之间震荡接近目标位置，大于1时不产生震动</param>
    /// <param name="r">控制系统初始响应</param>
    /// <param name="x0"></param>
    public class SecondOrderDynamics_Float(float f, float z, float r, float x0)
    {
        /// <summary>
        /// 上一次输入
        /// </summary>
        private float xp = x0;
        private float y = x0, yd = 0;
        /// <summary>
        /// 3个约束
        /// </summary>
        private float k1 = z / (MathHelper.Pi * f);
        private float k2 = 1 / ((MathHelper.TwoPi * f) * (MathHelper.TwoPi * f));
        private float k3 = r * z / (MathHelper.TwoPi * f);

        public float Update(float t, float x, float? xd = null)
        {
            if (!xd.HasValue)
            {
                xd = (x - xp) / t;
                xp = x;
            }

            float k2_stable = MathF.Max(MathF.Max(k2, t * t / 2 + t * k1 / 2), t * k1);

            y += t * yd;
            yd += t * (x + k3 * xd.Value - y - k1 * yd) / k2_stable;

            //y += + t * yd;
            //yd += t * (x + k3 * xd.Value - y - k1 * yd) / k2;

            return y;
        }
    }

    ///// <summary>
    ///// 用于根据输入平滑运动位置或速度
    ///// </summary>
    ///// <param name="f">系统固有频率，单位为Hz，描述系统对于变化的响应速度，越高变化速度越快</param>
    ///// <param name="z">阻尼系数，为0时无限震动，0-1之间震荡接近目标位置，大于1时不产生震动</param>
    ///// <param name="r">控制系统初始响应</param>
    ///// <param name="x0"></param>
    //public class SecondOrderDynamics_V2EX
    //{
    //    /// <summary>
    //    /// 上一次输入
    //    /// </summary>
    //    private Vector2 xp;
    //    private Vector2 y , yd = Vector2.Zero;
    //    /// <summary>
    //    /// 3个约束
    //    /// </summary>
    //    private float k1;
    //    private float k2;
    //    private float k3;

    //    private float _w;
    //    private float _z;
    //    private float _d;

    //    public SecondOrderDynamics_V2EX(float f, float z, float r, Vector2 x0)
    //    {
    //        _w = MathHelper.TwoPi * f;
    //        _z = z;
    //        _d = _w * MathF.Sqrt(Math.Abs(z * z - 1));

    //        k1 = z / (MathHelper.Pi * f);
    //        k2 = 1 / ((MathHelper.TwoPi * f) * 2 * MathHelper.TwoPi * f);
    //        k3 = r * z / (MathHelper.TwoPi * f);

    //        xp = x0;
    //        y = x0;
    //    }

    //    public Vector2 Update(float t, Vector2 x, Vector2? xd = null)
    //    {
    //        if (!xd.HasValue)
    //        {
    //            xd = (x - xp) / t;
    //            xp = x;
    //        }

    //        //float k2_stable = MathF.Max(MathF.Max(k2, t * t / 2 + t * k1 / 2), t * k1);

    //        float k1_stable;
    //        float k2_stable;

    //        if (_w * t < _z){
    //            k1_stable=
    //        }

    //        y += t * yd;
    //        yd += t * (x + k3 * xd.Value - y - k1 * yd) / k2_stable;

    //        //y += + t * yd;
    //        //yd += t * (x + k3 * xd.Value - y - k1 * yd) / k2;

    //        return y;
    //    }
    //}

}
