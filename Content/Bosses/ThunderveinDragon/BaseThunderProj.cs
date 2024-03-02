using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// ai2控制点距离，localai1控制闪电宽度，localai2控制闪电透明度
    /// </summary>
    public abstract class BaseThunderProj: ModProjectile
    {
        public ref float PointDistance => ref Projectile.ai[2];

        public ref float ThunderWidth => ref Projectile.localAI[1];
        public ref float ThunderAlpha => ref Projectile.localAI[2];

    }
}
