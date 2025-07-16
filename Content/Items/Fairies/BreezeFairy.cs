using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Fairies
{
    public class BreezeFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<BreezeFairy>();
        public override FairyRarity Rarity => FairyRarity.U;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<BreezeFairyProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_Blow>()
                ];
        }
    }

    public class BreezeFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<BreezeFairyItem>();

        public override FairyRarity Rarity => FairyRarity.U;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneForest)
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
                Dust d = Dust.NewDustPerfect(Center, DustID.AncientLight, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class BreezeFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "BreezeFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_Blow>()
                ];

        public override void SpawnFairyDust()
        {
            switch (State)
            {
                case AIStates.Shooting:
                    if (Main.rand.NextBool(3))
                        Projectile.SpawnTrailDust(DustID.Cloud, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Projectile.SpawnTrailDust(DustID.AncientLight, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Projectile.SpawnTrailDust(DustID.AncientLight, Main.rand.NextFloat(0.1f, 0.5f), 200);
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
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueFairy, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }

    public class BreezeBlow : BaseHeldProj
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 20;
            Projectile.width = Projectile.height = 240;
            Projectile.penetrate = -1;
        }

        public override void Initialize()
        {
            Projectile.scale = 1 + Math.Clamp(Projectile.ai[0], 0, 15) * 0.05f;
            Projectile.Resize((int)(Projectile.scale * Projectile.width), (int)(Projectile.scale * Projectile.height));
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f);
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(120, 120);
            Dust d = Dust.NewDustPerfect(pos, DustID.AncientLight, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 4f));
            d.noGravity = true;
            Projectile.ai[2]++;

            if (Projectile.ai[2] == 0)
            {
                Color c = Color.SkyBlue;
                c.A = 200;
                RedJades.RedGlowParticle.Spawn(Projectile.Center, Projectile.scale * 1.1f, c, 0.2f);
            }
            if (Projectile.ai[2] == 5)
            {
                Color c = Color.SkyBlue;
                c.A = 125;
                RedJades.RedGlowParticle.Spawn(Projectile.Center, Projectile.scale * 1.1f, c, 0.3f);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = target.Center.X - Projectile.Center.X > 0 ? 1 : -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class FSkill_Blow : FSkill_ShootProj
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "Blow";

        public LocalizedText Description { get; set; }

        public override Color SkillTextColor => Color.SkyBlue;
        protected override float ShootSpeed => 0;

        protected override int ProjType => ModContent.ProjectileType<BreezeBlow>();

        protected override float ChaseDistance => 100;

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = iv.SkillLevel;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.FairySkillBonus[Type].ModifyLevel(level);

            return Description.Format(GetDamageBonus(iv.Damage, level));
        }
    }
}
