using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem.NormalSkills
{
    /// <summary>
    /// 治疗
    /// </summary>
    public class FSkill_HealOutside : FairySkill
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "HealOutside";

        public LocalizedText HealValue { get; set; }

        public override Color SkillTextColor => Color.Green;

        /// <summary>
        /// 获取治疗百分比
        /// </summary>
        /// <param name="level"></param>
        public virtual float GetHealPercent(int level)
        {
            return 0.02f + level * 0.01f;
        }

        public override int GetDamage(int baseDamage, int skillLevel)
        {
            return -1;
        }

        public override void OnStartAttack(BaseFairyProjectile fairyProj)
        {
            SkillTimer = 30;
        }

        public override bool Update(BaseFairyProjectile fairyProj)
        {
            //只是计时
            SkillTimer--;
            if (SkillTimer > 0)
            {
                //向NPC移动
                Vector2 restSpeed = Vector2.Zero;
                if (Vector2.Distance(fairyProj.Owner.Center, fairyProj.Projectile.Center) > fairyProj.AttackDistance)
                    restSpeed = (fairyProj.Owner.Center - fairyProj.Projectile.Center).SafeNormalize(Vector2.Zero) * fairyProj.IVSpeed;

                float slowTime = 10;

                fairyProj.Projectile.velocity = ((fairyProj.Projectile.velocity * slowTime) + restSpeed) / (slowTime + 1);
            }
            else
            {
                //遍历在场的仙灵，回复并生成绿色“+”粒子
                float percent = GetHealPercent(fairyProj.SkillLevel);
                foreach (var proj in
                    Main.projectile.Where(p => p.active && p.owner == fairyProj.Projectile.owner && p.ModProjectile is BaseFairyProjectile))
                {
                    var i = (proj.ModProjectile as BaseFairyProjectile).FairyItem;
                    i.HealFairy(percent, fairyProj.Projectile.damage);

                    if (!VaultUtils.isServer)
                        for (int j = 0; j < 4; j++)
                        {
                            var particle = PRTLoader.NewParticle<HealParticle>(Main.rand.NextVector2FromRectangle(proj.getRect())
                                 , -Vector2.UnitY * Main.rand.NextFloat(1, 2), Scale: Main.rand.NextFloat(0.8f, 1f));
                            particle.projIndex = proj.whoAmI;
                        }
                }

                return true;
            }

            fairyProj.SetDirectionNormally();
            fairyProj.SpawnFairyDust(fairyProj.Projectile.Center, fairyProj.Projectile.velocity);

            return false;
        }

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = iv.SkillLevel;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.GetFairySkillBonus(Type, level);

            return HealValue.Format(GetHealPercent(level) * 100, iv.Damage);
        }
    }

    public class HealParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles + Name;

        public int projIndex = -1;
        public float alpha = 0;

        public override void SetProperty()
        {

        }

        public override void AI()
        {
            if (projIndex.GetProjectileOwner(out Projectile proj, () => projIndex = -1))
            {
                Position += proj.position - proj.oldPosition;
            }

            Opacity++;
            if (Opacity < 7)
            {
                alpha += 1 / 7f;
            }
            else if (Opacity < 7 + 15)
            {
                alpha -= 1 / 15f;
            }
            else
                active = false;

            Velocity *= 0.98f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Vector2 pos = Position - Main.screenPosition;
            TexValue.QuickCenteredDraw(spriteBatch, pos, Color.White * 0.75f * alpha, scale: Scale);
            TexValue.QuickCenteredDraw(spriteBatch, pos, Color.White with { A = 0 } * alpha, scale: Scale);
            return false;
        }
    }
}
