using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies.ColorSeries
{
    public class GreenFairyItem : TackleFairyItem
    {
        public override string Texture => AssetDirectory.ColorFairySeries + Name;
        public override int FairyType => CoraliteContent.FairyType<GreenFairy>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<GreenFairyProj>();
        }
    }

    public class GreenFairy : Fairy
    {
        public override string Texture => AssetDirectory.ColorFairySeries + Name;
        public override int ItemType => ModContent.ItemType<GreenFairyItem>();

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneForest)
                .RegisterToWall();
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            EscapeNormally(catcher, (60, 80), (0.8f, 1.2f));
        }

        public override void OnCatch(Player player, ref int catchPower)
        {
            targetVelocity = Helper.NextVec2Dir(0.8f, 1.2f);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.GreenFairy, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }

        public override void ModifyIVLevel(FairyIV fairyIV, FairyCatcherPlayer fcp)
        {
            //fairyIV.LifeMaxLevel = 7;
            //fairyIV.DamageLevel = 7;
            //fairyIV.DefenceLevel = 7;
            //fairyIV.SkillLevelLevel = 7;
            //fairyIV.StaminaLevel = 7;
            //fairyIV.SpeedLevel = 7;
            //fairyIV.ScaleLevel = 7;
        }
    }

    public class GreenFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.ColorFairySeries + "GreenFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_Tackle>()
                ];

        public override void SpawnFairyDust(Vector2 center, Vector2 velocity)
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.GreenFairy, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Helper.SpawnTrailDust(center, velocity, Projectile.width, DustID.GreenFairy, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
            }
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, 0, 0.2f, 0);
        }

        public override Vector2 GetRestSpeed()
        {
            return (Timer * 0.1f+Projectile.identity*MathHelper.TwoPi/6).ToRotationVector2() * 2;
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
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GreenFairy, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }
}
