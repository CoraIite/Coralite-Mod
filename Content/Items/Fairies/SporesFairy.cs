using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Fairies
{
    public class SporesFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<SporesFairy>();
        public override FairyRarity Rarity => FairyRarity.U;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<SporesFairyProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_Tackle>(),
                CoraliteContent.FairySkillType<FSkill_SporeExplode>(),
                ];
        }
    }

    public class SporesFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<SporesFairyItem>();

        public override FairyRarity Rarity => FairyRarity.U;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneUndergroundJungle)
                .RegisterToWall();
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            EscapeNormally(catcher, (60, 100), (0.8f, 1f));
        }

        public override void ModifyIVLevel(ref FairyIV fairyIV, FairyCatcherPlayer fcp)
        {
            fairyIV.StaminaLevel = 100;
        }

        public override void OnCatch(Player player, ref int catchPower)
        {
            targetVelocity = Helper.NextVec2Dir(0.8f, 1f);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.JungleSpore, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class SporesFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "SporesFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_Tackle>(),
                NewSkill<FSkill_SporeExplode>(),
                ];

        public override void SpawnFairyDust()
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Projectile.SpawnTrailDust(DustID.JungleSpore, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Projectile.SpawnTrailDust(DustID.JungleSpore, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
            }
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, 0.05f, 0.15f, 0.05f);
        }

        public override Vector2 GetRestSpeed()
        {
            float f =Timer * 0.1f + Projectile.identity * MathHelper.TwoPi / 6;
            return new Vector2(MathF.Sin(f) * 3, MathF.Cos(f) * 1.5f);
        }

        public override void OnStartUseSkill(NPC target)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Grass, Projectile.Center);
        }

        public override void OnKill_DeadEffect()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Grass, Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.ApprenticeStorm, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }

    public class FSkill_SporeExplode : FairySkill
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "SporeExplode";

        public LocalizedText Description { get; set; }

        public override Color SkillTextColor => Color.Lime;

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = iv.SkillLevel;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.FairySkillBonus[Type].ModifyLevel(level);

            return Description.Format(GetDamageBonus(iv.Damage, level));
        }

        public override bool CanUseSkill(BaseFairyProjectile fairyProj)
            => false;

        public override bool Update(BaseFairyProjectile fairyProj)
            => true;

        public static int GetDamageBonus(int baseDamage, int level)
        {
            return (int)(baseDamage * (2.5f + 3f * Math.Clamp(level / 15f, 0, 1)));
        }

        public override void OnFairyKill(BaseFairyProjectile fairyProj)
        {
            SpawnSkillText(fairyProj.Projectile.Top);
            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < 7; i++)
            {
                fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                    , rot.ToRotationVector2() * 5, ProjectileID.SporeCloud, GetDamageBonus(15, fairyProj.SkillLevel), 2);
                fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                    , (rot + MathHelper.TwoPi / 14).ToRotationVector2() * 2, ProjectileID.SporeCloud, GetDamageBonus(15, fairyProj.SkillLevel), 2);
                rot += MathHelper.TwoPi / 7 + Main.rand.NextFloat(-0.2f, 0.2f);
            }
        }
    }
}
