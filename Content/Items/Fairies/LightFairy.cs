using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Fairies
{
    public class LightFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<LightFairy>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<LightFairyProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_ShootStar>()
                ];
        }
    }

    public class LightFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<LightFairyItem>();

        public override FairyRarity Rarity => FairyRarity.C;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.DownedSlimeKing)
                .AddCondition(FairySpawnCondition.DayTime)
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
                Dust d = Dust.NewDustPerfect(Center, DustID.YellowStarDust, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class LightFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "LightFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_ShootStar>()
                ];

        public override void SpawnFairyDust(Vector2 center, Vector2 velocity)
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.YellowStarDust, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.YellowStarDust, Main.rand.NextFloat(0.1f, 0.5f), 200);
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
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }

    public class ShootStar : ModProjectile
    {
        public override string Texture => AssetDirectory.Sparkles + "ShotLineSPA";

        public ref float Count => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 18);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = TrueFairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
            Projectile.width = Projectile.height = 16;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
            }

            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(4f, DustID.YellowStarDust, Main.rand.NextFloat(0.3f, 0.6f), 100
                    , new Color(255, 255, 255, 40));

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3)
                    , 100, new Color(255, 255, 255, 30), 1f);
                dust.noGravity = true;
            }

            for (int i = 0; i < Count; i++)
            {
                Projectile.NewProjectileFromThis<StarArrow>(Projectile.Center
                    , (i / Count * MathHelper.TwoPi).ToRotationVector2() * (3 + Count)
                    , (int)(Projectile.damage * 0.5f), Projectile.knockBack);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(Color.Gold, 0.5f, 0.5f / 18, 0, 18, 1, 0.2f / 18, 0, 0.2f);

            Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, Projectile.oldPos[0] + new Vector2(8, 8) - Main.screenPosition,
                Color.White * 0.8f, Color.Gold, 0.5f, 0f, 0.5f, 0.5f, 0f, 0f,
                new Vector2(1f, 1f), Vector2.One * 0.5f);
            return false;
        }
    }

    public class StarArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = TrueFairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.width = Projectile.height = 40;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));

            Timer++;
            Projectile.velocity *= 0.94f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer > 14)
                Projectile.Kill();

            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(4f, DustID.YellowStarDust, Main.rand.NextFloat(-0.8f, -0.6f), 100
                    , new Color(255, 255, 255, 40));
        }

        public override void OnKill(int timeLeft)
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CoraliteAssets.Trail.ArrowSPA.Value;
            Texture2D tex2 = CoraliteAssets.Sparkle.ShotLineSPA.Value;

            float alpha = 1 - Timer / 14;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = Color.LightGoldenrodYellow * 0.75f * alpha;
            float scale = Projectile.scale * 0.4f;
            Vector2 Scale = new Vector2(0.8f, alpha) * scale;

            Vector2 toCenter = Projectile.Size / 2 - Main.screenPosition;
            for (int i = 0; i < 5; i++)
            {
                Main.spriteBatch.Draw(tex2, Projectile.oldPos[i] + toCenter, null,
                    c * (0.5f - (i * 0.5f / 5)), Projectile.oldRot[i], tex2.Size() / 2, Scale * (1 - (i * 0.1f)), 0, 0);
            }

            c = Color.Gold * 0.75f * alpha;

            Main.spriteBatch.Draw(tex, pos, null, c, Projectile.rotation, tex.Size() / 2
                , Scale, 0, 0);

            c.A = 0;
            Main.spriteBatch.Draw(tex, pos, null, c, Projectile.rotation, tex.Size() / 2
                , Scale, 0, 0);

            return false;
        }
    }

    public class FSkill_ShootStar : FSkill_ShootProj
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "Light";

        public LocalizedText Description { get; set; }

        public override Color SkillTextColor => Color.Yellow;
        protected override float ShootSpeed => 16;

        protected override int ProjType => ModContent.ProjectileType<ShootStar>();

        protected override float ChaseDistance => 300;

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = iv.SkillLevel;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.GetFairySkillBonus(Type, level);

            return Description.Format(GetEXProjCount(level), GetDamage(iv.Damage, level));
        }

        public int GetEXProjCount(int level)
        {
            int count = 4;
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

            int count = GetEXProjCount(level);
            int proj = fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                 , velocity, ProjType, damage, fairyProj.Projectile.knockBack, count);
        }
    }
}
