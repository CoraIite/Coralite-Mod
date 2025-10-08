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
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Fairies
{
    public class DirtiryItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<Dirtiry>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<DirtiryProj>();
        }

        public override int[] GetFairySkills()
        {
            return [
                CoraliteContent.FairySkillType<FSkill_ShootDirt>()
                ];
        }
    }

    public class Dirtiry : Fairy
    {
        public override int ItemType => ModContent.ItemType<DirtiryItem>();

        public override FairyRarity Rarity => FairyRarity.C;

        public override int VerticalFrames => 5;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.DownedSlimeKing)
                .RegisterToWallGroup(FairySpawnController.WallGroupType.Dirt);
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
                Dust d = Dust.NewDustPerfect(Center, DustID.Dirt, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class DirtiryProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "Dirtiry";

        public override int FrameY => 5;

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_ShootDirt>()
                ];

        public override void SpawnFairyDust(Vector2 center, Vector2 velocity)
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.Dirt, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.Dirt, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
            }
        }

        public override Vector2 GetRestSpeed()
        {
            float a = Timer * 0.2f + Projectile.identity * MathHelper.TwoPi / 6;
            return new Vector2(MathF.Cos(a), MathF.Sin(a) * 4);
        }

        public override void AIAfter()
        {
            Projectile.rotation = Projectile.velocity.X / 20;
        }

        public override void OnStartUseSkill(NPC target)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Dig, Projectile.Center);
        }

        public override void OnKill_DeadEffect()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Dig, Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }

    public class ShootDirt : ModProjectile
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Projectile.DamageType = TrueFairyDamage.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 16;
        }

        public override void AI()
        {
            Projectile.Falling(8, 0.2f);
            Projectile.SpawnTrailDust(DustID.Dirt, Main.rand.NextFloat(0.2f, 0.4f));
            Projectile.rotation += MathF.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 65;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, Helper.NextVec2Dir(1, 2));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[ProjectileID.DirtBall].Value;

            tex.QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition, lightColor, Projectile.rotation, Projectile.scale);

            return false;
        }
    }

    public class FSkill_ShootDirt : FSkill_ShootProj
    {
        public override string Texture => AssetDirectory.FairySkillIcons + "Misc";

        public LocalizedText Description { get; set; }

        public override Color SkillTextColor => Color.SaddleBrown;
        protected override float ShootSpeed => 9;

        protected override int ProjType => ModContent.ProjectileType<ShootDirt>();

        protected override float ChaseDistance => 120;

        public override string GetSkillTips(Player player, FairyIV iv)
        {
            int level = iv.SkillLevel;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                level = fcp.GetFairySkillBonus(Type, level);

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
                    fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                        , velocity.RotatedBy(i * 0.2f), ProjType, damage, fairyProj.Projectile.knockBack);
            }
            else
                fairyProj.Projectile.NewProjectileFromThis(fairyProj.Projectile.Center
                      , velocity, ProjType, damage, fairyProj.Projectile.knockBack);
        }
    }
}
