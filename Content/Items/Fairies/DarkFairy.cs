using Coralite.Content.DamageClasses;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Fairies
{
    public class DarkFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<DarkFairy>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<DarkFairyProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_ShootDark>()
                ];
        }
    }

    public class DarkFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<DarkFairyItem>();

        public override FairyRarity Rarity => FairyRarity.C;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.DownedSlimeKing)
                .AddCondition(FairySpawnCondition.NightTime)
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
                Dust d = Dust.NewDustPerfect(Center, DustID.Smoke, Helper.NextVec2Dir(0.5f, 1.5f), 50, Color.Black);
                d.noGravity = true;
            }
        }
    }

    public class DarkFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "DarkFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_ShootDark>()
                ];

        public override void SpawnFairyDust(Vector2 center, Vector2 velocity)
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.Smoke, Main.rand.NextFloat(0.1f, 0.5f), 50, Color.Black);
                    break;
                case AIStates.Skill:
                default:
                    Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.Smoke, Main.rand.NextFloat(0.1f, 0.5f), 50, Color.Black);
                    break;
            }
        }

        public override Vector2 GetRestSpeed()
        {
            float a = Timer * 0.2f + Projectile.identity * MathHelper.TwoPi / 6;
            return new Vector2(MathF.Cos(a), MathF.Sin(a)) * 2;
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));
        }

        public override void OnStartUseSkill(NPC target)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
        }

        public override void OnKill_DeadEffect()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Helper.NextVec2Dir(1, 2), 50, Color.Black);
                d.noGravity = true;
            }
        }
    }

    public class ShootDark : BaseHeldProj
    {
        public override string Texture => "Terraria/Images/Projectile_16";

        public ref float Scale => ref Projectile.ai[0];

        public const int trailCachesLength = 6;

        public override void SetDefaults()
        {
            Projectile.DamageType = TrueFairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 16;
        }

        public override void Initialize()
        {
            Projectile.InitOldPosCache(trailCachesLength);
            Projectile.InitOldRotCache(trailCachesLength);
        }

        public override void AI()
        {
            Projectile.UpdateOldPosCache(addVelocity: true);
            Projectile.UpdateOldRotCache();

            Projectile.SpawnTrailDust(8f, DustID.Smoke, -Main.rand.NextFloat(0.1f, 0.4f)
                , 50, Color.Black, Scale: Main.rand.NextFloat(0.6f, 0.8f));
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(5))
            {
                PRTLoader.NewParticle<HorizontalStar>(Projectile.Center
                    , -Projectile.velocity.SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(1, 2), Color.Black
                    , Main.rand.NextFloat(0.2f, 0.3f));
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (i * MathHelper.PiOver2).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, dir * (1 + (j * 0.8f))
                        , 50, Color.Black, Scale: 1.6f - (j * 0.15f));
                    d.noGravity = true;
                }
            }

            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);

            Projectile.NewProjectileFromThis<DarkBoom>(Projectile.Center, Vector2.Zero, Projectile.damage, 0, Scale);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails();
            Texture2D mainTex = Projectile.GetTexture();
            Color c = Color.Black;

            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, pos, null, c, 0, origin, 0.7f, 0, 0);
            c = Color.Red;

            Main.spriteBatch.Draw(mainTex, pos, null, c * 0.7f, MathHelper.PiOver4, origin, 0.5f, 0, 0);
            return false;
        }

        public virtual void DrawTrails()
        {
            Texture2D Texture = CoraliteAssets.Trail.CircleA.Value;

            List<ColoredVertex> bars = new();

            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i] - Main.screenPosition;
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center + (normal * 5 * factor);
                Vector2 Bottom = Center - (normal * 5 * factor);

                var color = Color.Black * factor;
                bars.Add(new(Top, color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, color, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        }
    }

    public class DarkBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Scale => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.DamageType = TrueFairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.width = Projectile.height = 80;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Timer == 0)
            {
                Timer = 1;
                Projectile.scale = Scale;
                int width = (int)(Projectile.width * Scale);
                Projectile.Resize(width, width);

                if (VaultUtils.isServer)
                    return;

                Color black = Color.Black;
                Vector2 center = Projectile.Center;
                int type = CoraliteContent.ParticleType<LightBall>();

                for (int i = 0; i < 2; i++)
                {
                    PRTLoader.NewParticle(center, Helper.NextVec2Dir() * Main.rand.NextFloat(16, 18) * Scale,
                        type, black, Main.rand.NextFloat(0.1f, 0.15f));
                }
                for (int i = 0; i < 5; i++)
                {
                    PRTLoader.NewParticle(center, Helper.NextVec2Dir() * Main.rand.NextFloat(10, 14) * Scale
                        , type, black, Main.rand.NextFloat(0.1f, 0.15f));
                    PRTLoader.NewParticle(center, Helper.NextVec2Dir() * Main.rand.NextFloat(10, 14) * Scale
                        , type, Color.White, Main.rand.NextFloat(0.05f, 0.1f));
                    Dust dust = Dust.NewDustPerfect(center, DustID.Smoke, Helper.NextVec2Dir() * Main.rand.NextFloat(4, 8) * Scale
                        , Scale: Main.rand.NextFloat(1.6f, 1.8f));
                    dust.noGravity = true;
                }

                RedExplosionParticle particle = PRTLoader.NewParticle<RedExplosionParticle>(center, Vector2.Zero, Color.Black, 0);
                particle.scaleAdder = 0.4f * Scale / 8;
                particle.PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;

                RedGlowParticle particle2 = PRTLoader.NewParticle<RedGlowParticle>(center, Vector2.Zero, Color.Black, 0.2f);
                particle2.scaleAdder = (0.35f - 0.2f) / 6;
                particle2.PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;

                particle2 = PRTLoader.NewParticle<RedGlowParticle>(center, Vector2.Zero, Color.White, 0.2f);
                particle2.scaleAdder = (0.35f - 0.2f) / 6;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D tex = CoraliteAssets.Trail.ArrowSPA.Value;

            //float alpha = 1 - Timer / 14;
            //Vector2 pos = Projectile.Center - Main.screenPosition;
            //Color c = Color.LightGoldenrodYellow * 0.75f * alpha;
            //float scale = Projectile.scale * 0.4f;
            //Vector2 Scale = new Vector2(0.8f, alpha) * scale;

            //Main.spriteBatch.Draw(tex, pos, null, c, Projectile.rotation, tex.Size() / 2
            //    , Scale, 0, 0);

            //c.A = 0;
            //Main.spriteBatch.Draw(tex, pos, null, c, Projectile.rotation, tex.Size() / 2
            //    , Scale, 0, 0);

            return false;
        }
    }

    public class FSkill_ShootDark : FSkill_ShootProj
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "Shadow";

        public LocalizedText Description { get; set; }

        public override Color SkillTextColor => Color.DarkGray;
        protected override float ShootSpeed => 16;

        protected override int ProjType => ModContent.ProjectileType<ShootDark>();

        protected override float ChaseDistance => 300;

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = iv.SkillLevel;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.GetFairySkillBonus(Type, level);

            return Description.Format(GetProjScale(level), GetDamage(iv.Damage, level));
        }

        public float GetProjScale(int level)
        {
            if (level < 100)
                return 1 + 1.5f * Helper.X2Ease(level / 100f);

            return 1 + 1.5f;
        }

        public override void ShootProj(BaseFairyProjectile fairyProj, Vector2 center, Vector2 velocity, int damage)
        {
            int level = fairyProj.FairyItem.FairyIV.SkillLevel;
            if (fairyProj.Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.GetFairySkillBonus(Type, level);

            float scale = GetProjScale(level);
            int proj = fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                 , velocity, ProjType, damage, fairyProj.Projectile.knockBack, scale);
        }
    }
}
