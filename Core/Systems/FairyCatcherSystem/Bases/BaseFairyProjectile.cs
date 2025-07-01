using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        /// <summary>
        /// 生成时间，生成仙灵时传入
        /// </summary>
        public ref float SpawnTime => ref Projectile.ai[0];
        /// <summary>
        /// 当前使用技能的索引
        /// </summary>
        public ref float UseSkillIndex => ref Projectile.ai[1];
        /// <summary>
        /// 目标敌怪的索引
        /// </summary>
        public ref float TargetIndex => ref Projectile.ai[2];

        public AIStates State {  get; set; }
        public int Timer {  get; set; }

        /// <summary>
        /// 受击后无敌时间
        /// </summary>
        public int ImmuneTimer {  get; set; }



        protected virtual int FrameX => 1;
        protected virtual int FrameY => 4;
        /// <summary>
        /// 能够开始攻击的距离
        /// </summary>
        protected float AttackDistance = 400;

        private bool init = true;
        protected bool canDamage;

        /// <summary>
        /// 存储所有的仙灵技能
        /// </summary>
        private FairySkill[] _skills;

        /// <summary>
        /// 仅本地端可用！用它获取仙灵个体值以及执行仙灵掉血等操作
        /// </summary>
        public BaseFairyItem BaseFairyItem { get; set; }

        public enum AIStates
        {
            /// <summary>
            /// 刚从仙灵捕手中射出的时候
            /// </summary>
            Shooting,
            /// <summary>
            /// 执行自身的特定AI
            /// </summary>
            Skill,
            /// <summary>
            /// 释放技能后的后摇
            /// </summary>
            Rest,
            /// <summary>
            /// 返回自身，此时无敌
            /// </summary>
            Backing,
        }

        public sealed override void AI()
        {
            if (init)
                Initilize();

            switch (State)
            {
                default:
                    Projectile.Kill();
                    break;
                case AIStates.Shooting:
                    Shooting();
                    break;
                case AIStates.Skill:
                    Skill();
                    break;
                case AIStates.Rest:
                    break;
                case AIStates.Backing:
                    Backing();
                    break;
            }
        }

        public void Initilize()
        {
            init = false;
            TargetIndex = -1;

            Projectile.Resize((int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale));

            //初始化技能
            var skill = InitSkill();
            if (skill != null && skill.Length > 0)
                _skills = skill;
            else
                "仙灵必须有至少一个技能！！".Dump();

            OnInitialize();
        }

        /// <summary>
        /// 初始化仙灵技能
        /// </summary>
        /// <returns></returns>
        public abstract FairySkill[] InitSkill();

        /// <summary>
        /// 帮助方法，获取技能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FairySkill NewSkill<T>() where T : FairySkill
            => ModContent.GetInstance<T>().NewInstance();

        public virtual void OnInitialize() { }

        public virtual void Shooting()
        {
            Timer++;
            if (Timer > SpawnTime)
                TryExchangeToAttack();
        }

        public virtual void Skill()
        {

        }

        public virtual void Backing()
        {

        }

        //public virtual void Backing_LerpToOwner()
        //{
        //    Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 2;
        //    Projectile.Center = Vector2.SmoothStep(Projectile.Center, Owner.Center, Helper.SqrtEase(Timer * 0.00075f));

        //    if (Vector2.Distance(Projectile.Center, Owner.Center) < 24)
        //        Projectile.Kill();
        //}

        //public virtual void SpawnSkillText(Color color)
        //{
        //    ModProjectile m = ModContent.GetModProjectile(Projectile.type);
        //    CombatText.NewText(Projectile.getRect(), color,
        //        (m as BaseFairyProjectile).SkillText.Value);
        //}

        public void UpdateFrameY(int spacing)
        {
            Projectile.UpdateFrameNormally(spacing, FrameY - 1);
        }

        public void SetDirectionNormally()
        {
            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        }

        public override bool? CanDamage()
        {
            if (canDamage)
                return null;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //自身受伤
            //if (FairyItem != null)
            //    if (FairyItem.Hurt(Owner, target, hit, damageDone))
            //    {
            //        Projectile.Kill();
            //        OnKillByNPC(target);
            //        return;
            //    }
        }

        public virtual void ExchangeToBack()
        {
            Timer = 0;
            State = AIStates.Backing;
            canDamage = false;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
        }

        /// <summary>
        /// 尝试开始攻击，如果没有那么就进入休息阶段
        /// </summary>
        public virtual void TryExchangeToAttack()
        {
            if (Helper.TryFindClosestEnemy(Projectile.Center, AttackDistance
                , n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
            {
                TargetIndex = target.whoAmI;
                State = AIStates.Skill;
                Timer = 0;

                OnExchangeToAction(target);
                canDamage = true;
            }
            else
                ExchangeToBack();
        }

        public virtual void RestartAttack()
        {
            if (Helper.TryFindClosestEnemy(Projectile.Center, AttackDistance, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
            {
                State = AIStates.Skill;
                Timer = 0;

                OnExchangeToAction(target);
            }
        }

        public virtual void OnExchangeToAction(NPC target) { }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == (int)AIStates.Shooting)
                TryExchangeToAttack();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            //FairyItem.IsOut = false;
        }

        /// <summary>
        /// 在仙灵血量减为0后执行，用于生成死亡效果
        /// </summary>
        /// <param name="target"></param>
        public virtual void OnKillByNPC(NPC target) { }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSelf(Main.spriteBatch, Main.screenPosition, lightColor);

            if (canDamage)
                DrawHealthBar();
            return false;
        }

        /// <summary>
        /// 绘制自己
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPos"></param>
        /// <param name="lightColor"></param>
        public virtual void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frame = mainTex.Frame(FrameX, FrameY, 0, Projectile.frame);

            spriteBatch.Draw(mainTex, Projectile.Center - screenPos, frame, lightColor, Projectile.rotation, frame.Size() / 2,
                Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public virtual void DrawHealthBar()
        {
            //Main.instance.DrawHealthBar(Projectile.Bottom.X, Projectile.Bottom.Y + 12, Life, LifeMax, 1, 1);
        }
    }
}
