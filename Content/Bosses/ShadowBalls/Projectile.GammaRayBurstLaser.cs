using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>伽玛射线暴的超大激光占位弹幕；当前仅定义传入参数。</summary>
    public class GammaRayBurstLaser : CoraliteBossHostileProj
    {
        // 暂用空白贴图，保证尚未实现绘制时仍可加载。
        public override string Texture => AssetDirectory.Blank;

        /// <summary>ai0：影子球本体的 NPC 索引。</summary>
        public ref float OwnerIndex => ref Projectile.ai[0];
        /// <summary>ai1：状态 3 的循环总时长，单位为帧。</summary>
        public ref float ShootTime => ref Projectile.ai[1];
        /// <summary>ai2：两极符号，正极为 1，负极为 -1。</summary>
        public ref float PoleDirection => ref Projectile.ai[2];

        public static ATex LaserGradient { get; private set; }
        protected float timer;
        public List<Vector2> laserTrailPoints = new();

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 200;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (laserTrailPoints.Count < 1)
                return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, laserTrailPoints[^1], 10, ref Projectile.localAI[2]);
        }

        public override bool? CanCutTiles() => false;

        public override void Initialize()
        {
            Projectile.timeLeft = (int)ShootTime;
        }

    }
}
