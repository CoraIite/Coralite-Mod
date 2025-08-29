using Coralite.Content.DamageClasses;
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
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.Fairies
{
    public class PopsicleFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<PopsicleFairy>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<PopsicleFairyProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_IceFire>()
                ];
        }
    }

    public class PopsicleFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<PopsicleFairyItem>();

        public override FairyRarity Rarity => FairyRarity.C;

        private int state;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneSnow)
                .RegisterToWall();
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            if (state < 1)
            {
                EscapeNormally(catcher, (60, 100), (0.8f, 1f)
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

                if (Main.rand.NextBool())
                {
                    Dust d = Dust.NewDustPerfect(Center + Main.rand.NextVector2Circular(5, 5)
                        , DustID.ApprenticeStorm, -velocity * Main.rand.NextFloat(0.1f, 0.35f));
                    d.noGravity = true;
                }

                if (FairyTimer < 0)
                {
                    velocity = targetVelocity = Helper.NextVec2Dir(1.75f, 2.2f);
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
            if (state == 0)
                targetVelocity = Helper.NextVec2Dir(0.8f, 1f);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.AncientLight, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class PopsicleFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "PopsicleFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_IceFire>()
                ];

        public override void SpawnFairyDust(Vector2 center, Vector2 velocity)
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.ApprenticeStorm, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.ApprenticeStorm, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
            }
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 0.15f);
        }

        public override Vector2 GetRestSpeed()
        {
            return new Vector2(MathF.Sin(Timer * 0.1f + Projectile.identity * MathHelper.TwoPi / 6) * 3, 0);
        }

        public override void OnStartUseSkill(NPC target)
        {
            SoundEngine.PlaySound(CoraliteSoundID.DigIce, Projectile.Center);
        }

        public override void OnKill_DeadEffect()
        {
            SoundEngine.PlaySound(CoraliteSoundID.DigIce, Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.ApprenticeStorm, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }

    public class FSkill_IceFire : FSkill_ShootProj
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "Ice";

        public LocalizedText Description { get; set; }

        public override Color SkillTextColor => Color.LightCyan;
        protected override float ShootSpeed => 9;

        protected override int ProjType => ProjectileID.WandOfFrostingFrost;

        protected override float ChaseDistance => 120;

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = Helper.GetBonusedSkillLevel(player, iv.SkillLevel, Type);

            return Description.Format(GetProjCount(level), GetDamage(iv.Damage, level));
        }

        public int GetProjCount(int level)
        {
            int count = 1;
            if (level > 4)
                count++;
            if (level > 7)
                count++;
            if (level > 10)
                count += 2;

            return count;
        }

        public override void ShootProj(BaseFairyProjectile fairyProj, Vector2 center, Vector2 velocity, int damage)
        {
            int level = fairyProj.FairyItem.FairyIV.SkillLevel;
            if (fairyProj.Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.GetFairySkillBonus(Type, level);

            int count = GetProjCount(level);
            if (count > 1)
            {
                float v = (count - 1) / 2f;
                for (float i = -v; i <= v; i++)
                {
                    int proj = fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                         , velocity.RotatedBy(i * 0.2f), ProjType, damage, fairyProj.Projectile.knockBack);
                    Main.projectile[proj].DamageType = TrueFairyDamage.Instance;
                    Main.projectile[proj].usesIDStaticNPCImmunity = true;
                    Main.projectile[proj].idStaticNPCHitCooldown = 20;
                }
            }
            else
            {
                int proj = fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                     , velocity, ProjType, damage, fairyProj.Projectile.knockBack);
                Main.projectile[proj].DamageType = TrueFairyDamage.Instance;
                Main.projectile[proj].usesIDStaticNPCImmunity = true;
                Main.projectile[proj].idStaticNPCHitCooldown = 20;
            }
        }
    }
}
