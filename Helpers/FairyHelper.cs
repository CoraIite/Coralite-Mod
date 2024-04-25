using Coralite.Core.Systems.FairyCatcherSystem;
using Terraria;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 在弹幕身上生成粒子，默认为与弹幕相反的速度，如果需要调整速度请将<paramref name="velocityMult"/>设置为负数
        /// </summary>
        /// <param name="fairy">弹幕自身</param>
        /// <param name="type">弹幕种类</param>
        /// <param name="velocityMult">速度系数</param>
        /// <param name="Alpha"></param>
        /// <param name="newColor"></param>
        /// <param name="Scale"></param>
        /// <param name="noGravity">粒子重力</param>
        public static void SpawnTrailDust(this Fairy fairy, int type, float velocityMult, int Alpha = 0, Color newColor = default, float Scale = 1f, bool noGravity = true)
        {
            Dust dust = Dust.NewDustDirect(fairy.position, fairy.width, fairy.height, type, Alpha: Alpha, newColor: newColor, Scale: Scale);
            dust.noGravity = noGravity;
            dust.velocity = -fairy.velocity * velocityMult;
        }

    }
}
