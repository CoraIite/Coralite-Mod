using InnoVault.GameContent.BaseEntity;
using Terraria;

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
            SetHeld();
        }

        public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
        {
            return base.PreDraw(player, ref lightColor);
        }
    }
}
