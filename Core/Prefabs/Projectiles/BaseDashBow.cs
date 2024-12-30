using System.Linq;
using Terraria;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// ai0用于判断玩家的手持方向，ai1用于控制是否是特殊的弹幕，ai2用于记录冲刺时间
    /// </summary>
    public abstract class BaseDashBow : BaseHeldProj
    {
        public ref float Rotation => ref Projectile.ai[0];
        /// <summary>
        /// 为0时是普通射击，为1时是冲刺射击
        /// </summary>
        public ref float Special => ref Projectile.ai[1];
        public ref float DashTime => ref Projectile.ai[2];

        protected bool Init = true;

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60000;
        }

        public override bool? CanDamage() => false;

        /// <summary>
        /// 物品类型
        /// </summary>
        /// <returns></returns>
        public abstract int GetItemType();

        public sealed override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            if (Owner.HeldItem.type != GetItemType())
            {
                Projectile.Kill();
                return;
            }

            if (Init)
            {
                Init = false;
                switch (Special)
                {
                    default:
                    case 0:
                        Projectile.timeLeft = Owner.itemTimeMax;

                        if (Main.projectile.Any(p => p.active && p.friendly
                            && p.owner == Projectile.owner && p.type == Projectile.type && p.ai[1] == 1))
                        {
                            Projectile.Kill();
                        }

                        break;
                    case 1:
                        break;
                }

                Initialize();
            }

            AIBefore();

            switch (Special)
            {
                default:
                case 0:
                    NormalShootAI();
                    SetCenter();
                    break;
                case 1:
                    DashAttackAI();
                    SetCenter();
                    break;
            }

            AIAfter();

            Owner.itemRotation = Rotation + (DirSign > 0 ? 0 : 3.141f);
        }

        public virtual void Initialize()
        {

        }

        public virtual void AIBefore()
        {

        }

        public virtual void AIAfter() { }

        public virtual void NormalShootAI()
        {
            Projectile.rotation = Rotation;
        }

        public virtual void SetCenter()
        {
            Vector2 offset = GetOffset();
            Vector2 dir2 = (Rotation + (DirSign > 0 ? 1.57f : -1.57f)).ToRotationVector2();
            Projectile.Center = Owner.Center + (Rotation.ToRotationVector2() * offset.X) + dir2 * offset.Y + new Vector2(0, Owner.gfxOffY);
        }

        public virtual void DashAttackAI()
        {

        }

        /// <summary>
        /// 获取弓的中心点与玩家之间的距离
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetOffset() => Vector2.Zero;
    }
}
