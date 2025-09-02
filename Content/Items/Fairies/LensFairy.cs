using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkillTags;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Fairies
{
    public class LensFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<LensFairy>();
        public override FairyRarity Rarity => FairyRarity.U;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<LensFairyProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_Dash>(),
                ];
        }
    }

    public class LensFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<LensFairyItem>();

        public override FairyRarity Rarity => FairyRarity.U;

        private int state;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddConditions(FairySpawnCondition.DownedEOC, FairySpawnCondition.NightTime)
                .RegisterToWall();
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            if (state < 1)
            {
                EscapeNormally(catcher, (60, 100), (1.5f, 1.75f)
                    , onRestart: () =>
                    {
                        state = Main.rand.Next(5);
                        if (state > 0)
                        {
                            velocity = targetVelocity = Helper.NextVec2Dir(1.75f, 2.2f);
                            FairyTimer = Main.rand.Next(40, 60);
                        }
                    });
            }
            else//随机朝一个方向移动，并缓慢减速
            {
                FairyTimer--;
                targetVelocity *= 0.98f;

                if (FairyTimer < 0)
                {
                    velocity = targetVelocity = Helper.NextVec2Dir(1.9f, 2.2f);
                    state--;
                    if (state < 1)
                        FairyTimer = Main.rand.Next(60, 100);
                    else
                        FairyTimer = Main.rand.Next(40, 60);
                }

                UpdateVelocity();
            }
        }

        public override void OnCatch(Player player, ref int catchPower)
        {
            targetVelocity = Helper.NextVec2Dir(0.8f, 1f);
        }
    }

    public class LensFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "LensFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_Dash>(),
                ];

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, 0.05f, 0.15f, 0.05f);
        }

        public override Vector2 GetRestSpeed()
        {
            float f = Timer * 0.1f + Projectile.identity * MathHelper.TwoPi / 6;
            return new Vector2(MathF.Sin(f) * 3, MathF.Cos(f) * 1.5f);
        }

        public override void OnStartUseSkill(NPC target)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Bloody_NPCHit9, Projectile.Center);
        }

        public override void OnKill_DeadEffect()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Bloody_NPCHit9, Projectile.Center);
        }
    }

    public class FSkill_Dash : FairySkill
    {
        public override string Texture => AssetDirectory.FairySkillIcons+ "Tackle";

        private int DashCount;
        private int RecordDashTime;
        private int DashTime;
        private int HitCount;

        public LocalizedText DashDamage { get; set; }

        public override int[] SkillTags => [CoraliteContent.FairySkillTagType<FSTag_Impact>()];

        public float GetDamageBonus(int skillLevel)
        {
            return 1.75f + 0.01f * skillLevel;
        }

        public override int GetDamage(int baseDamage, int skillLevel)
        {
            return (int)(baseDamage * GetDamageBonus(skillLevel));
        }

        /// <summary>
        /// 获取冲刺次数
        /// </summary>
        /// <param name="skillLevel"></param>
        /// <returns></returns>
        public int GetDashCount(int skillLevel)
        {
            return 3 + skillLevel / 25;
        }

        public override void OnStartAttack(BaseFairyProjectile fairyProj)
        {
            DashCount = GetDashCount(fairyProj.SkillLevel);
            RecordDashTime = 35 + (int)(35 * Helper.Clamp(fairyProj.SkillLevel / 100f, 0, 1));
        }

        public override bool Update(BaseFairyProjectile fairyProj)
        {
            //只是计时
            if (!fairyProj.TargetIndex.GetNPCOwner(out NPC target, () =>
            {
                fairyProj.TargetIndex = -1;
                fairyProj.Projectile.rotation = 0;
            }))
                return true;

            if (SkillTimer == 0)
            {
                float speed = fairyProj.IVSpeed;

                float length = (target.Center - fairyProj.Projectile.Center).Length();

                //根据技能等级增幅撞击速度
                speed *= Helper.Lerp(1.4f, 2f, Helper.X2Ease(Math.Clamp(fairyProj.SkillLevel / 100f, 0, 1)));

                fairyProj.Projectile.velocity = fairyProj.Projectile.velocity =
                    (target.Center - fairyProj.Projectile.Center).SafeNormalize(Vector2.Zero) * speed;

                fairyProj.Projectile.rotation = (target.Center - fairyProj.Projectile.Center).ToRotation()+MathHelper.PiOver2;

                if (length / speed + 10 < RecordDashTime)
                    DashTime = (int)(length / speed + 10);
                else
                    DashTime = RecordDashTime;

                //Helper.PlayPitched(CoraliteSoundID.ForceRoar, fairyProj.Projectile.Center, 0.4f,0.5f);
            }

            if (SkillTimer > DashTime)
            {
                fairyProj.Projectile.velocity *= 0.9f;
                fairyProj.Projectile.rotation = fairyProj.Projectile.rotation.AngleLerp(0, 0.2f);
            }

            if (SkillTimer > DashTime + 15)
            {
                SkillTimer = 0;
                DashCount--;
                fairyProj.CurrentStamina--;//每冲一次扣一次耐力

                if (fairyProj.CurrentStamina < 1 || DashCount < 1)
                    return true;

                return false;
            }

            SkillTimer++;
            SetDirection(fairyProj, target);

            return false;
        }

        public override void ModifyHitNPC_Active(BaseFairyProjectile fairyProj, NPC target, ref NPC.HitModifiers hitModifier, ref int npcDamage)
        {
            hitModifier.FinalDamage += (Math.Clamp(1 - HitCount / 5f, 0, 1))
                * (GetDamageBonus(fairyProj.SkillLevel) - 1);

            HitCount++;
        }

        public override void OnTileCollide(BaseFairyProjectile fairyProj, Vector2 oldVelocity)
        {
            if (SkillTimer < DashTime)
                SkillTimer = DashTime + 1;

            fairyProj.Projectile.velocity = -oldVelocity * 0.5f;
        }

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = Helper.GetBonusedSkillLevel(player, iv.SkillLevel, Type);

            return DashDamage.Format(GetDashCount(level));
        }
    }
}
