namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class BaseBowHeldProj : BaseHeldProj
    {
        /// <summary> 弓弦位置与贴图中心位置的X差值 </summary>
        public readonly int toCenterX;
        /// <summary> 弓弦位置与贴图中心位置的X差值 </summary>
        public readonly int topToCenterY;
        /// <summary> 弓弦位置与贴图中心位置的X差值 </summary>
        public readonly int BottomToCenterY;

        /// <summary>
        /// 与玩家的距离
        /// </summary>
        public float DistanceToOwner;
        /// <summary>
        /// Y方向的缩放，用于拉弓
        /// </summary>
        public float scaleY;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            SetHeldProj();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
