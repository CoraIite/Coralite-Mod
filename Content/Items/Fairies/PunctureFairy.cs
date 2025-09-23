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

namespace Coralite.Content.Items.Fairies
{
    public class PunctureFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<PunctureFairy>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<PunctureFairyProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_Tackle>(),
                CoraliteContent.FairySkillType<FSkill_ShootDrill>()
                ];
        }
    }

    public class PunctureFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<PunctureFairyItem>();

        public override FairyRarity Rarity => FairyRarity.C;

        private int state;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.DownedRediancie)
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
                        , DustID.Platinum, -velocity * Main.rand.NextFloat(0.1f, 0.35f));
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
                Dust d = Dust.NewDustPerfect(Center, DustID.Platinum, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class PunctureFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "PunctureFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_Tackle>(),
                NewSkill<FSkill_ShootDrill>()
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

    public class PunctureDrill : ModProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + Name;

        public ref float ReflectCount => ref Projectile.ai[0];
        public ref float SlowTime => ref Projectile.ai[1];
        public ref float Speed => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 18;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = TrueFairyDamage.Instance;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Speed = Projectile.velocity.Length();
            }

            if (SlowTime > 0)
            {
                SlowTime--;

                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Speed / 2;
            }
            else
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Speed;

            Projectile.UpdateFrameNormally(4, 4);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ReflectCount--;
            if (ReflectCount < 1)
                Projectile.Kill();
            else
            {
                SlowTime = 45;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool top = oldVelocity.Y < 0 && Math.Sign(oldVelocity.Y + Projectile.velocity.X) < 0;
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX * 0.7f;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY * 0.7f;
            if (top)
                Projectile.velocity.Y *= -1;

            ReflectCount--;
            if (ReflectCount < 1)
                return true;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickFrameDraw(new Rectangle(0, Projectile.frame, 1, 4), lightColor, 0);

            return false;
        }
    }

    public class FSkill_ShootDrill : FSkill_ShootProj
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "Misc";

        public LocalizedText Description { get; set; }

        public override Color SkillTextColor => Color.LightGray;
        protected override float ShootSpeed => 9;

        protected override int ProjType => ModContent.ProjectileType<PunctureDrill>();

        protected override float ChaseDistance => 240;

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = Helper.GetBonusedSkillLevel(player, iv.SkillLevel, Type);

            return Description.Format(GetProjReflectCount(level), GetDamage(iv.Damage, level));
        }

        public int GetProjReflectCount(int level)
        {
            int count = Math.Clamp(3 + level / 10, 3, 13);

            return count;
        }

        public override void ShootProj(BaseFairyProjectile fairyProj, Vector2 center, Vector2 velocity, int damage)
        {
            int level = fairyProj.FairyItem.FairyIV.SkillLevel;
            if (fairyProj.Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.GetFairySkillBonus(Type, level);

            ;
            fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                 , velocity, ProjType, damage, fairyProj.Projectile.knockBack, GetProjReflectCount(level));
        }
    }
}
