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
    public class YellowFairyItem : TackleFairyItem
    {
        public override string Texture => AssetDirectory.ColorFairySeries + Name;
        public override int FairyType => CoraliteContent.FairyType<YellowFairy>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 50);
            Item.shoot = ModContent.ProjectileType<YellowFairyProj>();
        }
    }

    public class YellowFairy : Fairy
    {
        public override string Texture => AssetDirectory.ColorFairySeries + Name;
        public override int ItemType => ModContent.ItemType<YellowFairyItem>();

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneDesert)
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
                Dust d = Dust.NewDustPerfect(Center, DustID.YellowTorch, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class YellowFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.ColorFairySeries + "YellowFairy";

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
                        Projectile.SpawnTrailDust(DustID.YellowTorch, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Projectile.SpawnTrailDust(DustID.YellowTorch, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
            }
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 0.05f);
        }

        public override Vector2 GetRestSpeed()
        {
            int dir = (Timer + Projectile.identity * 3) % 40 < 20 
                ? 1 : -1;
            return new Vector2(dir * 2, 0);
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
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }
}
