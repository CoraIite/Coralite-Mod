using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using InnoVault.PRT;
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
                level = fcp.GetFairySkillBonus(Type, level);

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
                fairyProj.Projectile.NewProjectileFromThis<SporesExplode>(fairyProj.Projectile.Center
                    , rot.ToRotationVector2() * 5, GetDamageBonus(15, fairyProj.SkillLevel), 2);
                fairyProj.Projectile.NewProjectileFromThis<SporesExplode>(fairyProj.Projectile.Center
                    , (rot + MathHelper.TwoPi / 14).ToRotationVector2() * 2, GetDamageBonus(15, fairyProj.SkillLevel), 2,1);
                rot += MathHelper.TwoPi / 7 + Main.rand.NextFloat(-0.2f, 0.2f);
            }
        }
    }

    /// <summary>
    /// 使用ai0判断弹幕类型，0大弹幕，1小弹幕
    /// </summary>
    public class SporesExplode : ModProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + Name;

        public ref float ProjType => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public ref float Alpha => ref Projectile.localAI[0];

        private float scaleMult = 1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.scale = 0.65f;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                Projectile.localAI[2] = Main.rand.Next(2);
            }

            switch (State)
            {
                default:
                case 0://生成
                    {
                        Alpha += 0.15f;
                        if (Alpha > 1)
                            Alpha = 1;

                        if (++Timer > 5)
                        {
                            Timer = 0;
                            State = 1;
                        }

                        Projectile.velocity *= 1.02f;
                        Projectile.SpawnTrailDust(DustID.JungleSpore, -Main.rand.NextFloat(0.2f, 0.4f)
                            , Scale: Main.rand.NextFloat(0.6f, 1f));
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;
                case 1:
                    {
                        Timer++;

                        int maxTime = 35;
                        if (ProjType == 1)
                            maxTime = 15;

                        if (Timer < maxTime)
                        {
                            Projectile.rotation = Projectile.velocity.ToRotation();

                            Projectile.SpawnTrailDust(DustID.JungleSpore, -Main.rand.NextFloat(0.2f, 0.4f)
                                , Scale: Main.rand.NextFloat(0.6f, 1f));

                            return;
                        }

                        if (Timer == maxTime)
                        {
                            int size = 75;
                            if (ProjType == 1)
                                size = 50;

                            Projectile.Resize(size, size);
                            return;
                        }

                        int currTime = (int)Timer - maxTime;

                        if (currTime < 20)
                            scaleMult += 0.01f;

                        Projectile.velocity *= 0.86f;

                        if (currTime == 3 * 4)
                            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        else if (currTime > 3 * 4)
                            Projectile.rotation += Projectile.velocity.Length() / 25;

                        if (currTime > 3 * 3 && currTime < 3 * 9 && Main.rand.NextBool(3))
                            PRTLoader.NewParticle<TwistFog>(Projectile.Center + Helper.NextVec2Dir(Projectile.width * 0.1f, Projectile.width * 0.3f)
                                , Helper.NextVec2Dir(0.2f, 0.4f)
                                , Main.rand.NextFromList(Color.Lime, Color.LimeGreen) with { A = 100 }, Main.rand.NextFloat(0.3f, 0.8f));

                        //for (int i = 0; i < 2; i++)
                        if (Main.rand.NextBool(2))
                        {
                            Vector2 dir = Helper.NextVec2Dir(Projectile.width * 0.3f, Projectile.width * 0.6f);
                            Vector2 pos = Projectile.Center + dir;
                            Vector2 velocity = dir.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(2, 4);

                            Dust d = Dust.NewDustPerfect(pos, DustID.JungleTorch, velocity, Scale: Main.rand.NextFloat(1, 1.5f));
                            d.noGravity = true;
                        }

                        if (currTime % 4 == 0)
                        {
                            Projectile.frame++;
                        }

                        if (currTime > 4*10)
                            Projectile.Kill();
                    }
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Fade();
            Projectile.tileCollide = false;
            Projectile.velocity *= 0;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Fade();

            //Projectile.damage = (int)(Projectile.damage * 0.95f);
            Projectile.velocity *= 0.9f;
        }

        public void Fade()
        {
            State = 1;
            Alpha = 1;
            int maxTime = 45;
            int size = 60;

            if (ProjType == 1)
            {
                size = 36;
                maxTime = 30;
            }

            if (Timer < maxTime)
            {
                Timer = maxTime;
            }
            if (Projectile.width != size)
                Projectile.Resize(size, size);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.frame > 9)
                return false;

            var frame = Projectile.GetTexture()
                .Frame(4, 10, (int)(ProjType * 2) + (int)Projectile.localAI[2], Projectile.frame);

            Projectile.QuickDraw(frame, lightColor, 0, scaleMult);

            return false;
        }
    }
}
