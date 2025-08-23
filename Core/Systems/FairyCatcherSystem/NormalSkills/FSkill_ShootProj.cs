using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.NormalSkills
{
    /// <summary>
    /// 射弹幕的技能
    /// </summary>
    public abstract class FSkill_ShootProj : FairySkill
    {
        /// <summary>
        /// 基础准备时间
        /// </summary>
        protected virtual int ReadyTime { get; } = 45;
        /// <summary>
        /// 后摇时间，不会受到技能等级加成
        /// </summary>
        protected virtual int DelayTime { get; } = 30;
        /// <summary>
        /// 射速，不会受到加成
        /// </summary>
        protected abstract float ShootSpeed { get; }
        /// <summary>
        /// 追踪距离
        /// </summary>
        protected abstract  float ChaseDistance { get; }

        /// <summary>
        /// 射击的弹幕类型
        /// </summary>
        protected abstract int ProjType { get; }

        public override void OnStartAttack(BaseFairyProjectile fairyProj)
        {
            //根据技能等级减少准备时间，最多40%
            SkillTimer = (int)(ReadyTime * (1 - 0.4f * Math.Clamp(fairyProj.SkillLevel / 15f, 0, 1)));
        }

        public override bool Update(BaseFairyProjectile fairyProj)
        {
            //只是计时
            if (!fairyProj.TargetIndex.GetNPCOwner(out NPC target, () => fairyProj.TargetIndex = -1))
                return true;

            SetDirection(fairyProj, target);
            fairyProj.SpawnFairyDust(fairyProj.Projectile.Center, fairyProj.Projectile.velocity);

            if (SkillTimer > 0)
            {
                //向NPC移动
                Vector2 restSpeed = Vector2.Zero;
                if (Vector2.Distance(target.Center, fairyProj.Projectile.Center) > ChaseDistance)
                    restSpeed = (target.Center - fairyProj.Projectile.Center).SafeNormalize(Vector2.Zero) * fairyProj.IVSpeed;
                else
                {
                    if (SkillTimer > 0)
                        SkillTimer--;
                }

                float slowTime = 10;

                fairyProj.Projectile.velocity = ((fairyProj.Projectile.velocity * slowTime) + restSpeed) / (slowTime + 1);

                BeforeShoot(fairyProj);

                return false;
            }

            if (SkillTimer == 0)
            {
                Vector2 center = fairyProj.Projectile.Center;
                Vector2 velocity = (target.Center - fairyProj.Projectile.Center).SafeNormalize(Vector2.Zero) * ShootSpeed;
                int damage = fairyProj.Projectile.damage;

                ModifyShootProj(fairyProj, ref center, ref velocity, ref damage);
                damage = GetDamageBonus(damage, fairyProj.SkillLevel);

                ShootProj(fairyProj, center, velocity, damage);
                SkillTimer--;

                return false;
            }

            if (SkillTimer < 0)
            {
                SkillTimer--;
                if (SkillTimer < -DelayTime)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 设置仙灵的朝向
        /// </summary>
        /// <param name="fairyProj"></param>
        /// <param name="target"></param>
        public virtual void SetDirection(BaseFairyProjectile fairyProj, NPC target)
        {
            if (MathF.Abs(fairyProj.Projectile.Center.X - target.Center.X) > 8)
                fairyProj.Projectile.spriteDirection = target.Center.X > fairyProj.Projectile.Center.X ? 1 : -1;
        }

        /// <summary>
        /// 在开始射击之前执行，可以用于生成粒子等
        /// </summary>
        public virtual void BeforeShoot(BaseFairyProjectile fairyProj)
        {

        }

        /// <summary>
        /// 修改射弹幕的一些属性
        /// </summary>
        /// <param name="fairyProj"></param>
        /// <param name="center"></param>
        /// <param name="velocity"></param>
        /// <param name="baseDamage"></param>
        /// <param name=""></param>
        public virtual void ModifyShootProj(BaseFairyProjectile fairyProj,ref Vector2 center,ref Vector2 velocity,ref int baseDamage)
        {

        }

        /// <summary>
        /// 根据技能等级增加伤害
        /// </summary>
        /// <param name="baseDamage"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public virtual int GetDamageBonus(int baseDamage, int level)
        {
            return (int)(baseDamage * (1 + 2f * Math.Clamp(level / 15f, 0, 1)));
        }

        /// <summary>
        /// 射出弹幕，默认在ai0传入技能等级
        /// </summary>
        /// <param name="fairyProj"></param>
        /// <param name="center"></param>
        /// <param name="velocity"></param>
        /// <param name="damage"></param>
        public virtual void ShootProj(BaseFairyProjectile fairyProj,Vector2 center,Vector2 velocity,int damage)
        {
            fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                , velocity, ProjType, damage, fairyProj.Projectile.knockBack,fairyProj.SkillLevel);
        }
    }
}
