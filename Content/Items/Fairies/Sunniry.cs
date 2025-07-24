using Coralite.Content.DamageClasses;
using Coralite.Content.Items.FairyCatcher.CircleCoreAccessories;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Fairies
{
    public class SunniryItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<Sunniry>();
        public override FairyRarity Rarity => FairyRarity.RRR;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<SunniryProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_Tackle>(),
                CoraliteContent.FairySkillType<FSkill_SunnyShot>(),
                ];
        }
    }

    public class Sunniry : Fairy
    {
        public override int ItemType => ModContent.ItemType<SunniryItem>();

        public override FairyRarity Rarity => FairyRarity.RRR;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(new FairySpawn_Circle(ModContent.ItemType<SunflowerRing>()))
                .RegisterToWall();
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            EscapeNormally(catcher, (60, 100), (1f, 1.5f));
        }

        public override void OnCatch(Player player, ref int catchPower)
        {
            targetVelocity = Helper.NextVec2Dir(1.5f, 1.75f);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.GoldCoin, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class SunniryProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "Sunniry";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_Tackle>(),
                NewSkill<FSkill_SunnyShot>(),
                ];

        public override void SpawnFairyDust()
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Projectile.SpawnTrailDust(DustID.GoldCoin, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Projectile.SpawnTrailDust(DustID.GoldCoin, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
            }
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, 0.1f, 0.15f, 0.15f);
        }

        public override Vector2 GetRestSpeed()
        {
            return new Vector2(0, MathF.Sin(Timer * 0.2f + Projectile.identity * MathHelper.TwoPi / 6) * 3);
        }

        public override void OnStartUseSkill(NPC target)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Fairy_NPCHit5, Projectile.Center);
        }

        public override void OnKill_DeadEffect()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Fairy_NPCHit5, Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }

    /// <summary>
    /// ai0传入拥有者，ai1传入角度
    /// </summary>
    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class SunnyShot : BaseHeldProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Rot => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public float Timer;

        public ref float Alpha => ref Projectile.localAI[0];
        public Vector2 Scale
        {
            get => new Vector2(Projectile.localAI[1], Projectile.localAI[2]);
            set
            {
                Projectile.localAI[1] = value.X;
                Projectile.localAI[2] = value.Y;
            }
        }

        //偷个懒借用一下日月同辉的颜色图
        public static ATex SolunarFlowGradient { get; private set; }

        private Vector2 oldCenter;
        private PRTGroup group;

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 12000;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
            => State == 1 && Timer < 10;

        public override bool ShouldUpdatePosition()
            => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CanHitLine(projHitbox.TopLeft(), projHitbox.Width, projHitbox.Height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height)
                && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center
                , Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[2] * 320, 60, ref a);
        }

        public override void AI()
        {
            if (!Projectile.ai[0].GetProjectileOwner(out Projectile owner, () => Projectile.Kill()))
                return;

            if (!VaultUtils.isServer)
                group ??= [];

            Projectile.Center = owner.Center;
            Projectile.rotation = Rot;
            Projectile.velocity = Projectile.rotation.ToRotationVector2();

            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3());
            switch (State)
            {
                default:
                case 0://捏在手里的阶段

                    Alpha = Helper.Lerp(Alpha, 0.2f, 0.2f);
                    Scale = Vector2.SmoothStep(Scale, new Vector2(1f, 0.3f), 0.2f);
                    Timer++;
                    if (Timer > 6)
                    {
                        State++;
                        Timer = 0;
                        if (Projectile.IsOwnedByLocalPlayer() && VisualEffectSystem.HitEffect_ScreenShaking)
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.rotation.ToRotationVector2(), 10, 7, 5, 500));

                        Helper.PlayPitched(CoraliteSoundID.LaserMachinegun_Item91, Projectile.Center);

                        return;
                    }
                    break;
                case 1://释放
                    if (Timer < 8)
                    {
                        Alpha = Helper.Lerp(Alpha, 1, 0.2f);

                        if (Timer < 4)
                        {
                            Projectile.localAI[2] = Helper.Lerp(0.3f, 1f, Timer / 4);
                        }
                        else
                            Projectile.localAI[2] = Helper.Lerp(Projectile.localAI[2], 0.4f, 0.5f);

                        Projectile.localAI[1] = Helper.Lerp(Projectile.localAI[1], 2.75f, 0.45f);

                        Vector2 dir = Projectile.rotation.ToRotationVector2();
                        Vector2 normal = (Projectile.rotation + 1.57f).ToRotationVector2();

                        group.NewParticle<LaserLine>(Projectile.Center + normal * Scale.Y * 15 * Main.rand.NextFloat(-1, 1)
                            + dir * Main.rand.NextFloat(0, 150), dir * Main.rand.NextFloat(5, 12)
                            , Main.rand.NextFromList(new Color(108, 237, 124), Color.Gold), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                    }

                    else if (Timer < 22)
                    {
                        Alpha = Helper.Lerp(Alpha, 0, 0.2f);
                        Scale = Vector2.SmoothStep(Scale, new Vector2(1.75f, 0), 0.35f);

                        Vector2 dir = Projectile.rotation.ToRotationVector2();
                        Vector2 normal = (Projectile.rotation + 1.57f).ToRotationVector2();
                        for (int i = 0; i < 2; i++)
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center + normal * Scale.Y * 20 * Main.rand.NextFloat(-1, 1)
                                + dir * Main.rand.NextFloat(0, 220), DustID.GoldCoin
                                , dir * 4, Scale: Main.rand.NextFloat(1, 1.5f));
                            d.noGravity = true;
                        }

                        if (Main.rand.NextBool())
                            group.NewParticle<LaserLine>(Projectile.Center + normal * Scale.Y * 15 * Main.rand.NextFloat(-1, 1)
                                + dir * Main.rand.NextFloat(0, 150), dir * Main.rand.NextFloat(5, 12)
                                , Main.rand.NextFromList(new Color(108, 237, 124), Color.Gold), Scale: Main.rand.NextFloat(0.2f, 0.35f));
                    }
                    else
                    {
                        Projectile.Kill();
                    }
                    break;
            }

            if (!VaultUtils.isServer)
            {
                if (oldCenter != Vector2.Zero)
                {
                    foreach (var p in group)
                        p.Position += Projectile.Center - oldCenter;
                }

                group?.Update();
                oldCenter = Projectile.Center;
            }

            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
            {
                Vector2 dir = (Projectile.rotation + 1.57f + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2();

                for (int i = -1; i < 2; i += 2)
                {
                    Helper.SpawnDirDustJet(target.Center, () => i * dir, 1, 5, i => 1 + i * Main.rand.NextFloat(2, 4)
                        , DustID.GoldFlame, Scale: 1.35f);
                }
            }
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            group?.Draw(spriteBatch);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CoraliteAssets.Trail.LightShot.Value;
            Vector2 pos = Projectile.Center - Projectile.rotation.ToRotationVector2() * 12;
            Vector2 scale = Scale;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.03f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(tex);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(SolunarFlowGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.Tunnel.Value);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            Vector2 origin = new Vector2(tex.Width * 5 / 6, tex.Height / 2);
            float rotation = Projectile.rotation + MathHelper.Pi;

            for (int i = 0; i < 2; i++)
                spriteBatch.Draw(tex, pos, null
                    , Color.White, rotation, origin, scale * 1.1f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(tex, pos - Main.screenPosition, null
                , new Color(255, 255, 255, 0) * 0.65f * Alpha, rotation, origin, scale * 1.1f, 0, 0);

            return base.PreDraw(ref lightColor);
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.FairyItems)]
    public class FSkill_SunnyShot : FSkill_ShootProj
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "Light";

        protected override int ReadyTime => 180;

        public static ATex SunniryGlow { get; set; }
        public LocalizedText Description { get; set; }

        public override Color SkillTextColor => Color.Yellow;
        protected override float ShootSpeed => 0;

        protected override int ProjType => ModContent.ProjectileType<SunnyShot>();

        protected override float ChaseDistance => 220;

        public override void BeforeShoot(BaseFairyProjectile fairyProj)
        {
            if (Main.rand.NextBool())
            {
                float length = 20 + SkillTimer / (float)ReadyTime * 20;
                Vector2 dir = Helper.NextVec2Dir(length - 5, length + 5);
                Dust d = Dust.NewDustPerfect(fairyProj.Projectile.Center + dir, DustID.GoldCoin
                    , -dir.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 1.5f), Scale: Main.rand.NextFloat(0.3f, 0.6f));
                d.noGravity = true;
            }

            if (fairyProj.TargetIndex.GetNPCOwner(out NPC owner) && Math.Abs(owner.Center.X - fairyProj.Projectile.Center.X) > 8)
                fairyProj.Projectile.spriteDirection = (owner.Center.X - fairyProj.Projectile.Center.X) > 0 ? 1 : -1;
        }

        public override int GetDamageBonus(int baseDamage, int level)
        {
            return (int)(baseDamage * (4f + 3f * Math.Clamp(level / 15f, 0, 1)));
        }

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = iv.SkillLevel;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.FairySkillBonus[Type].ModifyLevel(level);

            return Description.Format(GetDamageBonus(iv.Damage, level));
        }

        public override void ShootProj(BaseFairyProjectile fairyProj, Vector2 center, Vector2 velocity, int damage)
        {
            if (fairyProj.TargetIndex.GetNPCOwner(out NPC owner))
            {
                float rot = (owner.Center - fairyProj.Projectile.Center).ToRotation();
                fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                    , velocity, ProjType, damage, fairyProj.Projectile.knockBack, fairyProj.Projectile.whoAmI
                    , rot);

                fairyProj.Projectile.velocity = -rot.ToRotationVector2() * 2;
            }
        }

        public override void PreDrawSpecial(BaseFairyProjectile fairyProj, ref Color lightColor)
        {
            float factor = 1 - SkillTimer / (float)ReadyTime;

            float alpha = 1;
            if (factor < 0.2f)
            {
                alpha = factor / 0.2f;
            }
            else if (factor > 0.7f)
            {
                alpha = 1 - (factor - 0.7f) / 0.3f;
            }

            float scale = 1f - Helper.X2Ease(factor) * 0.8f;

            SunniryGlow.Value.QuickCenteredDraw(Main.spriteBatch, fairyProj.Projectile.Center - Main.screenPosition
                , new Color(230, 230, 230, 0) * alpha, 0, scale);
        }
    }
}
