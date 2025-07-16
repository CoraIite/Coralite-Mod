using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem.NormalSkills
{
    /// <summary>
    /// 撞击
    /// </summary>
    public class FSkill_Tackle : FairySkill
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "Tackle";

        public LocalizedText DashDamage { get; set; }
        public LocalizedText Chase { get; set; }

        /// <summary>
        /// 基础冲撞时间
        /// </summary>
        protected virtual int DashTime { get; } = 30;

        /// <summary>
        /// 穿透数
        /// </summary>
        private int _penetrate;

        /// <summary>
        /// 最大穿透数，在开始攻击时初始化
        /// </summary>
        protected int MaxPenetrate { get; set; }

        public override void OnStartAttack(BaseFairyProjectile fairyProj)
        {
            _penetrate = 0;
            //设置最大穿透数
            MaxPenetrate = fairyProj.SkillLevel / 3;
            fairyProj.Projectile.tileCollide = true;

            float speed = fairyProj.IVSpeed * (1.25f + fairyProj.SkillLevel * 0.03f);

            //根据技能等级增幅撞击速度
            speed *= Helper.Lerp(1, 1.75f, Helper.X2Ease(Math.Clamp(fairyProj.SkillLevel / 15, 0, 1)));

            if (fairyProj.TargetIndex.GetNPCOwner(out NPC target))
            {
                fairyProj.Projectile.velocity =
                    (target.Center - fairyProj.Projectile.Center).SafeNormalize(Vector2.Zero) * speed;

                //技能等级越高冲刺时间越长
                SkillTimer = (int)(Vector2.Distance(target.Center, fairyProj.Projectile.Center) / speed)
                    + 5 + fairyProj.SkillLevel * 10;
            }
        }

        public override bool Update(BaseFairyProjectile fairyProj)
        {
            //只是计时
            SkillTimer--;
            if (SkillTimer < 1)
            {
                fairyProj.Projectile.velocity *= 0.95f;
                if (SkillTimer < -45)
                    return true;

                return false;
            }

            //技能等级>6拥有追踪功能


            fairyProj.SpawnFairyDust();

            return false;
        }

        public override void OnTileCollide(BaseFairyProjectile fairyProj, Vector2 oldVelocity)
        {
            if (SkillTimer > 0)
                SkillTimer = 0;
            fairyProj.Projectile.velocity = -oldVelocity.SafeNormalize(Vector2.Zero) * 2;
        }

        public override void ModifyHitNPC_Active(BaseFairyProjectile fairyProj, NPC target, ref NPC.HitModifiers hitModifier, ref int npcDamage)
        {
            _penetrate++;
            if (_penetrate > MaxPenetrate)
            {
                //结束技能
                if (SkillTimer > 0)
                    SkillTimer = 0;
                fairyProj.Projectile.velocity = -fairyProj.Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
            }

            hitModifier.SourceDamage += (1.5f + fairyProj.SkillLevel * 0.15f);
        }

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            string chase = iv.SkillLevel > 6
                ? Chase.Value : FairySystem.SkillLVLimit.Format(7);

            int level = iv.SkillLevel;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.FairySkillBonus[Type].ModifyLevel(level);

            return string.Concat(DashDamage.Format(1.5f + level * 0.15f)
                , Environment.NewLine, chase);
        }
    }
}
