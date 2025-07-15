using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies
{
    public class RedFairyItem : TackleFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<RedFairy>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<RedFairyProj>();
        }
    }

    public class RedFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<RedFairyItem>();
        public override int VerticalFrames => 4;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneCrimson)
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
                Dust d = Dust.NewDustPerfect(Center, DustID.CrimsonTorch, Helper.NextVec2Dir(0.5f, 1.5f));
                d.noGravity = true;
            }
        }
    }

    public class RedFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "RedFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_Tackle>()
                ];

        public override void SpawnFairyDust()
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Projectile.SpawnTrailDust(DustID.CrimsonTorch, Main.rand.NextFloat(0.1f, 0.5f));
                    break;
                case AIStates.Skill:
                default:
                    Projectile.SpawnTrailDust(DustID.CrimsonTorch, Main.rand.NextFloat(0.1f, 0.5f));
                    break;
            }
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0, 0);
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
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CrimsonTorch, Helper.NextVec2Dir(1, 2));
                d.noGravity = true;
            }
        }
    }
}
