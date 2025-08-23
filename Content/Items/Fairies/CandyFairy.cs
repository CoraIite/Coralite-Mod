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

namespace Coralite.Content.Items.Fairies
{
    public class CandyFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<CandyFairy>();
        public override FairyRarity Rarity => FairyRarity.U;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 2);
            Item.shoot = ModContent.ProjectileType<CandyFairyProj>();
        }

        public override int[] GetFairySkills()
        {
            return [CoraliteContent.FairySkillType<FSkill_HealOutside>()];
        }
    }

    public class CandyFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<CandyFairyItem>();
        public override int VerticalFrames => 4;

        public override FairyRarity Rarity => FairyRarity.U;

        public bool RandomRolling;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .RegisterToWallGroup(FairySpawnController.WallGroupType.Gem);
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            FairyTimer--;
            if (RandomRolling)
                targetVelocity = targetVelocity.RotateByRandom(0.06f, 0.12f);

            if (FairyTimer <= 0)
            {
                RandomRolling = Main.rand.NextBool(3);
                FairyTimer = Main.rand.Next(30, 120);
                if (Main.rand.NextBool(3))
                    Helper.PlayPitched("Fairy/FairyMove" + Main.rand.Next(2), 0.3f, 0, position);
                Vector2 webCenter = catcher.webCenter;
                Vector2 dir;
                if (Vector2.Distance(Center, webCenter) > catcher.webRadius * 2 / 3)
                    dir = (webCenter - Center)
                        .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f);
                else
                    dir = (Center - catcher.Owner.Center)
                            .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f);

                if (RandomRolling)
                    targetVelocity = dir * Main.rand.NextFloat(1f, 1.5f);
                else
                    targetVelocity = dir * Main.rand.NextFloat(0.6f, 0.9f);
            }

            velocity = (velocity * 10 + targetVelocity) / 11;
        }

        public override void OnCatch(Player player, ref int catchPower)
        {
            targetVelocity = Helper.NextVec2Dir(0.6f, 0.8f);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.AncientLight, Helper.NextVec2Dir(0.5f, 1.5f));
                d.noGravity = true;
            }
        }
    }

    public class CandyFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "CandyFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_HealOutside>()
                ];

        public override void SpawnFairyDust(Vector2 center, Vector2 velocity)
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.RedTorch, Main.rand.NextFloat(0.1f, 0.5f));
                    break;
                case AIStates.Skill:
                default:
                    Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.RedTorch, Main.rand.NextFloat(0.1f, 0.5f));
                    break;
            }
        }

        public override void Rest()
        {
            Vector2 restSpeed = GetRestSpeed();
            if (Vector2.Distance(Owner.Center, Projectile.Center) > AttackDistance)
            {
                restSpeed += (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * IVSpeed;
            }

            float slowTime = 20;

            Projectile.velocity = ((Projectile.velocity * slowTime) + restSpeed) / (slowTime + 1);
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, 0.15f, 0.05f, 0.05f);
        }

        public override Vector2 GetRestSpeed()
        {
            return new Vector2(MathF.Cos(Timer * 0.2f)*3, MathF.Sin(Timer * 0.1f));
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
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RedTorch, Helper.NextVec2Dir(1, 2));
                d.noGravity = true;
            }
        }
    }
}
