using Coralite.Content.DamageClasses;
using InnoVault.GameContent.BaseEntity;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    /// <summary>
    /// 罐子类武器的基类
    /// </summary>
    public abstract class BaseJarProj : BaseHeldProj
    {
        public AIStates State
        {
            get => (AIStates)Projectile.ai[2];
            set => Projectile.ai[2] = (int)value;
        }

        public enum AIStates
        {
            /// <summary>
            /// 拿在手里的状态
            /// </summary>
            Held,
            /// <summary>
            /// 投掷出的状态
            /// </summary>
            Shoot
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return State == AIStates.Shoot;
        }

        public override bool? CanDamage()
        {
            if (State == AIStates.Shoot)
                return null;

            return false;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case AIStates.Held:
                    break;
                case AIStates.Shoot:
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}
