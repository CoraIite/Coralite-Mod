using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// ai1用于判断当前勾到的敌人
    /// </summary>
    public abstract class BaseSilkKnifeSpecialProj : BaseHeldProj
    {
        public ref float Target => ref Projectile.ai[1];
        public ref float HookState => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        protected Vector2 offset;

        /// <summary> 钩住敌人后能保留的最远距离 </summary>
        protected readonly int onHookedLength;
        /// <summary> 在手中旋转时的长度 </summary>
        protected readonly int rollingLength;
        /// <summary> 在手中旋转后松手射出的速度 </summary>
        protected readonly float shootSpeed;
        /// <summary> 射出多久后开始回到手中 </summary>
        protected readonly float shootTime;

        public bool canDamage;

        protected float backSpeed = 32;

        public BaseSilkKnifeSpecialProj(int onHookedLength, int rollingLength, float shootSpeed, float shootTime)
        {
            this.onHookedLength = onHookedLength;
            this.rollingLength = rollingLength;
            this.shootSpeed = shootSpeed;
            this.shootTime = shootTime;
        }

        public enum AIStates
        {
            back = -1,
            rolling = 0,
            shoot = 1,
            onHit = 2,
            drag = 3
        }

        public override void AI()
        {
            switch ((int)HookState)
            {
                default:
                case (int)AIStates.back:    //返回玩家手中的状态
                    BackToOwner();
                    break;
                case (int)AIStates.rolling: //拿在手里转转转的状态
                    RollingInHand();
                    break;
                case (int)AIStates.shoot://发射中的状态
                    Shoot();
                    break;
                case (int)AIStates.onHit://钩在敌人身上的状态
                    OnHookedToNPC();
                    break;
                case (int)AIStates.drag://将玩家拖拽过去的状态
                    Dragging();
                    break;
            }
        }

        /// <summary>
        /// 没有命中后收回到玩家手中
        /// </summary>
        public virtual void BackToOwner()
        {
            if (Vector2.Distance(Owner.Center, Projectile.Center) < (backSpeed + 24))
                Projectile.Kill();

            Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One) * backSpeed;
            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
        }

        /// <summary>
        /// 在手中转圈圈
        /// </summary>
        public virtual void RollingInHand()
        {
            if (Main.mouseRight)
            {
                Owner.heldProj = Projectile.whoAmI;
                Owner.itemAnimation = Owner.itemTime = 2;
                Projectile.rotation += 0.5f;
                if (Projectile.rotation % 4 < 0.2f)
                    SoundEngine.PlaySound(CoraliteSoundID.Swing2_Item7, Projectile.Center);

                Owner.itemRotation = Projectile.rotation + (OwnerDirection > 0 ? 0 : MathHelper.Pi);
                Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * rollingLength;
            }
            else
            {
                SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
                Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                Projectile.Center = Owner.Center + dir * 64;
                Projectile.velocity = dir * shootSpeed;
                Projectile.rotation = dir.ToRotation();
                HookState = (int)AIStates.shoot;
                Projectile.tileCollide = true;
                Projectile.netUpdate = true;
            }
        }

        /// <summary>
        /// 松手后射出
        /// </summary>
        public virtual void Shoot()
        {
            if (Timer > shootTime)
            {
                Timer = 0;
                HookState = (int)AIStates.back;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
            Timer++;
        }

        /// <summary>
        /// 钩在敌人身上时
        /// </summary>
        public virtual void OnHookedToNPC()
        {
            if (Target < 0 || Target > Main.maxNPCs)
            {
                Projectile.Kill();
            }
            NPC npc = Main.npc[(int)Target];
            if (!npc.active || npc.dontTakeDamage || npc.Distance(Owner.Center) > onHookedLength || !Collision.CanHitLine(Owner.Center, 1, 1, npc.Center, 1, 1))
            {
                Projectile.Kill();
            }

            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
            Projectile.Center = npc.Center + offset;
            Timer = 0;
        }

        /// <summary>
        /// 勾到敌人后再次右键的特殊行为<br></br>
        /// <para>
        /// 可以把玩家钩向敌人，也可以把敌人拖拽到面前，或是其他特殊动作
        /// </para>
        /// 
        /// </summary>
        public abstract void Dragging();


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if ((int)HookState == (int)AIStates.shoot)
            {
                HookState = (int)AIStates.back;
                Projectile.tileCollide = false;
            }
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((int)HookState == (int)AIStates.drag && canDamage)
            {
                return true;
            }
            if ((int)HookState < 2 && Collision.CanHitLine(Owner.Center, 1, 1, target.Center, 1, 1))
                return null;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((int)HookState == (int)AIStates.shoot)
            {
                Projectile.velocity *= 0;
                Target = target.whoAmI;
                offset = Projectile.Center - target.Center;
                HookState = (int)AIStates.onHit;
                Timer = 0;
                Projectile.netUpdate = true;

                OnHitNPC_Shoot(target, hit, damageDone);
            }
            else if ((int)HookState == (int)AIStates.drag)
                OnHitNPC_Draging(target, hit, damageDone);
        }

        public virtual void OnHitNPC_Shoot(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public virtual void OnHitNPC_Draging(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
    }
}
