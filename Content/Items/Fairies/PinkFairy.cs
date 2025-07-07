using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies
{
    public class PinkFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<PinkFairy>();
        public override FairyRarity Rarity => FairyRarity.R;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0, 0, 3);
            Item.shoot = ModContent.ProjectileType<PinkFairyProj>();
        }
    }

    public class PinkFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<PinkFairyItem>();

        public override FairyRarity Rarity => FairyRarity.R;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddConditions(FairySpawnCondition.ZoneForest,FairySpawnCondition.CircleR_9)
                .RegisterToWall();
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            EscapeNormally(catcher, (60, 90), (1, 1.6f));
        }

        public override void OnCatch(Player player, ref int catchPower)
        {
            targetVelocity = Helper.NextVec2Dir(1.4f, 1.6f);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.PinkFairy, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class PinkFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "PinkFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<Tackle>()
                ];

        public override void SpawnFairyDust()
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Projectile.SpawnTrailDust(DustID.PinkFairy, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Projectile.SpawnTrailDust(DustID.PinkFairy, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
            }
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.2f);
        }

        public override Vector2 GetRestSpeed()
        {
            return (MathHelper.PiOver4 + (Timer / 20) * MathHelper.PiOver2 + Projectile.identity * MathHelper.TwoPi / 6).ToRotationVector2() * 2;
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
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PinkFairy, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }
}
