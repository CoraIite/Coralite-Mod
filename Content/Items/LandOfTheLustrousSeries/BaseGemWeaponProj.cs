using Coralite.Core.Prefabs.Projectiles;
using InnoVault.GameContent.BaseEntity;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class BaseGemWeaponProj<T> : BaseHeldProj where T : ModItem
    {
        private bool init = true;

        public ref float AttackTime => ref Projectile.ai[0];

        public Vector2 TargetPos
        {
            get
            {
                return new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            }
            set
            {
                Projectile.localAI[0] = value.X;
                Projectile.localAI[1] = value.Y;
            }
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
        }

        public sealed override void AI()
        {
            if (Item.type == ModContent.ItemType<T>())
                Projectile.timeLeft = 2;

            if (init)
            {
                InitializeGemWeapon();
                init = false;
            }

            BeforeMove();
            Move();
            Attack();
        }

        public override void Initialize()
        {
            TargetPos = Owner.Center;
            InitializeGemWeapon();
        }

        public virtual void InitializeGemWeapon()
        {
            
        }

        public virtual void BeforeMove() { }
        public virtual void Move() { }
        public virtual void Attack() { }
        public virtual void StartAttack()
        {
            AttackTime = Owner.itemTimeMax;
        }
    }
}
