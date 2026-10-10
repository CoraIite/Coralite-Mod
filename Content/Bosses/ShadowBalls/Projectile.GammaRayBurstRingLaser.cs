using Coralite.Core;
using Coralite.Core.Systems.BossSystem;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>伽玛射线暴的环形激光占位弹幕；当前仅定义传入参数。</summary>
    public class GammaRayBurstRingLaser : CoraliteBossHostileProj
    {
        // 暂用空白贴图，保证尚未实现绘制时仍可加载。
        public override string Texture => AssetDirectory.Blank;

        /// <summary>ai0：影子球本体的 NPC 索引。</summary>
        public ref float OwnerIndex => ref Projectile.ai[0];

        /// <summary>ai1：本体记录距离的 1/8。</summary>
        public ref float Radius => ref Projectile.ai[1];

        /// <summary>velocity：本体中心 + 本体朝向单位向量 × 指定长度，保存远端世界坐标。</summary>
        public ref Vector2 EndPoint => ref Projectile.velocity;

        // TODO：补充默认属性、AI、碰撞和绘制。
    }
}
